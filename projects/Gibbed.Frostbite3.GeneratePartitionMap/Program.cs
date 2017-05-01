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

namespace Gibbed.Frostbite3.GeneratePartitionMap
{
    public class Program
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
            var outputPath = extras.Count > 1 ? extras[1] : Path.Combine(dataBasePath, "partition.map.json");

            var dataManager = DataManager.Initialize(dataBasePath, noPatch);
            if (dataManager == null)
            {
                Logger.Fatal("Could not initialize superbundle manager.");
                return;
            }

            var superbundles = new Dictionary<string, VfsFormats.SuperbundleFile>();
            foreach (var superbundleName in dataManager.Superbundles)
            {
                var superbundle = dataManager.MountSuperbundle(superbundleName);
                if (superbundle == null)
                {
                    continue;
                }
                superbundles.Add(superbundleName, superbundle);
            }

            var guidMap = new Dictionary<Guid, Dictionary<string, List<string>>>();

            foreach (var kv in superbundles.Where(kv => kv.Value.Bundles != null))
            {
                var superbundleName = kv.Key;
                foreach (var ebxInfo in kv.Value.Bundles.Where(bi => bi.Ebx != null)
                                          .SelectMany(bi => bi.Ebx)
                                          .OrderBy(bi => bi.Name))
                {
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

                        var partition = new ResourceFormats.PartitionFile();
                        partition.Deserialize(data);

                        Dictionary<string, List<string>> guidEntries;
                        if (guidMap.TryGetValue(partition.Guid, out guidEntries) == false)
                        {
                            guidMap[partition.Guid] = guidEntries = new Dictionary<string, List<string>>();
                        }

                        List<string> superbundleEntries;
                        if (guidEntries.TryGetValue(ebxInfo.Name, out superbundleEntries) == false)
                        {
                            guidEntries[ebxInfo.Name] = superbundleEntries = new List<string>();
                        }

                        if (superbundleEntries.Contains(superbundleName) == false)
                        {
                            superbundleEntries.Add(superbundleName);
                        }
                    }
                }
            }

            using (var textWriter = new StreamWriter(outputPath, false, Encoding.UTF8))
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 2;
                writer.WriteStartObject();
                foreach (var kv in guidMap.OrderBy(kv => kv.Key))
                {
                    if (kv.Value.Count == 0)
                    {
                        continue;
                    }

                    if (kv.Value.Count > 1)
                    {
                        throw new InvalidOperationException();
                    }

                    writer.WritePropertyName(kv.Key.ToString());

                    var kv2 = kv.Value.First();

                    var oldFormatting = writer.Formatting;
                    writer.Formatting = Formatting.None;
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(kv2.Key);
                    writer.WritePropertyName("superbundles");
                    writer.WriteStartArray();
                    foreach (var superbundleName in kv2.Value.OrderBy(sn => sn))
                    {
                        writer.WriteValue(superbundleName);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    writer.Formatting = oldFormatting;
                }
                writer.WriteEndObject();
            }

            dataManager.Dispose();
        }
    }
}
