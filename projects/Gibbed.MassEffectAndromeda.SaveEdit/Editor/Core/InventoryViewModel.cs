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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gibbed.MassEffectAndromeda.SaveFormats.Components;
using Gibbed.MassEffectAndromeda.SaveFormats.Items;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Core
{
    [Export(typeof(InventoryViewModel))]
    internal class InventoryViewModel : PropertyChangedBase
    {
        #region Imports
        private ShellViewModel _Shell;
        private SaveLoad _SaveLoad;

        [Import(typeof(ShellViewModel))]
        public ShellViewModel Shell
        {
            get { return this._Shell; }
            set
            {
                this._Shell = value;
                this.NotifyOfPropertyChange(() => this.Shell);
            }
        }

        [Import(typeof(SaveLoad))]
        public SaveLoad SaveLoad
        {
            get { return this._SaveLoad; }
            set
            {
                this._SaveLoad = value;
                this.NotifyOfPropertyChange(() => this.SaveLoad);
            }
        }
        #endregion

        #region Fields
        private readonly ObservableCollection<Items.ItemViewModel> _Items;
        private readonly List<InventoryComponent.RawItemData> _RawItems;
        private Items.ItemViewModel _SelectedItem;
        private uint _NextItemId;
        #endregion

        #region Properties
        public ObservableCollection<Items.ItemViewModel> Items
        {
            get { return this._Items; }
        }

        public List<InventoryComponent.RawItemData> RawItems
        {
            get { return this._RawItems; }
        }

        public Items.ItemViewModel SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                this._SelectedItem = value;
                this.NotifyOfPropertyChange(() => this.SelectedItem);
            }
        }

        public uint NextItemId
        {
            get { return this._NextItemId; }
            set
            {
                this._NextItemId = value;
                this.NotifyOfPropertyChange(() => this.NextItemId);
            }
        }
        #endregion

        [ImportingConstructor]
        public InventoryViewModel()
        {
            this._Items = new ObservableCollection<Items.ItemViewModel>();
            this._RawItems = new List<InventoryComponent.RawItemData>();
        }

        public void DuplicateSelectedItem()
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            var item = (ItemData)this.SelectedItem.Data.Clone();
            var id = this.NextItemId++;

            Items.ItemViewModel viewModel;
            if (item.GetType() == typeof(ItemData))
            {
                viewModel = new Items.ItemViewModel(id, item);
            }
            else if (item.GetType() == typeof(GearItemData))
            {
                viewModel = new Items.GearItemViewModel(id, (GearItemData)item);
            }
            else if (item.GetType() == typeof(RangedWeaponItemData))
            {
                viewModel = new Items.RangedWeaponItemViewModel(id, (RangedWeaponItemData)item);
            }
            else if (item.GetType() == typeof(PowerItemData))
            {
                viewModel = new Items.PowerItemViewModel(id, (PowerItemData)item);
            }
            else
            {
                throw new NotSupportedException();
            }

            this.Items.Add(viewModel);
            this.SelectedItem = viewModel;
        }

        public void DeleteSelectedItem()
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            this.Items.Remove(this.SelectedItem);
            this.SelectedItem = this.Items.FirstOrDefault();
        }
    }
}
