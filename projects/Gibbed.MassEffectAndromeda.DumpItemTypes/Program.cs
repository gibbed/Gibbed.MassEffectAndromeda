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
using PartitionFile = Gibbed.Frostbite3.ResourceFormats.PartitionFile;
using PartitionReader = Gibbed.Frostbite3.Dynamic.PartitionReader;

namespace Gibbed.MassEffectAndromeda.DumpItemTypes
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

            if (extras.Count < 2 || extras.Count > 3 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ game_dir partition_map [output_json]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            LogHelper.SetConfiguration(NLog.LogLevel.FromOrdinal(logLevelOrdinal));

            var dataBasePath = extras[0];
            var partitionMapPath = extras[1];
            var outputPath = extras.Count > 2 ? extras[2] : Path.Combine(dataBasePath, "item_types.json");

            var partitionMap = PartitionMap.Load(partitionMapPath);

            var dataManager = DataManager.Initialize(dataBasePath, noPatch);
            if (dataManager == null)
            {
                Logger.Fatal("Could not initialize superbundle manager.");
                return;
            }

            if (dataManager.MountSuperbundle("win32/globals") == null)
            {
                Logger.Fatal("Failed to mount win32/globals.");
                return;
            }

            var itemTypes = new Dictionary<string, ItemInfo>();

            Logger.Info("Loading masteritemlist...");
            var masterItemListReader = LoadEbx(dataManager, "game/items/masteritemlist");
            if (masterItemListReader == null)
            {
                Logger.Fatal("Failed to load game/items/masteritemlist.");
                return;
            }
            using (masterItemListReader)
            {
                var masterItemList = masterItemListReader.GetObjectsOfSpecificType("MasterItemList").First();
                foreach (var itemAsset in masterItemList.ItemAssets)
                {
                    PartitionMap.PartitionInfo partitionInfo;
                    if (partitionMap.TryGetValue(itemAsset.PartitionId, out partitionInfo) == false)
                    {
                        Logger.Warn("Failed to find partition info for {0}!", itemAsset.PartitionId);
                        continue;
                    }

                    dataManager.MountSuperbundle(partitionInfo.Superbundles.First());

                    Logger.Info("Loading item '{0}'...", partitionInfo.Name);
                    var itemDataReader = LoadEbx(dataManager, partitionInfo.Name, typeof(ItemType));
                    if (itemDataReader == null)
                    {
                        Logger.Warn("Failed to load item data '{0}'!", partitionInfo.Name);
                        continue;
                    }
                    using (itemDataReader)
                    {
                        var itemData = itemDataReader.GetObject(itemAsset.InstanceId);
                        itemTypes[partitionInfo.Name] = new ItemInfo()
                        {
                            Type = itemData.ItemType,
                            ItemHash = itemData.ItemHash,
                        };
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
                foreach (var kv in itemTypes.OrderBy(kv => kv.Key))
                {
                    var info = kv.Value;
                    writer.WritePropertyName(kv.Key);
                    writer.WriteStartObject();
                    writer.WritePropertyName("type");
                    writer.WriteValue(info.Type.ToString());
                    writer.WritePropertyName("item_hash");
                    writer.WriteValue(info.ItemHash);
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
        }

        private struct ItemInfo
        {
            public ItemType Type;
            public uint ItemHash;
        }

        private static PartitionReader LoadEbx(DataManager dataManager, string name, params Type[] enumTypes)
        {
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
