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
    public class DestructionUnknown5
    {
        #region Fields
        private uint _Unknown1;
        private bool _Unknown2;
        private uint _Unknown3;
        private byte[] _Unknown4;
        #endregion

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

        public uint Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        public byte[] Unknown4
        {
            get { return this._Unknown4; }
            set { this._Unknown4 = value; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2 = reader.ReadBoolean();
            this._Unknown3 = reader.ReadUInt32();
            // TODO(gibbed): figure out what this actually
            this._Unknown4 = reader.ReadBytes(Math.Max(1, (int)((this._Unknown3 + 31) >> 5)) * 4);
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt32(this._Unknown1);
            writer.WriteBoolean(this._Unknown2);
            writer.WriteUInt32(this._Unknown3);
            writer.WriteBytes(this._Unknown4);
            writer.PopFrameLength();
        }
    }
}
