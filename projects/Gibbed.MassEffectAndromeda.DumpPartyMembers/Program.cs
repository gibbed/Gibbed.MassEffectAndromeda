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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using LocalizedStringFile = Gibbed.Frostbite3.ResourceFormats.LocalizedStringFile;

namespace Gibbed.MassEffectAndromeda.DumpPartyMembers
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
                "win32/loctext/en",
                "win32/characters_party",
                "win32/game/levels/rootlevel/rootlevel");
            dumper.Main(args, "party_members.json", Dump);
        }

        private static void Dump(Dumping.Dumper dumper,
                                 Dictionary<Guid, Dumping.PartitionMap.PartitionInfo> partitionMap,
                                 string outputPath)
        {
            Logger.Info("Loading globalmaster...");
            var globalMaster = new LocalizedStringFile();
            using (var input = dumper.LoadResource("game/localization/config/texttable/en/game/globalmaster",
                                                   "localizedstringresource"))
            {
                globalMaster.Deserialize(input);
            }

            var blueprintNames = new List<string>()
            {
                "dev/characters/player/player",
            };

            Logger.Info("Loading bundlecollection_partymember...");
            var partyMemberCollectionReader = dumper.LoadEbx("game/characters/party/bundlecollection_partymember");
            if (partyMemberCollectionReader == null)
            {
                Logger.Fatal("Failed to load bundlecollection_partymember.");
                return;
            }
            using (partyMemberCollectionReader)
            {
                var partyMemberCollection = partyMemberCollectionReader
                    .GetObjectsOfSpecificType("BlueprintBundleCollection").First();
                foreach (var bundle in partyMemberCollection.Bundles)
                {
                    string bundleName = bundle.Name;

                    var blueprintBundleReader = dumper.LoadEbx(bundleName.ToLowerInvariant());
                    if (blueprintBundleReader == null)
                    {
                        Logger.Fatal("Failed to load {0}.", bundle.Name);
                        continue;
                    }
                    using (blueprintBundleReader)
                    {
                        var blueprintBundle = blueprintBundleReader.GetObjectsOfSpecificType("BlueprintBundle").First();
                        var blueprint = blueprintBundle.Blueprint;
                        Dumping.PartitionMap.PartitionInfo partitionInfo;
                        if (partitionMap.TryGetValue(blueprint.PartitionId, out partitionInfo) == false)
                        {
                            Logger.Warn("Failed to find partition info for {0}!", blueprint.PartitionId);
                            continue;
                        }
                        blueprintNames.Add(partitionInfo.Name);
                        Logger.Info("Found '{0}' => '{1}'.", bundleName, partitionInfo.Name);
                    }
                }
            }

            var partyMembers = new List<PartyMemberInfo>();
            foreach (var blueprintName in blueprintNames)
            {
                var blueprintReader = dumper.LoadEbx(blueprintName);
                if (blueprintReader == null)
                {
                    Logger.Fatal("Failed to load {0}.", blueprintName);
                    continue;
                }
                Logger.Info("Processing '{0}'...", blueprintName);
                using (blueprintReader)
                {
                    var soldierBlueprint = blueprintReader.GetObjectsOfSpecificType("MESoldierBlueprint").First();
                    var soldier = soldierBlueprint.Object;
                    if (soldier == null)
                    {
                        Logger.Fatal("Missing MESoldierBlueprint!");
                        continue;
                    }
                    var soldierBodyComponent = GetComponent(soldier, "SoldierBodyComponentData");
                    if (soldierBodyComponent == null)
                    {
                        Logger.Fatal("Missing SoldierBodyComponentData!");
                        continue;
                    }
                    var partyMemberComponent = GetComponent(soldierBodyComponent, "MEPartyMemberComponentData");
                    if (partyMemberComponent == null)
                    {
                        Logger.Fatal("Missing MEPartyMemberComponentData!");
                        continue;
                    }
                    var progressionComponent = GetComponent(soldierBodyComponent, "ProgressionComponentData");
                    if (progressionComponent == null)
                    {
                        Logger.Fatal("Missing ProgressionComponentData!");
                        continue;
                    }
                    var targetableComponent = GetComponent(soldierBodyComponent, "AIMETargetableComponentData");

                    Dumping.PartitionMap.PartitionInfo skillProgressionPartitionInfo;
                    if (partitionMap.TryGetValue(progressionComponent.SkillProgressionAsset.PartitionId,
                                                 out skillProgressionPartitionInfo) == false)
                    {
                        Logger.Warn("Failed to find partition info for {0}!",
                                    progressionComponent.SkillProgressionAsset.PartitionId);
                        continue;
                    }

                    var skillProgressionReader = dumper.LoadEbx(skillProgressionPartitionInfo.Name);
                    if (skillProgressionReader == null)
                    {
                        Logger.Fatal("Failed to load {0}.", skillProgressionPartitionInfo.Name);
                        continue;
                    }
                    using (skillProgressionReader)
                    {
                        var skillProgression =
                            skillProgressionReader.GetObjectsOfSpecificType("SkillProgression").First();

                        PartyMemberInfo partyMember;
                        partyMember.Id = partyMemberComponent.CharacterId;

                        if (targetableComponent == null)
                        {
                            partyMember.Name = "Ryder";
                        }
                        else
                        {
                            int nameId = targetableComponent.ARInfoPanelData.TargetName.StringId;
                            partyMember.Name = Decode(globalMaster, (uint)nameId);
                        }

                        partyMember.ExcludeProfiles = skillProgression.ExcludeProfiles;
                        partyMember.ExcludePresets = skillProgression.ExcludePresets;
                        partyMembers.Add(partyMember);
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
                foreach (var partyMember in partyMembers.OrderBy(i => i.Id))
                {
                    writer.WritePropertyName(partyMember.Id.ToString(CultureInfo.InvariantCulture));
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(partyMember.Name);
                    writer.WritePropertyName("exclude_profiles");
                    writer.WriteValue(partyMember.ExcludeProfiles);
                    writer.WritePropertyName("exclude_presets");
                    writer.WriteValue(partyMember.ExcludePresets);
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
        }

        private struct PartyMemberInfo
        {
            public int Id;
            public string Name;
            public bool ExcludeProfiles;
            public bool ExcludePresets;
        }

        private static dynamic GetComponent(dynamic obj, string name)
        {
            foreach (var component in obj.Components)
            {
                if (component.__TYPE == name)
                {
                    return component;
                }
            }
            return null;
        }

        private static string Decode(LocalizedStringFile table, uint id)
        {
            return Decode(table, table.Get(id));
        }

        private static string Decode(LocalizedStringFile table, string text)
        {
            bool shouldContinue;
            do
            {
                shouldContinue = false;
                text = Regex.Replace(text ?? "",
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
