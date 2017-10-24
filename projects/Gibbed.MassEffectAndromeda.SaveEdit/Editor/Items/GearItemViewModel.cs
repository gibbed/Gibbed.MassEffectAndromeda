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
using Gibbed.MassEffectAndromeda.SaveFormats.Items;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Items
{
    internal class GearItemViewModel : ItemViewModel
    {
        #region Fields
        private readonly GearItemData _Item;
        #endregion

        public GearItemViewModel(uint id, GearItemData item)
            : base(id, item)
        {
            this._Item = item;
        }

        #region Properties
        public override string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(this.CustomName) == false
                           ? this.CustomName + " (" + base.DisplayName + ")"
                           : base.DisplayName;
            }
        }

        public bool Unknown4
        {
            get { return this._Item.Unknown4; }
            set
            {
                this._Item.Unknown4 = value;
                this.NotifyOfPropertyChange(() => this.Unknown4);
            }
        }

        public List<KeyValuePair<uint, ItemData>> Mods
        {
            get { return this._Item.Mods; }
        }

        public List<uint> AugmentationItemHashes
        {
            get { return this._Item.AugmentationItemHashes; }
        }

        public string CustomName
        {
            get { return this._Item.CustomName; }
            set
            {
                this._Item.CustomName = value;
                this.NotifyOfPropertyChange(() => this.CustomName);
                this.NotifyOfPropertyChange(() => this.DisplayName);
            }
        }
        #endregion
    }
}
