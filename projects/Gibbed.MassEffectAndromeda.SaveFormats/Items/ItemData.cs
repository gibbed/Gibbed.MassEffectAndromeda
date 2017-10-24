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
using System.ComponentModel;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Items
{
    public class ItemData : INotifyPropertyChanged, ICloneable
    {
        #region Fields
        private uint _Unknown1;
        private string _PartitionName;
        private GameInfo.ItemDefinition _Definition;

        private int _Quantity;
        private bool _IsNew;
        private int _Rarity;
        #endregion

        #region Properties
        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set
            {
                this._Unknown1 = value;
                this.NotifyPropertyChanged("Unknown1");
            }
        }

        [JsonProperty("partition_name")]
        public string PartitionName
        {
            get { return this._PartitionName; }
            set
            {
                this._PartitionName = value;
                this.NotifyPropertyChanged("PartitionName");
            }
        }

        public GameInfo.ItemDefinition Definition
        {
            get { return this._Definition; }
            internal set
            {
                this._Definition = value;
                this.NotifyPropertyChanged("Definition");
            }
        }

        [JsonProperty("quantity")]
        public int Quantity
        {
            get { return this._Quantity; }
            set
            {
                this._Quantity = value;
                this.NotifyPropertyChanged("Quantity");
            }
        }

        [JsonProperty("is_new")]
        public bool IsNew
        {
            get { return this._IsNew; }
            set
            {
                this._IsNew = value;
                this.NotifyPropertyChanged("IsNew");
            }
        }

        [JsonProperty("rarity")]
        public int Rarity
        {
            get { return this._Rarity; }
            set
            {
                this._Rarity = value;
                this.NotifyPropertyChanged("Rarity");
            }
        }
        #endregion

        public virtual void Read(IBitReader reader, ushort version)
        {
            this._Quantity = reader.ReadInt32();
            this._IsNew = reader.ReadBoolean();
            this._Rarity = reader.ReadInt32();
        }

        public virtual void Write(IBitWriter writer)
        {
            writer.WriteInt32(this._Quantity);
            writer.WriteBoolean(this._IsNew);
            writer.WriteInt32(this._Rarity);
        }

        public override string ToString()
        {
            return this._PartitionName ?? base.ToString();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public virtual object Clone()
        {
            return new ItemData()
            {
                Unknown1 = this.Unknown1,
                PartitionName = this.PartitionName,
                Definition = this.Definition,
                Quantity = this.Quantity,
                IsNew = this.IsNew,
                Rarity = this.Rarity,
            };
        }
    }
}
