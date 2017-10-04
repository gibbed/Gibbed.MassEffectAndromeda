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
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class PartyMemberUnknown : BaseSaveData
    {
        private Components.InventoryComponent _Inventory;
        private Components.EquipmentComponent _Equipment;
        private Components.ProgressionComponent _Progression;
        private Components.CraftingProgressionComponent _CraftingProgression;
        private Components.SoldierHealthComponent _SoldierHealth;
        private int _CharacterIndex;

        public PartyMemberUnknown(int characterIndex)
        {
            this._CharacterIndex = characterIndex;
        }

        internal override void Read(IBitReader reader)
        {
            base.Read(reader);

            var baseVersion = reader.ReadUInt32();
            if (baseVersion < 1)
            {
                throw new FormatException();
            }

            var version = baseVersion >= 11
                              ? reader.ReadUInt16()
                              : baseVersion >= 8 ? 9 : 4;

            var unknown3 = reader.PushFrameLength(24);
            if (unknown3 != 0)
            {
                var unknown4 = reader.ReadBytes(12);
                var unknown5 = reader.ReadBytes(12);
                var unknown6 = reader.ReadBytes(12);
                var unknown7 = reader.ReadBytes(12);
                var unknown8 = reader.ReadBoolean();
                if (unknown8 == true)
                {
                    var unknown9 = reader.ReadUInt32();
                    var unknown10 = reader.ReadUInt32();
                }
                if (baseVersion > 9)
                {
                    var vehicleCount = reader.ReadUInt32();
                    for (uint i = 0; i < vehicleCount; i++)
                    {
                        var vehicleData = new PartyMemberVehicleData();
                        vehicleData.Read(reader, version);
                    }
                }
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
                    this._Progression.Read(reader, version, this._CharacterIndex);
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

        internal override void Write(IBitWriter writer)
        {
            base.Write(writer);
            throw new NotImplementedException();
        }
    }
}
