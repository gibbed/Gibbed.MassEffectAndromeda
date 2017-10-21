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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerMESoldierHealthComponent
    [JsonObject(MemberSerialization.OptIn)]
    public class SoldierHealthComponent
    {
        #region Fields
        private float _Health;
        private bool _Unknown2;
        private bool _Unknown3;
        private float _Shields;
        private uint _Unknown5;
        private uint _Unknown6;
        private uint _Unknown7;
        private float _Unknown8;
        private uint _Unknown9;
        private uint _Unknown10;
        private uint _Unknown11;
        private float _Unknown12;
        #endregion

        #region Properties
        [JsonProperty("health")]
        public float Health
        {
            get { return this._Health; }
            set { this._Health = value; }
        }

        [JsonProperty("unknown2")]
        public bool Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("unknown3")]
        public bool Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        [JsonProperty("shields")]
        public float Shields
        {
            get { return this._Shields; }
            set { this._Shields = value; }
        }

        [JsonProperty("unknown5")]
        public uint Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }

        [JsonProperty("unknown6")]
        public uint Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        [JsonProperty("unknown7")]
        public uint Unknown7
        {
            get { return this._Unknown7; }
            set { this._Unknown7 = value; }
        }

        [JsonProperty("unknown8")]
        public float Unknown8
        {
            get { return this._Unknown8; }
            set { this._Unknown8 = value; }
        }

        [JsonProperty("unknown9")]
        public uint Unknown9
        {
            get { return this._Unknown9; }
            set { this._Unknown9 = value; }
        }

        [JsonProperty("unknown10")]
        public uint Unknown10
        {
            get { return this._Unknown10; }
            set { this._Unknown10 = value; }
        }

        [JsonProperty("unknown11")]
        public uint Unknown11
        {
            get { return this._Unknown11; }
            set { this._Unknown11 = value; }
        }

        [JsonProperty("unknown12")]
        public float Unknown12
        {
            get { return this._Unknown12; }
            set { this._Unknown12 = value; }
        }
        #endregion

        public void Read(IBitReader reader, int version)
        {
            this._Health = reader.ReadFloat32();
            this._Unknown2 = reader.ReadBoolean();
            this._Unknown3 = reader.ReadBoolean();
            this._Shields = reader.ReadFloat32();
            this._Unknown5 = reader.ReadUInt32();
            this._Unknown6 = reader.ReadUInt32();
            this._Unknown7 = reader.ReadUInt32();
            this._Unknown8 = reader.ReadFloat32();
            this._Unknown9 = reader.ReadUInt32();
            this._Unknown10 = reader.ReadUInt32();
            this._Unknown11 = reader.ReadUInt32();
            this._Unknown12 = reader.ReadFloat32();
        }

        public void Write(IBitWriter writer)
        {
            writer.WriteFloat32(this._Health);
            writer.WriteBoolean(this._Unknown2);
            writer.WriteBoolean(this._Unknown3);
            writer.WriteFloat32(this._Shields);
            writer.WriteUInt32(this._Unknown5);
            writer.WriteUInt32(this._Unknown6);
            writer.WriteUInt32(this._Unknown7);
            writer.WriteFloat32(this._Unknown8);
            writer.WriteUInt32(this._Unknown9);
            writer.WriteUInt32(this._Unknown10);
            writer.WriteUInt32(this._Unknown11);
            writer.WriteFloat32(this._Unknown12);
        }
    }
}
