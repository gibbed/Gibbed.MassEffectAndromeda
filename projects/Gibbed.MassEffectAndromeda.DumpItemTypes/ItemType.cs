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

using Gibbed.Frostbite3.Dynamic;

namespace Gibbed.MassEffectAndromeda.DumpItemTypes
{
    [PartitionEnum("ItemType", PartitionEnumOptions.Validate)]
    internal enum ItemType : uint
    {
        [PartitionEnumMember("ItemType_Power")]
        Power = 0,

        [PartitionEnumMember("ItemType_RangedWeapon")]
        RangedWeapon = 1,

        [PartitionEnumMember("ItemType_MeleeWeapon")]
        MeleeWeapon = 2,

        [PartitionEnumMember("ItemType_WeaponPart")]
        WeaponPart = 3,

        [PartitionEnumMember("ItemType_Gear")]
        Gear = 4,

        [PartitionEnumMember("ItemType_GearPart")]
        GearPart = 5,

        [PartitionEnumMember("ItemType_SpaceTool")]
        SpaceTool = 6,

        [PartitionEnumMember("ItemType_Consumable")]
        Consumable = 7,

        [PartitionEnumMember("ItemType_Research")]
        Research = 8,

        [PartitionEnumMember("ItemType_Resource")]
        Resource = 9,

        [PartitionEnumMember("ItemType_AugmentationMaterial")]
        AugmentationMaterial = 10,

        [PartitionEnumMember("ItemType_MakoMod")]
        MakoMod = 11,

        [PartitionEnumMember("ItemType_MakoSkin")]
        MakoSkin = 12,

        [PartitionEnumMember("ItemType_Plot")]
        Plot = 13,

        [PartitionEnumMember("ItemType_CasualOutfit")]
        CasualOutfit = 14,

        [PartitionEnumMember("ItemType_PassiveSkill")]
        PassiveSkill = 15,

        [PartitionEnumMember("ItemType_Count")]
        Count = 16,

        [PartitionEnumMember("ItemType_Invalid")]
        Invalid = 17,
    }
}
