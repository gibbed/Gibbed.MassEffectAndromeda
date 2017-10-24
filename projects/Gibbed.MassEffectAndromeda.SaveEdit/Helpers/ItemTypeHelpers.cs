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

using ItemType = Gibbed.MassEffectAndromeda.GameInfo.ItemType;

namespace Gibbed.MassEffectAndromeda.SaveEdit
{
    internal static class ItemTypeHelpers
    {
        public static string GetDisplayName(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Invalid:
                {
                    return "Misc";
                }

                case ItemType.Power:
                {
                    return "Power";
                }

                case ItemType.RangedWeapon:
                {
                    return "Ranged Weapon";
                }

                case ItemType.MeleeWeapon:
                {
                    return "Melee Weapon";
                }

                case ItemType.WeaponPart:
                {
                    return "Weapon Part";
                }

                case ItemType.Gear:
                {
                    return "Gear";
                }

                case ItemType.GearPart:
                {
                    return "Gear Part";
                }

                case ItemType.SpaceTool:
                {
                    return "Space Tool";
                }

                case ItemType.Consumable:
                {
                    return "Consumable";
                }

                case ItemType.Research:
                {
                    return "Research";
                }

                case ItemType.Resource:
                {
                    return "Resource";
                }

                case ItemType.AugmentationMaterial:
                {
                    return "Augmentation";
                }

                case ItemType.MakoMod:
                {
                    return "Nomad Mod";
                }

                case ItemType.MakoSkin:
                {
                    return "Nomad Skin";
                }

                case ItemType.Plot:
                {
                    return "Plot";
                }

                case ItemType.CasualOutfit:
                {
                    return "Casual Outfit";
                }

                case ItemType.PassiveSkill:
                {
                    return "Passive Skill";
                }
            }

            return null;
        }

        public static int GetDisplayOrder(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Invalid:
                {
                    return (int)ItemTypeOrder.Invalid;
                }

                case ItemType.Power:
                {
                    return (int)ItemTypeOrder.Power;
                }

                case ItemType.RangedWeapon:
                {
                    return (int)ItemTypeOrder.RangedWeapon;
                }

                case ItemType.MeleeWeapon:
                {
                    return (int)ItemTypeOrder.MeleeWeapon;
                }

                case ItemType.WeaponPart:
                {
                    return (int)ItemTypeOrder.WeaponPart;
                }

                case ItemType.Gear:
                {
                    return (int)ItemTypeOrder.Gear;
                }

                case ItemType.GearPart:
                {
                    return (int)ItemTypeOrder.GearPart;
                }

                case ItemType.SpaceTool:
                {
                    return (int)ItemTypeOrder.SpaceTool;
                }

                case ItemType.Consumable:
                {
                    return (int)ItemTypeOrder.Consumable;
                }

                case ItemType.Research:
                {
                    return (int)ItemTypeOrder.Research;
                }

                case ItemType.Resource:
                {
                    return (int)ItemTypeOrder.Resource;
                }

                case ItemType.AugmentationMaterial:
                {
                    return (int)ItemTypeOrder.AugmentationMaterial;
                }

                case ItemType.MakoMod:
                {
                    return (int)ItemTypeOrder.MakoMod;
                }

                case ItemType.MakoSkin:
                {
                    return (int)ItemTypeOrder.MakoSkin;
                }

                case ItemType.Plot:
                {
                    return (int)ItemTypeOrder.Plot;
                }

                case ItemType.CasualOutfit:
                {
                    return (int)ItemTypeOrder.CasualOutfit;
                }

                case ItemType.PassiveSkill:
                {
                    return (int)ItemTypeOrder.PassiveSkill;
                }
            }

            return int.MaxValue;
        }
    }
}
