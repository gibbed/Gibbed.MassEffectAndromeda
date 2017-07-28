/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.Frostbite3.Unbundling;
using NDesk.Options;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.DumpPlotFlags
{
    internal class Program
    {
        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        private static void IncreaseLogLevel(string arg, ref int level)
        {
            if (arg == null || level <= 0)
            {
                return;
            }

            level--;
        }

        public static void Main(string[] args)
        {
            int logLevelOrdinal = 3;
            bool noPatch = false;
            bool noCatch = false;
            bool showHelp = false;

            var options = new OptionSet()
            {
                { "no-patch", "don't use patch data", v => noPatch = v != null },
                { "no-catch", "don't catch exceptions when loading data", v => noCatch = v != null },
                { "v|verbose", "increase log level (-v/-vv/-vvv)", v => IncreaseLogLevel(v, ref logLevelOrdinal) },
                { "h|help", "show this message and exit", v => showHelp = v != null },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ game_dir [output_dir]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            LogHelper.SetConfiguration(NLog.LogLevel.FromOrdinal(logLevelOrdinal));

            var dataBasePath = extras[0];
            var outputPath = extras.Count > 1 ? extras[1] : Path.Combine(dataBasePath, "plot_flags.json");

            var dataManager = DataManager.Initialize(dataBasePath, noPatch);
            if (dataManager == null)
            {
                Logger.Fatal("Could not initialize superbundle manager.");
                return;
            }

            var superbundles = new Dictionary<string, Frostbite3.VfsFormats.SuperbundleFile>();
            foreach (var superbundleName in dataManager.Superbundles)
            {
                var superbundle = dataManager.MountSuperbundle(superbundleName);
                if (superbundle == null)
                {
                    continue;
                }
                superbundles.Add(superbundleName, superbundle);
            }

            var dumper = new Dumper();

            foreach (var ebxInfo in GetPartitionInfos(superbundles))
            {
                var partition = new Frostbite3.ResourceFormats.PartitionFile();

                using (var data = new MemoryStream())
                {
                    Logger.Info(ebxInfo.Name);

                    if (noCatch == true)
                    {
                        dataManager.LoadData(ebxInfo, data);
                        data.Position = 0;
                    }
                    else
                    {
                        try
                        {
                            dataManager.LoadData(ebxInfo, data);
                            data.Position = 0;
                        }
                        catch (ChunkCryptoKeyMissingException e)
                        {
                            Logger.Warn("Cannot decrypt '{0}' without crypto key '{1}'.",
                                        ebxInfo.Name,
                                        e.KeyId);
                            continue;
                        }
                        catch (Exception e)
                        {
                            Logger.Warn(e, "Exception while loading '{0}':", ebxInfo.Name);
                            continue;
                        }
                    }

                    partition.Deserialize(data);
                }

                dumper.DumpPartition(ebxInfo, partition);
            }

            dataManager.Dispose();
            WriteOutput(outputPath, dumper.Results);
        }

        private static void WriteOutput(string outputPath,
                                        Dictionary<Guid, Dictionary<string, List<string>>> foundPlotFlags)
        {
            using (var textWriter = new StreamWriter(outputPath, false, Encoding.UTF8))
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 2;
                writer.WriteStartObject();
                foreach (var kv in foundPlotFlags.OrderBy(kv => kv.Key))
                {
                    if (kv.Value.Count == 0)
                    {
                        continue;
                    }

                    writer.WritePropertyName(kv.Key.ToString());
                    writer.WriteStartObject();
                    foreach (var kv2 in kv.Value.OrderBy(kv2 => kv2.Key))
                    {
                        writer.WritePropertyName(kv2.Key);
                        writer.WriteStartArray();
                        foreach (var info in kv2.Value.Distinct().OrderBy(v => v))
                        {
                            writer.WriteValue(info);
                        }
                        writer.WriteEndArray();
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
        }

        private static IEnumerable<Frostbite3.VfsFormats.Superbundle.EbxInfo> GetPartitionInfos(
            Dictionary<string, Frostbite3.VfsFormats.SuperbundleFile> superbundles)
        {
            return superbundles.Values.Where(sbf => sbf.Bundles != null)
                               .SelectMany(sbf => sbf.Bundles)
                               .Where(bi => bi.Ebx != null)
                               .SelectMany(bi => bi.Ebx)
                               .OrderBy(bi => bi.Name)
                               .GroupBy(ei => ei.Name, ei => ei)
                               .Select(kv => kv.First());
        }
    }
}
