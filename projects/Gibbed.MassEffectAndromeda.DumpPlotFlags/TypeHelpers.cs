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

namespace Gibbed.MassEffectAndromeda.DumpPlotFlags
{
    internal static class TypeHelpers
    {
        public static bool IsKnown(string name)
        {
            return
                // get
                name == "BoolProvider_PlotCondition" ||
                name == "ConversationBoolProvider_Plot" ||
                name == "FloatProvider_PlotCondition" ||
                name == "ConversationEntityData" ||
                name == "ConversationLine" ||
                name == "ConditionLogicPrefabBlueprint" ||
                name == "GameVaultVarInt" ||
                name == "IntegerProvider_PlotStateValue" ||
                name == "PlotConditionNotificationEntityData" ||
                name == "PlotIntegerFlagNotificationEntityData" ||
                name == "PlotGetBooleanEntityData" ||
                name == "PlotGetIntegerEntityData" ||
                name == "PlotGetFloatEntityData" ||
                name == "PlotClearBooleanEntityData" ||
                name == "PlotSetBooleanEntityData" ||
                name == "PlotSetIntegerEntityData" ||
                name == "PlotSetFloatEntityData" ||
                name == "PlotConditionEntityData" ||
                name == "PlotTimelineTrackCondition" ||
                name == "PlotCompareIntegerEntityData" ||
                name == "PlotFlagListenerData" ||
                name == "AwardListenerEntityData" ||
                name == "ActivePlayTimerEntityData" ||
                name == "TelemetryManagerData" ||
                name == "LootManagerEntityData" ||
                name == "PartyManagerManagerEntityData" ||
                name == "ViabilitySystemManagerEntityData" ||
                name == "DebugBooleanPlotFlag" ||
                name == "DebugIntegerPlotFlag" ||
                name == "ItemCountTrackerAsset" ||
                name == "PlotSubLevelStreamingTriggerEntityData" ||
                name == "MESaveLevelContext" ||
                name == "ConversationConditionData" ||
                name == "PlotLogicConditionRule" ||
                name == "PlotLocationMarkerEntityData" ||
                name == "StorytellerRulePlotConditionData" ||
                name == "SystemData" ||
                name == "DestinationData" ||
                name == "PerksUIDataProviderData" ||
                name == "TutorialEntry" ||
                name == "PlotLocationMarkerComponentData" ||
                name == "ConversationTypeSettings" ||
                name == "ConversationCategory" ||
                name == "NeighboringSystemInfo" ||
                name == "ProgressiveTaskData" ||
                name == "JournalContentData" ||
                name == "JournalTaskData" ||
                name == "PlotConfigurationAsset" ||
                name == "GiveJournalPlotFlagReward" ||

                // misc
                name == "JournalEntry" ||
                name == "MECCodexEntry" ||
                name == "PlotLogicBooleanFlagRule" ||
                name == "PlotLogicIntFlagRule" ||
                name == "MECraftingResearchProjectItemData" ||
                name == "GalaxyCompletionData" ||

                // DebugCondition
                name == "MEWeaponList" ||
                name == "MECasualOutfitList" ||
                name == "MEConsumableItemList" ||
                name == "MEGearItemList" ||
                name == "MEPowerList" ||
                name == "MESpaceToolList" ||
                name == "MEMeleeWeaponList" ||

                // set
                name == "PlotActionEntityData" ||
                name == "PlotIncrementIntegerByValueEntityData" ||
                name == "PlotIncrementIntegerByPlotFlagValueEntityData" ||

                // ignore
                name == "PlotFlagsDefaultValues" ||
                name == "ContactSaveGameSettings" ||
                name == "WorldStateConfig" ||
                name == "SpawnBundleGroupRequestsEntity_PathfinderData" ||
                name == "ContactDurangoPresenceBackendData" ||
                name == "ResearchNexusLevelRequirement" ||
                name == "SkillProgression";
        }

        public static bool IsNested(string name)
        {
            return name == "PlotFlagReference" ||
                   name == "PlotConditionReference" ||
                   name == "PlotActionReference" ||
                   name == "PlotLogicFlagRule" ||
                   name == "PlotLogicNumericFlagRule" ||
                   name == "PlotFlagSettings" ||
                   name == "PlotIncrementIntegerBaseEntityData" ||
                   name == "JournalEntryBase" ||
                   name == "CodexEntry" ||
                   name == "MultiPlotAward" ||
                   name == "PartyMemberBundle" ||
                   name == "LootConfiguration" ||
                   name == "ViabilitySystemData" ||
                   name == "SubLevelStreamingTriggerState" ||
                   name == "ItemCountTracker" ||
                   name == "ExtraLevelData" ||
                   name == "Perk" ||
                   name == "PerkCategory";
        }
    }
}
