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

using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Items
{
    public class RangedWeaponItemData : GearItemData
    {
        #region Fields
        private ushort _Unknown5;
        private ushort _Unknown6;
        private ushort _Unknown7;
        private uint _Unknown8;
        #endregion

        #region Properties
        [JsonProperty("unknown5")]
        public ushort Unknown5
        {
            get { return this._Unknown5; }
            set
            {
                this._Unknown5 = value;
                this.NotifyPropertyChanged("Unknown5");
            }
        }

        [JsonProperty("unknown6")]
        public ushort Unknown6
        {
            get { return this._Unknown6; }
            set
            {
                this._Unknown6 = value;
                this.NotifyPropertyChanged("Unknown6");
            }
        }

        [JsonProperty("unknown7")]
        public ushort Unknown7
        {
            get { return this._Unknown7; }
            set
            {
                this._Unknown7 = value;
                this.NotifyPropertyChanged("Unknown7");
            }
        }

        [JsonProperty("unknown8")]
        public uint Unknown8
        {
            get { return this._Unknown8; }
            set
            {
                this._Unknown8 = value;
                this.NotifyPropertyChanged("Unknown8");
            }
        }
        #endregion

        public override void Read(IBitReader reader, ushort version)
        {
            this._Unknown5 = reader.ReadUInt16();
            this._Unknown6 = reader.ReadUInt16();
            this._Unknown7 = reader.ReadUInt16();
            this._Unknown8 = version < 8 ? 0 : reader.ReadUInt32();
            base.Read(reader, version);
        }

        public override void Write(IBitWriter writer)
        {
            writer.WriteUInt16(this._Unknown5);
            writer.WriteUInt16(this._Unknown6);
            writer.WriteUInt16(this._Unknown7);
            writer.WriteUInt32(this._Unknown8);
            base.Write(writer);
        }

        public override object Clone()
        {
            var instance = new RangedWeaponItemData()
            {
                Unknown1 = this.Unknown1,
                PartitionName = this.PartitionName,
                Definition = this.Definition,
                Quantity = this.Quantity,
                IsNew = this.IsNew,
                Rarity = this.Rarity,
                Unknown4 = this.Unknown4,
                CustomName = this.CustomName,
                Unknown5 = this.Unknown5,
                Unknown6 = this.Unknown6,
                Unknown7 = this.Unknown7,
                Unknown8 = this.Unknown8,
            };
            foreach (var kv in this.Mods)
            {
                instance.Mods.Add(new KeyValuePair<uint, ItemData>(kv.Key, (ItemData)kv.Value.Clone()));
            }
            instance.AugmentationItemHashes.AddRange(this.AugmentationItemHashes);
            return instance;
        }
    }
}
