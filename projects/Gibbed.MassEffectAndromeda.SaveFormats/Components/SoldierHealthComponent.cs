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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerMESoldierHealthComponent
    public class SoldierHealthComponent
    {
        #region Fields
        private uint _Unknown1;
        private bool _Unknown2;
        private bool _Unknown3;
        private uint _Unknown4;
        private uint _Unknown5;
        private uint _Unknown6;
        private uint _Unknown7;
        private uint _Unknown8;
        private uint _Unknown9;
        private uint _Unknown10;
        private uint _Unknown11;
        private uint _Unknown12;
        #endregion

        public SoldierHealthComponent()
        {
        }

        #region Properties
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        public bool Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        public bool Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        public uint Unknown4
        {
            get { return this._Unknown4; }
            set { this._Unknown4 = value; }
        }

        public uint Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }

        public uint Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        public uint Unknown7
        {
            get { return this._Unknown7; }
            set { this._Unknown7 = value; }
        }

        public uint Unknown8
        {
            get { return this._Unknown8; }
            set { this._Unknown8 = value; }
        }

        public uint Unknown9
        {
            get { return this._Unknown9; }
            set { this._Unknown9 = value; }
        }

        public uint Unknown10
        {
            get { return this._Unknown10; }
            set { this._Unknown10 = value; }
        }

        public uint Unknown11
        {
            get { return this._Unknown11; }
            set { this._Unknown11 = value; }
        }

        public uint Unknown12
        {
            get { return this._Unknown12; }
            set { this._Unknown12 = value; }
        }
        #endregion

        public void Read(IBitReader reader, int version)
        {
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2 = reader.ReadBoolean();
            this._Unknown3 = reader.ReadBoolean();
            this._Unknown4 = reader.ReadUInt32();
            this._Unknown5 = reader.ReadUInt32();
            this._Unknown6 = reader.ReadUInt32();
            this._Unknown7 = reader.ReadUInt32();
            this._Unknown8 = reader.ReadUInt32();
            this._Unknown9 = reader.ReadUInt32();
            this._Unknown10 = reader.ReadUInt32();
            this._Unknown11 = reader.ReadUInt32();
            this._Unknown12 = reader.ReadUInt32();
        }
    }
}
