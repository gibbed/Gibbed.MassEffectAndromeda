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
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.CustomizedParameters
{
    [JsonObject(MemberSerialization.OptIn)]
    [CustomizedParameter(_ComponentName)]
    public class CustomizedHairParameter : CustomizedParameter
    {
        private const string _ComponentName = "CustomizedHairDesc";

        public override string ComponentName
        {
            get { return _ComponentName; }
        }

        #region Fields
        private Guid _Unknown;
        private uint _HeadId;
        private uint _HairstyleIndex;
        #endregion

        #region Properties
        [JsonProperty("unknown")]
        public Guid Unknown
        {
            get { return this._Unknown; }
            set { this._Unknown = value; }
        }

        [JsonProperty("head_id")]
        public uint HeadId
        {
            get { return this._HeadId; }
            set { this._HeadId = value; }
        }

        [JsonProperty("hairstyle_index")]
        public uint HairstyleIndex
        {
            get { return this._HairstyleIndex; }
            set { this._HairstyleIndex = value; }
        }
        #endregion

        public override void Read(IBitReader reader, ushort version)
        {
            base.Read(reader, version);
            this._Unknown = reader.ReadGuid();
            this._HeadId = reader.ReadUInt32();
            this._HairstyleIndex = reader.ReadUInt32();
        }

        public override void Write(IBitWriter writer, ushort version)
        {
            base.Write(writer, version);
            writer.WriteGuid(this._Unknown);
            writer.WriteUInt32(this._HeadId);
            writer.WriteUInt32(this._HairstyleIndex);
        }
    }
}
