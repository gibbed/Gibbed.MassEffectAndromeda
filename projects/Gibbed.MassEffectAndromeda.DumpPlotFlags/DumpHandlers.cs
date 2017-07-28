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
using EbxInfo = Gibbed.Frostbite3.VfsFormats.Superbundle.EbxInfo;

namespace Gibbed.MassEffectAndromeda.DumpPlotFlags
{
    internal static class DumpHandlers
    {
        public delegate void DumpHandler(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data);

        public static Dictionary<string, DumpHandler> Create()
        {
            return new Dictionary<string, DumpHandler>()
            {
                { "BoolProvider_PlotCondition", DumpGetPlotCondition },
                { "ConversationBoolProvider_Plot", DumpGetPlotCondition },
                { "StorytellerRulePlotConditionData", DumpGetPlotCondition },
                { "IntegerProvider_PlotStateValue", DumpGetPlotFlag },
                { "PlotIntegerFlagNotificationEntityData", DumpGetPlotFlag },
                { "ConditionLogicPrefabBlueprint", DumpGetPlotFlagIds },
                { "PlotGetBooleanEntityData", DumpGetPlotFlagReference },
                { "PlotGetFloatEntityData", DumpGetPlotFlagReference },
                { "PlotGetIntegerEntityData", DumpGetPlotFlagReference },
                { "PlotClearBooleanEntityData", DumpGetPlotFlagReference },
                { "PlotSetBooleanEntityData", DumpGetPlotFlagReference },
                { "PlotSetIntegerEntityData", DumpSetPlotFlagReferenceValue },
                { "PlotSetFloatEntityData", DumpSetPlotFlagReferenceValue },
                { "PlotIncrementIntegerByValueEntityData", DumpSetPlotFlagReferenceValue },
                { "PlotIncrementIntegerByPlotFlagValueEntityData", DumpSetPlotFlagValue },
                { "DebugBooleanPlotFlag", DumpDebugBooleanPlotFlag },
                { "DebugIntegerPlotFlag", DumpDebugIntegerPlotFlag },
                { "PlotConditionEntityData", DumpGetCondition },
                { "PlotTimelineTrackCondition", DumpGetConditions },
                { "PlotLogicConditionRule", DumpCompareCondition },
                { "PlotActionEntityData", DumpAction },
                { "GameVaultVarInt", DumpGameVaultVarInt },
                { "MEWeaponList", DumpDebugCondition },
                { "MECasualOutfitList", DumpDebugCondition },
                { "MEConsumableItemList", DumpDebugCondition },
                { "MEGearItemList", DumpDebugCondition },
                { "MEPowerList", DumpDebugCondition },
                { "MESpaceToolList", DumpDebugCondition },
                { "MEMeleeWeaponList", DumpDebugCondition },
                { "PlotConditionNotificationEntityData", DumpPlotConditions },
                { "PlotLogicBooleanFlagRule", DumpPlotLogicBooleanFlagRule },
                { "PlotLogicIntFlagRule", DumpPlotLogicIntFlagRule },
                { "PlotCompareIntegerEntityData", DumpPlotCompareIntegerEntityData },
                { "MECraftingResearchProjectItemData", DumpCraftingResearchProjectItemData },
                { "MECCodexEntry", DumpCodexEntry },
                { "JournalEntry", DumpJournalEntry },
                { "ConversationConditionData", DumpConversationConditionData },
                { "PlotLocationMarkerEntityData", DumpPlotLocationMarkerEntityData },
                { "ConversationEntityData", DumpConversationEntityData },
                { "ConversationLine", DumpConversationLine },
                { "AwardListenerEntityData", DumpAwardListenerEntityData },
                { "PlotFlagListenerData", DumpPlotFlagListenerData },
                { "TelemetryManagerData", DumpTelemetryManagerData },
                { "PlotSubLevelStreamingTriggerEntityData", DumpPlotSubLevelStreamingTriggerEntityData },
                { "GalaxyCompletionData", DumpGalaxyCompletionData },
                { "ActivePlayTimerEntityData", DumpActivePlayTimerEntityData },
                { "LootManagerEntityData", DumpLootManagerEntityData },
                { "PartyManagerManagerEntityData", DumpPartyManagerManagerEntityData },
                { "ViabilitySystemManagerEntityData", DumpViabilitySystemManagerEntityData },
                { "ItemCountTrackerAsset", DumpItemCountTrackerAsset },
                { "MESaveLevelContext", DumpSaveLevelContext },
                { "SystemData", DumpSystemData },
                { "DestinationData", DumpDestinationData },
                { "PerksUIDataProviderData", DumpPerksUIDataProviderData },
                { "TutorialEntry", DumpTutorialEntry },
                { "PlotLocationMarkerComponentData", DumpPlotLocationMarkerComponentData },
                { "ConversationTypeSettings", DumpConversationTypeSettings },
                { "ConversationCategory", DumpConversationCategory },
                { "SystemDataReference", DumpSystemDataReference },
                { "ProgressiveTaskData", DumpProgressiveTaskData },
                { "JournalTextData", DumpJournalContentData },
                { "JournalTaskData", DumpJournalTaskData },
                { "PlotConfigurationAsset", DumpPlotConfigurationAsset },
                { "GiveJournalPlotFlagReward", DumpGiveJournalPlotFlagReward },
            };
        }

        private static void DumpAction(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddActionResult(ebxInfo, data.Action, typeName, "Action");
        }

        private static void DumpGetPlotCondition(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddConditionResult(ebxInfo, data.PlotCondition, typeName, "PlotCondition");
        }

        private static void DumpGetCondition(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddConditionResult(ebxInfo, data.Condition, typeName, "Condition");
        }

        private static void DumpGetConditions(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var conditions = (object[])data.Conditions;
            if (conditions == null || conditions.Length == 0)
            {
                return;
            }

            foreach (dynamic condition in conditions)
            {
                dumper.AddConditionResult(ebxInfo, condition, typeName, "Conditions");
            }
        }

        private static void DumpCompareCondition(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.DesiredValue.ToString();
            dumper.AddConditionResult(ebxInfo, data.Condition, callback, typeName, "Condition");
        }

        private static void DumpGetPlotFlag(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddGuidResult(ebxInfo, (Guid)data.PlotFlag.PlotFlagId.Guid, typeName, "PlotFlag");
        }

        private static void DumpGetPlotFlagIds(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var flags = (object[])data.PlotFlagIds;
            if (flags == null || flags.Length == 0)
            {
                return;
            }

            foreach (dynamic flag in flags)
            {
                var guid = (Guid)flag.Guid;
                if (guid == Guid.Empty)
                {
                    continue;
                }

                dumper.AddGuidResult(ebxInfo, guid, typeName, "PlotFlagIds");
            }
        }

        private static void DumpGetPlotFlagReference(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddGuidResult(ebxInfo, (Guid)data.PlotFlagReference.PlotFlagId.Guid, typeName, "PlotFlagReference");
        }

        private static void DumpSetPlotFlagReferenceValue(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.Value.ToString();
            var guid = (Guid)data.PlotFlagReference.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, guid, callback, typeName, "PlotFlagReference");
        }

        private static void DumpSetPlotFlagValue(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var guid1 = (Guid)data.PlotFlagReference.PlotFlagId.Guid;
            var guid2 = (Guid)data.PlotFlagValue.PlotFlagId.Guid;
            Func<string> callback1 = () => guid2.ToString();
            Func<string> callback2 = () => guid1.ToString();
            dumper.AddGuidResult(ebxInfo, guid1, callback1, typeName, "PlotFlagReference");
            dumper.AddGuidResult(ebxInfo, guid2, callback2, typeName, "PlotFlagValue");
        }

        private static void DumpDebugBooleanPlotFlag(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.Value.ToString();
            dumper.AddGuidResult(ebxInfo, (Guid)data.Target.PlotFlagId.Guid, callback, typeName, "Target");
        }

        private static void DumpDebugIntegerPlotFlag(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddGuidResult(ebxInfo, (Guid)data.Target.PlotFlagId.Guid, typeName, "Target");
            dumper.AddGuidResult(ebxInfo, (Guid)data.Source.PlotFlagId.Guid, typeName, "Source");
        }

        private static void DumpGameVaultVarInt(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback =
                () =>
                {
                    var defaultValue = (int)data.DefaultValue;
                    var minValue = (int)data.MinValue;
                    var maxValue = (int)data.MaxValue;
                    var variableNameOnServer = (string)data.VariableNameOnServer;
                    return string.IsNullOrEmpty(variableNameOnServer) == false
                               ? string.Format("default {0}, {1} to {2} ({3})",
                                               defaultValue,
                                               minValue,
                                               maxValue,
                                               variableNameOnServer)
                               : string.Format("default {0}, {1} to {2}",
                                               defaultValue,
                                               minValue,
                                               maxValue);
                };
            dumper.AddGuidResult(ebxInfo, (Guid)data.PlotFlag.PlotFlagId.Guid, callback, typeName, "PlotFlag");
        }

        private static void DumpDebugCondition(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddConditionResult(ebxInfo, data.DebugCondition, typeName, "DebugCondition");
        }

        private static void DumpPlotConditions(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var plotConditions = (object[])data.PlotConditions;
            if (plotConditions == null || plotConditions.Length == 0)
            {
                return;
            }

            foreach (dynamic plotCondition in plotConditions)
            {
                dumper.AddConditionResult(ebxInfo, plotCondition, typeName, "PlotConditions");
            }
        }

        private static void DumpPlotLogicBooleanFlagRule(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.DesiredValue.ToString();
            dumper.AddGuidResult(ebxInfo, (Guid)data.Flag.PlotFlagId.Guid, callback, typeName, "Flag");
        }

        private static void DumpPlotLogicIntFlagRule(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => string.Format("{0}, {1}", (PlotLogicOperator)data.Operator, data.DesiredValue);
            dumper.AddGuidResult(ebxInfo, (Guid)data.Flag.PlotFlagId.Guid, callback, typeName, "Flag");
        }

        private static void DumpPlotCompareIntegerEntityData(EbxInfo ebxInfo,
                                                             Dumper dumper,
                                                             string typeName,
                                                             dynamic data)
        {
            dumper.AddGuidResult(ebxInfo, (Guid)data.PlotFlagReferenceA.PlotFlagId.Guid, typeName, "PlotFlagReferenceA");
            dumper.AddGuidResult(ebxInfo, (Guid)data.PlotFlagReferenceB.PlotFlagId.Guid, typeName, "PlotFlagReferenceB");
        }

        private static void DumpCraftingResearchProjectItemData(EbxInfo ebxInfo,
                                                                Dumper dumper,
                                                                string typeName,
                                                                dynamic data)
        {
            // also data.ResearchNexusLevel.NexusLevelPlotFlag
            dumper.AddGuidResult(ebxInfo, (Guid)data.CompletedFlag.PlotFlagId.Guid, typeName, "CompletedFlag");
        }

        private static void DumpCodexEntry(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var displayConditions = (object[])data.DisplayConditions;
            if (displayConditions != null && displayConditions.Length > 0)
            {
                foreach (dynamic displayCondition in displayConditions)
                {
                    dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "DisplayConditions");
                }
            }

            var hideConditions = (object[])data.HideConditions;
            if (hideConditions != null && hideConditions.Length > 0)
            {
                foreach (dynamic hideCondition in hideConditions)
                {
                    dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "HideConditions");
                }
            }
        }

        private static void DumpJournalEntry(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var displayConditions = (object[])data.DisplayConditions;
            if (displayConditions != null && displayConditions.Length > 0)
            {
                foreach (dynamic displayCondition in displayConditions)
                {
                    dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "DisplayConditions");
                }
            }

            var failureConditions = (object[])data.FailureConditions;
            if (failureConditions != null && failureConditions.Length > 0)
            {
                foreach (dynamic failureCondition in failureConditions)
                {
                    dumper.AddConditionResult(ebxInfo, failureCondition, typeName, "FailureConditions");
                }
            }

            var hideConditions = (object[])data.HideConditions;
            if (hideConditions != null && hideConditions.Length > 0)
            {
                foreach (dynamic hideCondition in hideConditions)
                {
                    dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "HideConditions");
                }
            }
        }

        private static void DumpConversationConditionData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.DebugName;
            var activationConditions = (object[])data.ActivationConditions;
            if (activationConditions != null && activationConditions.Length > 0)
            {
                foreach (dynamic activationCondition in activationConditions)
                {
                    dumper.AddConditionResult(ebxInfo, activationCondition, callback, typeName, "ActivationConditions");
                }
            }
        }

        private static void DumpPlotLocationMarkerEntityData(EbxInfo ebxInfo,
                                                             Dumper dumper,
                                                             string typeName,
                                                             dynamic data)
        {
            dumper.AddConditionResult(ebxInfo, data.DisplayCondition, typeName, "DisplayCondition");
            dumper.AddConditionResult(ebxInfo, data.HideCondition, typeName, "HideCondition");
        }

        private static void DumpConversationEntityData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var flags = (object[])data.FirstLineTimelineTrackConditionFlags;
            if (flags == null || flags.Length == 0)
            {
                return;
            }

            foreach (dynamic flag in flags)
            {
                var guid = (Guid)flag.Guid;
                if (guid == Guid.Empty)
                {
                    continue;
                }

                dumper.AddGuidResult(ebxInfo, guid, typeName, "FirstLineTimelineTrackConditionFlags");
            }
        }

        private static void DumpConversationLine(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddGuidResult(ebxInfo, (Guid)data.OccurrencePlotFlagId.Guid, typeName, "OccurrencePlotFlagId");
        }

        private static void DumpAwardListenerEntityData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var plotAwards = (object[])data.PlotAwards;
            if (plotAwards == null || plotAwards.Length == 0)
            {
                return;
            }

            foreach (dynamic plotAward in plotAwards)
            {
                var plotFlags = (object[])plotAward.PlotFlags;
                if (plotFlags != null)
                {
                    foreach (dynamic plotFlag in plotFlags)
                    {
                        var guid = (Guid)plotFlag.PlotFlagId.Guid;
                        dumper.AddGuidResult(ebxInfo, guid, typeName, "PlotAwards", "PlotFlags");
                    }
                }
            }
        }

        private static void DumpPlotFlagListenerData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var guid1 = (Guid)data.NewGameStartedPlotFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, guid1, typeName, "NewGameStartedPlotFlag");
            var guid2 = (Guid)data.TempestEnterExitPlotFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, guid2, typeName, "TempestEnterExitPlotFlag");
            var guid3 = (Guid)data.TempestPilotingPlotFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, guid3, typeName, "TempestPilotingPlotFlag");

            var plotFlagsSettings = (object[])data.PlotFlagsSettings;
            if (plotFlagsSettings != null)
            {
                foreach (dynamic plotFlagSetting in plotFlagsSettings)
                {
                    var guid4 = (Guid)plotFlagSetting.PlotFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid4, typeName, "PlotFlagsSettings");
                }
            }
        }

        private static void DumpTelemetryManagerData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var critPathPlotCompletion = (object[])data.CritPathPlotCompletion;
            if (critPathPlotCompletion == null || critPathPlotCompletion.Length == 0)
            {
                return;
            }

            foreach (dynamic critPathPlot in critPathPlotCompletion)
            {
                var guid = (Guid)critPathPlot.PlotFlagId.Guid;
                dumper.AddGuidResult(ebxInfo, guid, typeName, "CritPathPlotCompletion");
            }
        }

        private static void DumpPlotSubLevelStreamingTriggerEntityData(EbxInfo ebxInfo,
                                                                       Dumper dumper,
                                                                       string typeName,
                                                                       dynamic data)
        {
            var runtimeStates = (object[])data.RuntimeStates;
            if (runtimeStates == null || runtimeStates.Length == 0)
            {
                return;
            }

            foreach (dynamic runtimeState in runtimeStates)
            {
                var conditions = (object[])runtimeState.Conditions;
                if (conditions == null || conditions.Length <= 0)
                {
                    continue;
                }

                foreach (var condition in conditions)
                {
                    dumper.AddConditionResult(ebxInfo, condition, typeName, "RuntimeStates", "Conditions");
                }
            }
        }

        private static void DumpGalaxyCompletionData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var criticalPathFlags = (object[])data.CriticalPathFlags;
            if (criticalPathFlags != null)
            {
                foreach (dynamic criticalPathFlag in criticalPathFlags)
                {
                    var guid = (Guid)criticalPathFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "CriticalPathFlags");
                }
            }

            var enemyBaseFlags = (object[])data.EnemyBaseFlags;
            if (enemyBaseFlags != null)
            {
                foreach (dynamic enemyBaseFlag in enemyBaseFlags)
                {
                    var guid = (Guid)enemyBaseFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "EnemyBaseFlags");
                }
            }

            var loyaltyAndBStoryFlags = (object[])data.LoyaltyAndBStoryFlags;
            if (loyaltyAndBStoryFlags != null)
            {
                foreach (dynamic loyaltyAndBStoryFlag in loyaltyAndBStoryFlags)
                {
                    var guid = (Guid)loyaltyAndBStoryFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "LoyaltyAndBStoryFlags");
                }
            }

            var remnantVaultFlags = (object[])data.RemnantVaultFlags;
            if (remnantVaultFlags != null)
            {
                foreach (dynamic remnantVaultFlag in remnantVaultFlags)
                {
                    var guid = (Guid)remnantVaultFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "RemnantVaultFlags");
                }
            }

            var sideQuestFlags = (object[])data.SideQuestFlags;
            if (sideQuestFlags != null)
            {
                foreach (dynamic sideQuestFlag in sideQuestFlags)
                {
                    var guid = (Guid)sideQuestFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "SideQuestFlags");
                }
            }
        }

        private static void DumpActivePlayTimerEntityData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var guid = (Guid)data.TempestPilotingPlotFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, guid, typeName, "TempestPilotingPlotFlag");
        }

        private static void DumpLootManagerEntityData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dynamic lootConfiguration = data.LootConfiguration;
            if (lootConfiguration != null)
            {
                var guid = (Guid)lootConfiguration.DisableLootInteraction.PlotFlagId.Guid;
                dumper.AddGuidResult(ebxInfo, guid, typeName, "LootConfiguration", "DisableLootInteraction");
            }
        }

        private static void DumpPartyManagerManagerEntityData(EbxInfo ebxInfo,
                                                              Dumper dumper,
                                                              string typeName,
                                                              dynamic data)
        {
            var partyMemberBundles = (object[])data.PartyMemberBundles;
            if (partyMemberBundles != null)
            {
                foreach (dynamic partyMemberBundle in partyMemberBundles)
                {
                    var canLevelUpFlagGuid = (Guid)partyMemberBundle.CanLevelUpFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, canLevelUpFlagGuid, typeName, "CanLevelUpFlag");
                    var inPartyFlagGuid = (Guid)partyMemberBundle.InPartyFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, inPartyFlagGuid, typeName, "InPartyFlag");
                    var loyaltyMissionGuid = (Guid)partyMemberBundle.LoyaltyMission.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, loyaltyMissionGuid, typeName, "LoyaltyMission");
                    var trustLevelGuid = (Guid)partyMemberBundle.TrustLevel.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, trustLevelGuid, typeName, "TrustLevel");
                    var unlockedFlagGuid = (Guid)partyMemberBundle.UnlockedFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, unlockedFlagGuid, typeName, "UnlockedFlag");
                }
            }
        }

        private static void DumpViabilitySystemManagerEntityData(EbxInfo ebxInfo,
                                                                 Dumper dumper,
                                                                 string typeName,
                                                                 dynamic data)
        {
            var viabilityDatas = (object[])data.ViabilityData;
            if (viabilityDatas != null)
            {
                foreach (dynamic viabilityData in viabilityDatas)
                {
                    var guid = (Guid)viabilityData.PlotFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "ViabilityData", "PlotFlag");
                }
            }
        }

        private static void DumpItemCountTrackerAsset(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var itemCountTrackers = (object[])data.ItemCountTracker;
            if (itemCountTrackers != null)
            {
                foreach (dynamic itemCountTracker in itemCountTrackers)
                {
                    var guid = (Guid)itemCountTracker.ItemCountFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "ItemCountTracker", "ItemCountFlag");
                }
            }
        }

        private static void DumpSaveLevelContext(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var extraDatas = (object[])data.ExtraData;
            if (extraDatas != null)
            {
                foreach (dynamic extraData in extraDatas)
                {
                    var guid = (Guid)extraData.ShouldUse.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "ExtraData", "ShouldUse");
                }
            }
        }

        private static void DumpSystemData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var availableFlag = (Guid)data.AvailableFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, availableFlag, typeName, "AvailableFlag");
            var visitedFlag = (Guid)data.VisitedFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, visitedFlag, typeName, "VisitedFlag");
        }

        private static void DumpDestinationData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var anomalies = (object[])data.Anomalies;
            if (anomalies != null)
            {
                foreach (dynamic anomaly in anomalies)
                {
                    var guid = (Guid)anomaly.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "Anomalies");
                }
            }

            var currentDestinationFlag = (Guid)data.CurrentDestinationFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, currentDestinationFlag, typeName, "CurrentDestinationFlag");

            dumper.AddConditionResult(ebxInfo, data.ExcludeFromJournalTask, typeName, "ExcludeFromJournalTask");

            var hazards = (object[])data.Hazards;
            if (hazards != null && hazards.Length > 0)
            {
                throw new NotSupportedException();
            }

            dumper.AddConditionResult(ebxInfo, data.IncludeInCompletion, typeName, "IncludeInCompletion");

            var scannedFlag = (Guid)data.ScannedFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, scannedFlag, typeName, "ScannedFlag");

            var techs = (object[])data.Tech;
            if (techs != null && techs.Length > 0)
            {
                throw new NotSupportedException();
            }
        }

        private static void DumpPerksUIDataProviderData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var perkCategories = (object[])data.PerkCategories;
            if (perkCategories != null)
            {
                foreach (dynamic perkCategory in perkCategories)
                {
                    var perks = (object[])perkCategory.Perks;
                    if (perks != null)
                    {
                        foreach (dynamic perk in perks)
                        {
                            DumpPerk(ebxInfo, dumper, typeName, perk);
                        }
                    }
                }
            }
        }

        private static void DumpPerk(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var plotFlagAcquired = (Guid)data.PlotFlagAcquired.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, plotFlagAcquired, typeName, "PerkCategories", "Perks", "PlotFlagAcquired");

            var plotFlagRequirements = (object[])data.PlotFlagRequirement;
            if (plotFlagRequirements != null)
            {
                foreach (dynamic plotFlagRequirement in plotFlagRequirements)
                {
                    var plotFlagRequirementGuid = (Guid)plotFlagRequirement.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo,
                                         plotFlagRequirementGuid,
                                         typeName,
                                         "PerkCategories",
                                         "Perks",
                                         "PlotFlagRequirement");
                }
            }

            var visibilityFlag = (Guid)data.VisibilityFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, visibilityFlag, typeName, "PerkCategories", "Perks", "VisibilityFlag");
        }

        private static void DumpTutorialEntry(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var entrySeenFlag = (Guid)data.EntrySeenFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, entrySeenFlag, typeName, "EntrySeenFlag");
            var entryVisibleFlag = (Guid)data.EntryVisibleFlag.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, entryVisibleFlag, typeName, "EntryVisibleFlag");
        }

        private static void DumpPlotLocationMarkerComponentData(EbxInfo ebxInfo,
                                                                Dumper dumper,
                                                                string typeName,
                                                                dynamic data)
        {
            dumper.AddConditionResult(ebxInfo, data.DisplayCondition, typeName, "DisplayCondition");
            dumper.AddConditionResult(ebxInfo, data.HideCondition, typeName, "HideCondition");
        }

        private static void DumpConversationTypeSettings(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            Func<string> callback = () => data.DebugDisplayName;
            dumper.AddConditionResult(ebxInfo, data.EnabledCondition, callback, typeName, "EnabledCondition");
        }

        private static void DumpConversationCategory(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            dumper.AddActionResult(ebxInfo, data.OnCategorySelectedAction, typeName, "OnCategorySelectedAction");
        }

        private static void DumpSystemDataReference(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var neighboringSystemsInfos = (object[])data.NeighboringSystemsInfo;
            if (neighboringSystemsInfos != null)
            {
                foreach (dynamic neighboringSystemsInfo in neighboringSystemsInfos)
                {
                    DumpNeighboringSystemsInfo(ebxInfo, dumper, typeName, neighboringSystemsInfo);
                }
            }
        }

        private static void DumpNeighboringSystemsInfo(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var displayCondition = data.DisplayCondition;
            dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "NeighboringSystemsInfo", "DisplayCondition");
            var hideCondition = data.HideCondition;
            dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "NeighboringSystemsInfo", "HideCondition");
        }

        private static void DumpProgressiveTaskData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var currentProgress = (Guid)data.CurrentProgress.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, currentProgress, typeName, "CurrentProgress");

            var displayConditions = (object[])data.DisplayConditions;
            if (displayConditions != null)
            {
                foreach (var displayCondition in displayConditions)
                {
                    dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "DisplayConditions");
                }
            }

            var failureConditions = (object[])data.FailureConditions;
            if (failureConditions != null)
            {
                foreach (var failureCondition in failureConditions)
                {
                    dumper.AddConditionResult(ebxInfo, failureCondition, typeName, "FailureConditions");
                }
            }

            var hideConditions = (object[])data.HideConditions;
            if (hideConditions != null)
            {
                foreach (var hideCondition in hideConditions)
                {
                    dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "HideConditions");
                }
            }

            var successConditions = (object[])data.SuccessConditions;
            if (successConditions != null)
            {
                foreach (var successCondition in successConditions)
                {
                    dumper.AddConditionResult(ebxInfo, successCondition, typeName, "SuccessConditions");
                }
            }

            var total = (Guid)data.Total.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, total, typeName, "Total");
        }

        private static void DumpJournalContentData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var displayConditions = (object[])data.DisplayConditions;
            if (displayConditions != null)
            {
                foreach (var displayCondition in displayConditions)
                {
                    dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "DisplayConditions");
                }
            }

            var hideConditions = (object[])data.HideConditions;
            if (hideConditions != null)
            {
                foreach (var hideCondition in hideConditions)
                {
                    dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "HideConditions");
                }
            }
        }

        private static void DumpJournalTaskData(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var displayConditions = (object[])data.DisplayConditions;
            if (displayConditions != null)
            {
                foreach (var displayCondition in displayConditions)
                {
                    dumper.AddConditionResult(ebxInfo, displayCondition, typeName, "DisplayConditions");
                }
            }

            var failureConditions = (object[])data.FailureConditions;
            if (failureConditions != null)
            {
                foreach (var failureCondition in failureConditions)
                {
                    dumper.AddConditionResult(ebxInfo, failureCondition, typeName, "FailureConditions");
                }
            }

            var hideConditions = (object[])data.HideConditions;
            if (hideConditions != null)
            {
                foreach (var hideCondition in hideConditions)
                {
                    dumper.AddConditionResult(ebxInfo, hideCondition, typeName, "HideConditions");
                }
            }

            var successConditions = (object[])data.SuccessConditions;
            if (successConditions != null)
            {
                foreach (var successCondition in successConditions)
                {
                    dumper.AddConditionResult(ebxInfo, successCondition, typeName, "SuccessConditions");
                }
            }
        }

        private static void DumpPlotConfigurationAsset(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var newGamePlusPlotFlags = (object[])data.NewGamePlusPlotFlags;
            if (newGamePlusPlotFlags != null)
            {
                foreach (dynamic newGamePlusPlotFlag in newGamePlusPlotFlags)
                {
                    var guid = (Guid)newGamePlusPlotFlag.PlotFlagId.Guid;
                    dumper.AddGuidResult(ebxInfo, guid, typeName, "NewGamePlusPlotFlags");
                }
            }
        }

        private static void DumpGiveJournalPlotFlagReward(EbxInfo ebxInfo, Dumper dumper, string typeName, dynamic data)
        {
            var plotFlagValueDestination = (Guid)data.PlotFlagValueDestination.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, plotFlagValueDestination, typeName, "PlotFlagValueDestination");
            var plotFlagValueSource = (Guid)data.PlotFlagValueSource.PlotFlagId.Guid;
            dumper.AddGuidResult(ebxInfo, plotFlagValueSource, typeName, "PlotFlagValueSource");
        }
    }
}
