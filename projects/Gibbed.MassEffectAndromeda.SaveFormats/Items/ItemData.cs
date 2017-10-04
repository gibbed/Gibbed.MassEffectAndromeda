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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Items
{
    public class ItemData
    {
        #region Fields
        private uint _Unknown0;
        private string _PartitionName;

        private int _Quantity;
        private bool _Unknown2;
        private int _Unknown3;
        #endregion

        #region Properties
        public uint Unknown0
        {
            get { return this._Unknown0; }
            set { this._Unknown0 = value; }
        }

        public string PartitionName
        {
            get { return this._PartitionName; }
            set { this._PartitionName = value; }
        }

        public int Quantity
        {
            get { return this._Quantity; }
            set { this._Quantity = value; }
        }

        public bool Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        public int Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }
        #endregion

        public virtual void Read(IBitReader reader, int version)
        {
            this._Quantity = reader.ReadInt32();
            this._Unknown2 = reader.ReadBoolean();
            this._Unknown3 = reader.ReadInt32();
        }

        public override string ToString()
        {
            return this._PartitionName ?? base.ToString();
        }
    }
}
