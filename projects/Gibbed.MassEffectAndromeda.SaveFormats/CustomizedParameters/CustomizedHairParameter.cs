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

namespace Gibbed.MassEffectAndromeda.SaveFormats.CustomizedParameters
{
    [CustomizedParameter(_ComponentName)]
    public class CustomizedHairParameter : CustomizedParameter
    {
        private const string _ComponentName = "CustomizedHairDesc";

        public override string ComponentName
        {
            get { return _ComponentName; }
        }

        #region Fields
        private readonly byte[] _Unknown1;
        private uint _Unknown2;
        private uint _Unknown3;
        #endregion

        public CustomizedHairParameter()
        {
            this._Unknown1 = new byte[16];
        }

        #region Properties
        public byte[] Unknown1
        {
            get { return this._Unknown1; }
        }

        public uint Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        public uint Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }
        #endregion

        public override void Read(IBitReader reader, ushort version)
        {
            base.Read(reader, version);
            reader.ReadBytes(this._Unknown1);
            this._Unknown2 = reader.ReadUInt32();
            this._Unknown3 = reader.ReadUInt32();
        }

        public override void Write(IBitWriter writer, ushort version)
        {
            base.Write(writer, version);
            writer.WriteBytes(this._Unknown1);
            writer.WriteUInt32(this._Unknown2);
            writer.WriteUInt32(this._Unknown3);
        }
    }
}
