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
using Caliburn.Micro;
using Gibbed.MassEffectAndromeda.FileFormats;
using Gibbed.MassEffectAndromeda.SaveFormats.Components;
using Gibbed.MassEffectAndromeda.SaveFormats.Data;
using Gibbed.MassEffectAndromeda.SaveFormats.Items;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Squad
{
    internal class MemberViewModel : PropertyChangedBase
    {
        #region Fields
        private readonly Core.InventoryViewModel _Inventory;
        private readonly int _Index;
        private readonly GameInfo.PartyMemberDefinition _Definition;
        private PartyMemberSnapshot _Snapshot;
        private PartyMember _Data;
        #endregion

        public MemberViewModel(Core.InventoryViewModel inventory, int index, GameInfo.PartyMemberDefinition definition)
        {
            this._Inventory = inventory;
            this._Index = index;
            this._Definition = definition;
        }

        #region Properties
        public int Index
        {
            get { return this._Index; }
        }

        public GameInfo.PartyMemberDefinition Definition
        {
            get { return this._Definition; }
        }

        public PartyMemberSnapshot Snapshot
        {
            get { return this._Snapshot; }
            set
            {
                this._Snapshot = value;
                this.NotifyOfPropertyChange(() => this.Snapshot);
            }
        }

        public PartyMember Data
        {
            get { return this._Data; }
            set
            {
                this._Data = value;
                this.NotifyOfPropertyChange(() => this.Data);
            }
        }
        #endregion

        public void ImportData(PartyMemberSnapshot snapshot, PartyMember data)
        {
            this._Snapshot = snapshot;
            this._Data = data;

            if (this._Inventory != null)
            {
                if (data.Inventory != null)
                {
                    foreach (var rawItem in data.Inventory.Items)
                    {
                        if (rawItem.DataBytes == null || rawItem.DataLength == 0)
                        {
                            this._Inventory.RawItems.Add(rawItem);
                            continue;
                        }

                        var bitReader = new BitReader(rawItem.DataBytes, rawItem.DataLength);
                        ItemData item;
                        try
                        {
                            item = InventoryComponent.ReadItemData(bitReader, data.Inventory.ReadVersion);
                        }
                        catch (Exception e)
                        {
                            this._Inventory.RawItems.Add(rawItem);
                            continue;
                        }

                        Items.ItemViewModel viewModel;
                        if (item.GetType() == typeof(ItemData))
                        {
                            viewModel = new Items.ItemViewModel(rawItem.Id, item);
                        }
                        else if (item.GetType() == typeof(GearItemData))
                        {
                            viewModel = new Items.GearItemViewModel(rawItem.Id, (GearItemData)item);
                        }
                        else if (item.GetType() == typeof(RangedWeaponItemData))
                        {
                            viewModel = new Items.RangedWeaponItemViewModel(rawItem.Id, (RangedWeaponItemData)item);
                        }
                        else if (item.GetType() == typeof(PowerItemData))
                        {
                            viewModel = new Items.PowerItemViewModel(rawItem.Id, (PowerItemData)item);
                        }
                        else
                        {
                            this._Inventory.RawItems.Add(rawItem);
                            continue;
                        }
                        this._Inventory.Items.Add(viewModel);
                    }
                }
            }
        }

        public void ExportData(out PartyMemberSnapshot snapshot, out PartyMember data)
        {
            snapshot = this._Snapshot;
            data = this._Data;

            if (this._Inventory != null)
            {
                if (data.Inventory == null)
                {
                    data.Inventory = new InventoryComponent();
                }
                else
                {
                    data.Inventory.Items.Clear();
                }

                data.Inventory.Items.AddRange(this._Inventory.RawItems);

                foreach (var viewModel in this._Inventory.Items)
                {
                    var bitWriter = new BitWriter(0x1000);
                    InventoryComponent.WriteItemData(bitWriter, viewModel.Data);
                    var rawItem = new InventoryComponent.RawItemData();
                    rawItem.Id = viewModel.Id;
                    rawItem.DataBytes = bitWriter.GetBytes();
                    rawItem.DataLength = bitWriter.Position;
                    data.Inventory.Items.Add(rawItem);
                }
            }
        }
    }
}
