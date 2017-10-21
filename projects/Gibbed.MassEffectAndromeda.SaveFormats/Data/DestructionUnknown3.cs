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
    public class DestructionUnknown3
    {
        #region Fields
        private readonly List<DestructionUnknown4> _Unknown1; 
        private uint _Unknown2;
        #endregion

        public DestructionUnknown3()
        {
            this._Unknown1 = new List<DestructionUnknown4>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public List<DestructionUnknown4> Unknown1
        {
            get { return this._Unknown1; }
        }

        [JsonProperty("unknown2")]
        public uint Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            reader.PushFramePosition(26);
            this._Unknown1.Clear();
            var unknown1Count = reader.ReadUInt16();
            for (int i = 0; i < unknown1Count; i++)
            {
                var unknown1 = new DestructionUnknown4();
                unknown1.Read(reader);
                this._Unknown1.Add(unknown1);
            }
            reader.PopFramePosition();
            reader.PushFrameLength(24);
            this._Unknown2 = reader.ReadUInt32();
            reader.PopFrameLength();
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.PushFramePosition(26);
            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var unknown1 in this._Unknown1)
            {
                unknown1.Write(writer);
            }
            writer.PopFramePosition();
            writer.PushFrameLength(24);
            writer.WriteUInt32(this._Unknown2);
            writer.PopFrameLength();
            writer.PopFrameLength();
        }
    }
}
