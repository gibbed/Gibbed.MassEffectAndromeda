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
using Newtonsoft.Json;
using LocalizedStringFile = Gibbed.Frostbite3.ResourceFormats.LocalizedStringFile;

namespace Gibbed.MassEffectAndromeda.DumpItemTypes
{
    public class Program
    {
        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        public static void Main(string[] args)
        {
            var dumper = new Dumping.Dumper(true);
            dumper.AddRequiredSuperbundle(
                "win32/globals",
                "win32/loctext/en");
            dumper.Main(args, "item_types.json", Dump);
        }

        private static void Dump(Dumping.Dumper dumper,
                                 Dictionary<Guid, Dumping.PartitionMap.PartitionInfo> partitionMap,
                                 string outputPath)
        {
            var itemTypes = new Dictionary<string, ItemInfo>();

            Logger.Info("Loading globalmaster...");
            var globalMaster = new LocalizedStringFile();
            using (var input = dumper.LoadResource("game/localization/config/texttable/en/game/globalmaster",
                                                   "localizedstringresource"))
            {
                globalMaster.Deserialize(input);
            }

            Logger.Info("Loading masteritemlist...");
            var masterItemListReader = dumper.LoadEbx("game/items/masteritemlist");
            if (masterItemListReader == null)
            {
                Logger.Fatal("Failed to load masteritemlist.");
                return;
            }
            using (masterItemListReader)
            {
                var masterItemList = masterItemListReader.GetObjectsOfSpecificType("MasterItemList").First();
                foreach (var itemAsset in masterItemList.ItemAssets)
                {
                    Dumping.PartitionMap.PartitionInfo partitionInfo;
                    if (partitionMap.TryGetValue(itemAsset.PartitionId, out partitionInfo) == false)
                    {
                        Logger.Warn("Failed to find partition info for {0}!", itemAsset.PartitionId);
                        continue;
                    }

                    dumper.MountSuperbundle(partitionInfo.Superbundles.First());

                    Logger.Info("Loading item '{0}'...", partitionInfo.Name);
                    var itemDataReader = dumper.LoadEbx(partitionInfo.Name, typeof(ItemType));
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

        private struct ItemInfo
        {
            public ItemType Type;
            public uint ItemHash;
            public string Name;
            public int Tier;
            public bool IsHidden;
        }

        private static string Decode(LocalizedStringFile table, string text)
        {
            bool shouldContinue;
            do
            {
                shouldContinue = false;
                text = Regex.Replace(
                    text,
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
    }
}
