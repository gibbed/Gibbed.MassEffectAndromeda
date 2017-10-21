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
    public class JournalUnknown0
    {
        #region Fields
        private ushort _Unknown1;
        private readonly List<uint> _Unknown2;
        private readonly List<uint> _Unknown3;
        private readonly List<uint> _Unknown4;
        #endregion

        public JournalUnknown0()
        {
            this._Unknown2 = new List<uint>();
            this._Unknown3 = new List<uint>();
            this._Unknown4 = new List<uint>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public ushort Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public List<uint> Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("unknown3")]
        public List<uint> Unknown3
        {
            get { return this._Unknown3; }
        }

        [JsonProperty("unknown4")]
        public List<uint> Unknown4
        {
            get { return this._Unknown4; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._Unknown1 = reader.ReadUInt16();
            this._Unknown2.Clear();
            this._Unknown3.Clear();
            this._Unknown4.Clear();
            if (this._Unknown1 >= 1)
            {
                var unknown2Count = reader.ReadUInt16();
                for (int i = 0; i < unknown2Count; i++)
                {
                    this._Unknown2.Add(reader.ReadUInt32());
                }
                var unknown3Count = reader.ReadUInt16();
                for (int i = 0; i < unknown3Count; i++)
                {
                    this._Unknown3.Add(reader.ReadUInt32());
                }
            }
            if (this._Unknown1 >= 2)
            {
                var unknown4Count = reader.ReadUInt16();
                for (int i = 0; i < unknown4Count; i++)
                {
                    this._Unknown4.Add(reader.ReadUInt32());
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt16(this._Unknown1);
            if (this._Unknown1 >= 1)
            {
                writer.WriteUInt16((ushort)this._Unknown2.Count);
                foreach (var unknown2 in this._Unknown2)
                {
                    writer.WriteUInt32(unknown2);
                }
                writer.WriteUInt16((ushort)this._Unknown3.Count);
                foreach (var unknown3 in this._Unknown3)
                {
                    writer.WriteUInt32(unknown3);
                }
            }
            if (this._Unknown1 >= 2)
            {
                writer.WriteUInt16((ushort)this._Unknown4.Count);
                foreach (var unknown4 in this._Unknown4)
                {
                    writer.WriteUInt32(unknown4);
                }
            }
        }
    }
}
