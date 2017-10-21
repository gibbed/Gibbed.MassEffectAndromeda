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
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.CustomizedParameters
{
    [JsonObject(MemberSerialization.OptIn)]
    [CustomizedParameter(_ComponentName)]
    public class PackedShaderSetParameter : CustomizedParameter
    {
        private const string _ComponentName = "PackedShaderSetParamDesc";

        public override string ComponentName
        {
            get { return _ComponentName; }
        }

        #region Fields
        private uint _LayoutSignature;
        private byte[] _PackedData;
        #endregion

        #region Properties
        [JsonProperty("layout_signature")]
        public uint LayoutSignature
        {
            get { return this._LayoutSignature; }
            set { this._LayoutSignature = value; }
        }

        [JsonProperty("packed_data")]
        public byte[] PackedData
        {
            get { return this._PackedData; }
            set { this._PackedData = value; }
        }
        #endregion

        public override void Read(IBitReader reader, ushort version)
        {
            base.Read(reader, version);
            this._LayoutSignature = reader.ReadUInt32();
            var packedDataLength = reader.ReadUInt32();
            this._PackedData = reader.ReadBytes((int)packedDataLength);
        }

        public override void Write(IBitWriter writer, ushort version)
        {
            base.Write(writer, version);
            writer.WriteUInt32(this._LayoutSignature);
            writer.WriteUInt32((uint)this._PackedData.Length);
            writer.WriteBytes(this._PackedData);
        }
    }
}
