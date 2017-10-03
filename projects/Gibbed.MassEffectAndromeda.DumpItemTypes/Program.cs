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
using System.Text.RegularExpressions;
using Gibbed.Frostbite3.Unbundling;
using NDesk.Options;
using Newtonsoft.Json;
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;
using LocalizedStringFile = Gibbed.Frostbite3.ResourceFormats.LocalizedStringFile;
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

            if (dataManager.MountSuperbundle("win32/loctext/en") == null)
            {
                Logger.Fatal("Failed to mount win32/loctext/en.");
                return;
            }

            var itemTypes = new Dictionary<string, ItemInfo>();

            Logger.Info("Loading globalmaster...");
            var globalMaster = new LocalizedStringFile();
            using (var input = LoadResource(dataManager,
                                            "game/localization/config/texttable/en/game/globalmaster",
                                            (int)DJB.Compute("localizedstringresource")))
            {
                globalMaster.Deserialize(input);
            }

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

                        var type = (ItemType)itemData.ItemType;
                        int tier = itemData.HasMember("TierValue") == false || itemData.TierValue == null
                                       ? -1
                                       : itemData.TierValue.ConstValue;
                        var itemInfo = new ItemInfo()
                        {
                            Type = type,
                            ItemHash = itemData.ItemHash,
                            Name = globalMaster.Get((uint)itemData.DisplayName.StringId),
                            Tier = tier,
                            IsHidden = itemData.HideInInventory,
                        };
                        itemTypes[partitionInfo.Name] = itemInfo;
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
                    if (string.IsNullOrEmpty(info.Name) == false)
                    {
                        writer.WritePropertyName("name");
                        writer.WriteValue(Decode(globalMaster, info.Name));
                    }
                    if (info.Tier >= 0)
                    {
                        writer.WritePropertyName("tier");
                        writer.WriteValue(info.Tier);
                    }
                    if (info.IsHidden == true)
                    {
                        writer.WritePropertyName("is_hidden");
                        writer.WriteValue(true);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
        }

        private static string Decode(LocalizedStringFile table, string text)
        {
            bool shouldContinue;
            do
            {
                shouldContinue = false;
                text = Regex.Replace(text,
                                     @"{string}(?<id>\d+){/string}",
                                     m =>
                                     {
                                         shouldContinue = true;
                                         return ReplaceToken(table, m);
                                     });
            }
            while (shouldContinue == true);
            return text;
        }

        private static string ReplaceToken(LocalizedStringFile table, Match match)
        {
            var idText = match.Groups["id"].Value;
            uint id;
            if (uint.TryParse(idText, out id) == false)
            {
                throw new InvalidOperationException();
            }
            return table.Get(id);
        }

        private struct ItemInfo
        {
            public ItemType Type;
            public uint ItemHash;
            public string Name;
            public int Tier;
            public bool IsHidden;
        }

        private static MemoryStream LoadResource(DataManager dataManager, string name, int type)
        {
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
