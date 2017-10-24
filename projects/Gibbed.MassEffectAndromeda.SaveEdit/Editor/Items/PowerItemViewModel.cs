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

using Gibbed.MassEffectAndromeda.SaveFormats.Items;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Items
{
    internal class PowerItemViewModel : GearItemViewModel
    {
        #region Fields
        private readonly PowerItemData _Item;
        #endregion

        public PowerItemViewModel(uint id, PowerItemData item)
            : base(id, item)
        {
            this._Item = item;
        }

        #region Properties
        public uint Unknown5
        {
            get { return this._Item.Unknown5; }
            set
            {
                this._Item.Unknown5 = value;
                this.NotifyOfPropertyChange(() => this.Unknown5);
            }
        }

        public uint Unknown6
        {
            get { return this._Item.Unknown6; }
            set
            {
                this._Item.Unknown5 = value;
                this.NotifyOfPropertyChange(() => this.Unknown6);
            }
        }

        public bool Unknown7
        {
            get { return this._Item.Unknown7; }
            set
            {
                this._Item.Unknown7 = value;
                this.NotifyOfPropertyChange(() => this.Unknown7);
            }
        }

        public uint Unknown8
        {
            get { return this._Item.Unknown8; }
            set
            {
                this._Item.Unknown8 = value;
                this.NotifyOfPropertyChange(() => this.Unknown8);
            }
        }
        #endregion
    }
}
