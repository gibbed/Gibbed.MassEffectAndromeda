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
    public class EventTrackerUnknown1
    {
        #region Fields
        private float _Unknown1;
        private uint _Unknown2;
        private readonly List<EventTrackerUnknown2> _Unknown3;
        private readonly List<EventTrackerUnknown2> _Unknown4;
        #endregion

        public EventTrackerUnknown1()
        {
            this._Unknown3 = new List<EventTrackerUnknown2>();
            this._Unknown4 = new List<EventTrackerUnknown2>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public float Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public uint Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("unknown3")]
        public List<EventTrackerUnknown2> Unknown3
        {
            get { return this._Unknown3; }
        }

        [JsonProperty("unknown4")]
        public List<EventTrackerUnknown2> Unknown4
        {
            get { return this._Unknown4; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown1 = reader.ReadFloat32();
            this._Unknown2 = reader.ReadUInt32();
            this._Unknown3.Clear();
            var unknown3Count = reader.ReadUInt16();
            for (int i = 0; i < unknown3Count; i++)
            {
                var unknown3 = new EventTrackerUnknown2();
                unknown3.Read(reader);
                this._Unknown3.Add(unknown3);
            }
            this._Unknown4.Clear();
            var unknown4Count = reader.ReadUInt16();
            for (int i = 0; i < unknown4Count; i++)
            {
                var unknown4 = new EventTrackerUnknown2();
                unknown4.Read(reader);
                this._Unknown4.Add(unknown4);
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteFloat32(this._Unknown1);
            writer.WriteUInt32(this._Unknown2);
            writer.WriteUInt16((ushort)this._Unknown3.Count);
            foreach (var unknown3 in this._Unknown3)
            {
                unknown3.Write(writer);
            }
            writer.WriteUInt16((ushort)this._Unknown4.Count);
            foreach (var unknown4 in this._Unknown4)
            {
                unknown4.Write(writer);
            }
            writer.PopFrameLength();
        }
    }
}
