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
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DestructionUnknown0
    {
        #region Fields
        private byte _Unknown1;
        private readonly List<byte> _Unknown2;
        private string _Unknown3;
        private readonly List<DestructionUnknown1> _Unknown4;
        private uint _Unknown5;
        #endregion

        public DestructionUnknown0()
        {
            this._Unknown2 = new List<byte>();
            this._Unknown4 = new List<DestructionUnknown1>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public byte Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public List<byte> Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("unknown3")]
        public string Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        [JsonProperty("unknown4")]
        public List<DestructionUnknown1> Unknown4
        {
            get { return this._Unknown4; }
        }

        [JsonProperty("unknown5")]
        public uint Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown1 = reader.ReadUInt8();
            this._Unknown2.Clear();
            var unknown2Count = reader.ReadUInt16();
            for (int i = 0; i < unknown2Count; i++)
            {
                this._Unknown2.Add(reader.ReadUInt8());
            }
            this._Unknown3 = reader.ReadString();
            this._Unknown4.Clear();
            var unknown4Count = reader.ReadUInt32();
            for (uint i = 0; i < unknown4Count; i++)
            {
                var unknown4 = new DestructionUnknown1();
                unknown4.Read(reader);
                this._Unknown4.Add(unknown4);
            }
            this._Unknown5 = reader.ReadUInt32();
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt8(this._Unknown1);
            writer.WriteUInt16((ushort)this._Unknown2.Count);
            foreach (var unknown2 in this._Unknown2)
            {
                writer.WriteUInt8(unknown2);
            }
            writer.WriteString(this._Unknown3);
            writer.WriteUInt32((uint)this._Unknown4.Count);
            foreach (var unknown4 in this._Unknown4)
            {
                unknown4.Write(writer);
            }
            writer.WriteUInt32(this._Unknown5);
            writer.PopFrameLength();
        }
    }
}
