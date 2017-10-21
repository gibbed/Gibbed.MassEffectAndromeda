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
    public class CodexListUnknown0
    {
        #region Fields
        private uint _Unknown1;
        private readonly List<CodexListUnknown1> _Unknown2;
        #endregion

        public CodexListUnknown0()
        {
            this._Unknown2 = new List<CodexListUnknown1>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public List<CodexListUnknown1> Unknown2
        {
            get { return this._Unknown2; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2.Clear();
            if (this._Unknown1 >= 1)
            {
                var unknown2Count = reader.ReadUInt16();
                for (int i = 0; i < unknown2Count; i++)
                {
                    var unknown2 = new CodexListUnknown1();
                    unknown2.Read(reader);
                    this._Unknown2.Add(unknown2);
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt32(this._Unknown1);
            if (this._Unknown1 >= 1)
            {
                writer.WriteUInt16((ushort)this._Unknown2.Count);
                foreach (var unknown2 in this._Unknown2)
                {
                    unknown2.Write(writer);
                }
            }
        }
    }
}
