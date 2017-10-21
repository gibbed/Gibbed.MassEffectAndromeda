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

using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerMEEquipmentComponent
    [JsonObject(MemberSerialization.OptIn)]
    public class EquipmentComponent
    {
        #region Fields
        private uint _Unknown1;
        private bool _Unknown2;
        private readonly uint[] _PowerIds;
        private readonly uint[] _RangedWeaponIds;
        private uint _MeleeWeaponId;
        private readonly uint[] _GearIds;
        private readonly uint[] _SpaceToolIds;
        private readonly uint[] _ConsumableIds;
        private uint _CasualOutfitId;
        #endregion

        public EquipmentComponent()
        {
            this._PowerIds = new uint[3];
            this._RangedWeaponIds = new uint[6];
            this._GearIds = new uint[4];
            this._SpaceToolIds = new uint[3];
            this._ConsumableIds = new uint[4];
        }

        #region Properties
        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public bool Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("power_ids")]
        public uint[] PowerIds
        {
            get { return this._PowerIds; }
        }

        [JsonProperty("ranged_weapon_ids")]
        public uint[] RangedWeaponIds
        {
            get { return this._RangedWeaponIds; }
        }

        [JsonProperty("melee_weapon_id")]
        public uint MeleeWeaponId
        {
            get { return this._MeleeWeaponId; }
            set { this._MeleeWeaponId = value; }
        }

        [JsonProperty("gear_ids")]
        public uint[] GearIds
        {
            get { return this._GearIds; }
        }

        [JsonProperty("space_tool_ids")]
        public uint[] SpaceToolIds
        {
            get { return this._SpaceToolIds; }
        }

        [JsonProperty("consumable_ids")]
        public uint[] ConsumableIds
        {
            get { return this._ConsumableIds; }
        }

        [JsonProperty("casual_outfit_id")]
        public uint CasualOutfitId
        {
            get { return this._CasualOutfitId; }
            set { this._CasualOutfitId = value; }
        }
        #endregion

        public void Read(IBitReader reader, int version)
        {
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2 = version >= 10 && reader.ReadBoolean();

            for (int i = 0; i < this._PowerIds.Length; i++)
            {
                this._PowerIds[i] = reader.ReadUInt32();
            }

            for (int i = 0; i < this._RangedWeaponIds.Length; i++)
            {
                this._RangedWeaponIds[i] = reader.ReadUInt32();
            }

            this._MeleeWeaponId = reader.ReadUInt32();

            for (int i = 0; i < this._GearIds.Length; i++)
            {
                this._GearIds[i] = reader.ReadUInt32();
            }

            for (int i = 0; i < this._SpaceToolIds.Length; i++)
            {
                this._SpaceToolIds[i] = reader.ReadUInt32();
            }

            for (int i = 0; i < this._ConsumableIds.Length; i++)
            {
                this._ConsumableIds[i] = reader.ReadUInt32();
            }

            this._CasualOutfitId = reader.ReadUInt32();
        }

        public void Write(IBitWriter writer)
        {
            writer.WriteUInt32(this._Unknown1);
            writer.WriteBoolean(this._Unknown2);

            for (int i = 0; i < this._PowerIds.Length; i++)
            {
                writer.WriteUInt32(this._PowerIds[i]);
            }

            for (int i = 0; i < this._RangedWeaponIds.Length; i++)
            {
                writer.WriteUInt32(this._RangedWeaponIds[i]);
            }

            writer.WriteUInt32(this._MeleeWeaponId);

            for (int i = 0; i < this._GearIds.Length; i++)
            {
                writer.WriteUInt32(this._GearIds[i]);
            }

            for (int i = 0; i < this._SpaceToolIds.Length; i++)
            {
                writer.WriteUInt32(this._SpaceToolIds[i]);
            }

            for (int i = 0; i < this._ConsumableIds.Length; i++)
            {
                writer.WriteUInt32(this._ConsumableIds[i]);
            }

            writer.WriteUInt32(this._CasualOutfitId);
        }
    }
}
