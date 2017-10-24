using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemType = Gibbed.MassEffectAndromeda.GameInfo.ItemType;

namespace Gibbed.MassEffectAndromeda.SaveEdit
{
    internal static class ItemTypeDisplayNames
    {
        public static string Get(ItemType type)
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
                    return "Augmentation Material";
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
    }
}
