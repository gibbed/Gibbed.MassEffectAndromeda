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

using System.ComponentModel;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PartyMember : BaseSaveData, INotifyPropertyChanged
    {
        #region Fields
        private readonly bool _ExcludePresets;
        private readonly bool _ExcludeProfiles;
        private Components.UnknownComponent _Unknown;
        private Components.InventoryComponent _Inventory;
        private Components.EquipmentComponent _Equipment;
        private Components.ProgressionComponent _Progression;
        private Components.CraftingProgressionComponent _CraftingProgression;
        private Components.SoldierHealthComponent _SoldierHealth;
        #endregion

        public PartyMember(bool excludePresets, bool excludeProfiles)
        {
            this._ExcludePresets = excludePresets;
            this._ExcludeProfiles = excludeProfiles;
        }

        #region Properties
        public bool ExcludePresets
        {
            get { return this._ExcludePresets; }
        }

        public bool ExcludeProfiles
        {
            get { return this._ExcludeProfiles; }
        }

        [JsonProperty("unknown")]
        public Components.UnknownComponent Unknown
        {
            get { return this._Unknown; }
            set
            {
                this._Unknown = value;
                this.NotifyPropertyChanged("Unknown");
            }
        }

        [JsonProperty("inventory")]
        public Components.InventoryComponent Inventory
        {
            get { return this._Inventory; }
            set
            {
                this._Inventory = value;
                this.NotifyPropertyChanged("Inventory");
            }
        }

        [JsonProperty("equipment")]
        public Components.EquipmentComponent Equipment
        {
            get { return this._Equipment; }
            set
            {
                this._Equipment = value;
                this.NotifyPropertyChanged("Equipment");
            }
        }

        [JsonProperty("progression")]
        public Components.ProgressionComponent Progression
        {
            get { return this._Progression; }
            set
            {
                this._Progression = value;
                this.NotifyPropertyChanged("Progression");
            }
        }

        [JsonProperty("crafting_progression")]
        public Components.CraftingProgressionComponent CraftingProgression
        {
            get { return this._CraftingProgression; }
            set
            {
                this._CraftingProgression = value;
                this.NotifyPropertyChanged("CraftingProgression");
            }
        }

        [JsonProperty("soldier_health")]
        public Components.SoldierHealthComponent SoldierHealth
        {
            get { return this._SoldierHealth; }
            set
            {
                this._SoldierHealth = value;
                this.NotifyPropertyChanged("SoldierHealth");
            }
        }
        #endregion

        public override void Read(IBitReader reader)
        {
            base.Read(reader);

            var baseVersion = reader.ReadUInt32();
            if (baseVersion < 1 || baseVersion > 11)
            {
                throw new SaveFormatException("unsupported base version");
            }

            var version = baseVersion >= 11
                              ? reader.ReadUInt16()
                              : baseVersion >= 8 ? (ushort)9 : (ushort)4;
            if (version > 10)
            {
                throw new SaveFormatException("unsupported version");
            }

            var unknownLength = reader.PushFrameLength(24);
            if (unknownLength == 0)
            {
                this._Unknown = null;
            }
            else
            {
                this._Unknown = new Components.UnknownComponent();
                this._Unknown.Read(reader, baseVersion);
            }
            reader.PopFrameLength();

            var inventoryLength = reader.PushFrameLength(24);
            if (inventoryLength == 0)
            {
                this._Inventory = null;
            }
            else
            {
                this._Inventory = new Components.InventoryComponent();
                this._Inventory.Read(reader, version);
            }
            reader.PopFrameLength();

            var equipmentLength = reader.PushFrameLength(24);
            if (equipmentLength == 0)
            {
                this._Equipment = null;
            }
            else
            {
                this._Equipment = new Components.EquipmentComponent();
                this._Equipment.Read(reader, version);
            }
            reader.PopFrameLength();

            if (baseVersion >= 2)
            {
                var progressionLength = reader.PushFrameLength(24);
                if (progressionLength == 0)
                {
                    this._Progression = null;
                }
                else
                {
                    this._Progression = new Components.ProgressionComponent();
                    this._Progression.Read(reader, version, this._ExcludeProfiles, this._ExcludePresets);
                }
                reader.PopFrameLength();
            }

            if (baseVersion >= 6)
            {
                var craftingProgressionLength = reader.PushFrameLength(24);
                if (craftingProgressionLength == 0)
                {
                    this._CraftingProgression = null;
                }
                else
                {
                    this._CraftingProgression = new Components.CraftingProgressionComponent();
                    this._CraftingProgression.Read(reader, version);
                }
                reader.PopFrameLength();
            }

            if (baseVersion >= 4)
            {
                var solderHealthLength = reader.PushFrameLength(24);
                if (solderHealthLength == 0)
                {
                    this._SoldierHealth = null;
                }
                else
                {
                    this._SoldierHealth = new Components.SoldierHealthComponent();
                    this._SoldierHealth.Read(reader, version);
                }
                reader.PopFrameLength();
            }

            if (baseVersion <= 4)
            {
                reader.SkipBits(2); // two booleans
            }
        }

        public override void Write(IBitWriter writer)
        {
            base.Write(writer);

            writer.WriteUInt32(11); // base version
            writer.WriteUInt16(10); // version

            writer.PushFrameLength(24);
            if (this._Unknown != null)
            {
                this._Unknown.Write(writer);
            }
            writer.PopFrameLength();

            writer.PushFrameLength(24);
            if (this._Inventory != null)
            {
                this._Inventory.Write(writer);
            }
            writer.PopFrameLength();

            writer.PushFrameLength(24);
            if (this._Equipment != null)
            {
                this._Equipment.Write(writer);
            }
            writer.PopFrameLength();

            writer.PushFrameLength(24);
            if (this._Progression != null)
            {
                this._Progression.Write(writer, this._ExcludeProfiles, this._ExcludePresets);
            }
            writer.PopFrameLength();

            writer.PushFrameLength(24);
            if (this._CraftingProgression != null)
            {
                this._CraftingProgression.Write(writer);
            }
            writer.PopFrameLength();

            writer.PushFrameLength(24);
            if (this._SoldierHealth != null)
            {
                this._SoldierHealth.Write(writer);
            }
            writer.PopFrameLength();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
