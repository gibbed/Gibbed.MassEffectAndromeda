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
using Gibbed.Frostbite3.Unbundling;
using NDesk.Options;
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;
using PartitionFile = Gibbed.Frostbite3.ResourceFormats.PartitionFile;
using PartitionReader = Gibbed.Frostbite3.Dynamic.PartitionReader;

namespace Gibbed.MassEffectAndromeda.Dumping
{
    public class Dumper
    {
        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        private readonly bool _LoadPartitionMap;
        private readonly List<string> _RequiredSuperbundleNames;
        private DataManager _DataManager;

        public Dumper(bool loadPartitionMap)
        {
            this._LoadPartitionMap = loadPartitionMap;
            this._RequiredSuperbundleNames = new List<string>();
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().CodeBase);
        }

        private static void IncreaseLogLevel(string arg, ref int level)
        {
            if (arg == null || level <= 0)
            {
                return;
            }

            level--;
        }

        public void AddRequiredSuperbundle(params string[] names)
        {
            if (names != null)
            {
                foreach (var name in names)
                {
                    this._RequiredSuperbundleNames.Add(name);
                }
            }
        }

        public delegate void DumpDelegate(Dumper dumper, Dictionary<Guid, PartitionMap.PartitionInfo> partitionMap, string outputPath); 

        public void Main(string[] args, string defaultOutputPath, DumpDelegate callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            int logLevelOrdinal = 3;
            bool noPatch = false;
            bool showHelp = false;

            var options = new OptionSet()
            {
                { "no-patch", "don't use patch data", v => noPatch = v != null },
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

            if (this._LoadPartitionMap == false)
            {
                if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
                {
                    Console.WriteLine("Usage: {0} [OPTIONS]+ game_dir [output_json]", GetExecutableName());
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    options.WriteOptionDescriptions(Console.Out);
                    return;
                }
            }
            else
            {
                if (extras.Count < 2 || extras.Count > 3 || showHelp == true)
                {
                    Console.WriteLine("Usage: {0} [OPTIONS]+ game_dir partition_map [output_json]", GetExecutableName());
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    options.WriteOptionDescriptions(Console.Out);
                    return;
                }
            }

            LogHelper.SetConfiguration(NLog.LogLevel.FromOrdinal(logLevelOrdinal));

            var dataBasePath = extras[0];
            Dictionary<Guid, PartitionMap.PartitionInfo> partitionMap;
            string outputPath;

            if (this._LoadPartitionMap == false)
            {
                partitionMap = null;
                outputPath = extras.Count > 1 ? extras[1] : Path.Combine(dataBasePath, defaultOutputPath);
            }
            else
            {
                var partitionMapPath = extras[1];
                partitionMap = PartitionMap.Load(partitionMapPath);
                outputPath = extras.Count > 2 ? extras[2] : Path.Combine(dataBasePath, defaultOutputPath);
            }

            var dataManager = DataManager.Initialize(dataBasePath, noPatch);
            if (dataManager == null)
            {
                Logger.Fatal("Could not initialize superbundle manager.");
                return;
            }

            foreach (var superbundleName in this._RequiredSuperbundleNames.Distinct())
            {
                if (dataManager.MountSuperbundle(superbundleName) == null)
                {
                    Logger.Fatal("Failed to mount '{0}'.", superbundleName);
                    return;
                }
            }

            this._DataManager = dataManager;
            callback(this, partitionMap, outputPath);
        }

        public Frostbite3.VfsFormats.SuperbundleFile MountSuperbundle(string name)
        {
            var dataManager = this._DataManager;
            if (dataManager == null)
            {
                return null;
            }
            return dataManager.MountSuperbundle(name);
        }

        public MemoryStream LoadResource(string name, string type)
        {
            return LoadResource(name, (int)DJB.Compute(type));
        }

        public MemoryStream LoadResource(string name, int type)
        {
            var dataManager = this._DataManager;
            if (dataManager == null)
            {
                return null;
            }

            var info = dataManager.GetResourceInfo(name, type);
            if (info == null)
            {
                return null;
            }

            var data = new MemoryStream();
            dataManager.LoadData(info, data);
            data.Position = 0;
            return data;
        }

        public PartitionReader LoadEbx(string name, params Type[] enumTypes)
        {
            var dataManager = this._DataManager;
            if (dataManager == null)
            {
                return null;
            }

            var info = dataManager.GetEbxInfo(name);
            if (info == null)
            {
                return null;
            }

            using (var data = new MemoryStream())
            {
                dataManager.LoadData(info, data);
                data.Position = 0;

                var partition = new PartitionFile();
                partition.Deserialize(data);
                return new PartitionReader(partition, enumTypes);
            }
        }
    }
}
