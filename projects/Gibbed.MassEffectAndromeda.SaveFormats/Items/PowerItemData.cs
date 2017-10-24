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
    public class PowerItemData : GearItemData
    {
        #region Fields
        private uint _Unknown5;
        private uint _Unknown6;
        private bool _Unknown7;
        private uint _Unknown8;
        #endregion

        #region Properties
        [JsonProperty("unknown5")]
        public uint Unknown5
        {
            get { return this._Unknown5; }
            set
            {
                this._Unknown5 = value;
                this.NotifyPropertyChanged("Unknown5");
            }
        }

        [JsonProperty("unknown6")]
        public uint Unknown6
        {
            get { return this._Unknown6; }
            set
            {
                this._Unknown6 = value;
                this.NotifyPropertyChanged("Unknown6");
            }
        }

        [JsonProperty("unknown7")]
        public bool Unknown7
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
            if (version >= 1)
            {
                reader.PushFrameLength(24);
                this._Unknown5 = reader.ReadUInt32();
                this._Unknown6 = reader.ReadUInt32();
                this._Unknown7 = reader.ReadBoolean();
                this._Unknown8 = reader.ReadUInt32();
                reader.PopFrameLength();
            }
            base.Read(reader, version);
        }

        public override void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt32(this._Unknown5);
            writer.WriteUInt32(this._Unknown6);
            writer.WriteBoolean(this._Unknown7);
            writer.WriteUInt32(this._Unknown8);
            writer.PopFrameLength();
            base.Write(writer);
        }

        public override object Clone()
        {
            var instance = new PowerItemData()
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
