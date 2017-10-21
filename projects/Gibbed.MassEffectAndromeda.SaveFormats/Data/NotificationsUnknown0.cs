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
    public class NotificationsUnknown0
    {
        #region Fields
        private readonly List<NotificationsUnknown1> _Unknown1;
        private readonly List<NotificationsUnknown2> _Unknown2;
        private readonly List<NotificationsUnknown3> _Unknown3;
        private readonly List<NotificationsUnknown4> _Unknown4;
        #endregion

        public NotificationsUnknown0()
        {
            this._Unknown1 = new List<NotificationsUnknown1>();
            this._Unknown2 = new List<NotificationsUnknown2>();
            this._Unknown3 = new List<NotificationsUnknown3>();
            this._Unknown4 = new List<NotificationsUnknown4>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public List<NotificationsUnknown1> Unknown1
        {
            get { return this._Unknown1; }
        }

        [JsonProperty("unknown2")]
        public List<NotificationsUnknown2> Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("unknown3")]
        public List<NotificationsUnknown3> Unknown3
        {
            get { return this._Unknown3; }
        }

        [JsonProperty("unknown4")]
        public List<NotificationsUnknown4> Unknown4
        {
            get { return this._Unknown4; }
        }
        #endregion

        internal void Read(IBitReader reader, ushort version)
        {
            this._Unknown1.Clear();
            this._Unknown2.Clear();
            this._Unknown3.Clear();
            this._Unknown4.Clear();
            if (version >= 1)
            {
                var unknown1Count = reader.ReadUInt16();
                for (int i = 0; i < unknown1Count; i++)
                {
                    var unknown1 = new NotificationsUnknown1();
                    unknown1.Read(reader, version);
                    this._Unknown1.Add(unknown1);
                }

                var unknown2Count = reader.ReadUInt16();
                for (int i = 0; i < unknown2Count; i++)
                {
                    var unknown2 = new NotificationsUnknown2();
                    unknown2.Read(reader, version);
                    this._Unknown2.Add(unknown2);
                }

                var unknown3Count = reader.ReadUInt16();
                for (int i = 0; i < unknown3Count; i++)
                {
                    var unknown3 = new NotificationsUnknown3();
                    unknown3.Read(reader, version);
                    this._Unknown3.Add(unknown3);
                }

                var unknown4Count = reader.ReadUInt16();
                for (int i = 0; i < unknown4Count; i++)
                {
                    var unknown4 = new NotificationsUnknown4();
                    unknown4.Read(reader, version);
                    this._Unknown4.Add(unknown4);
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var unknown1 in this._Unknown1)
            {
                unknown1.Write(writer);
            }
            writer.WriteUInt16((ushort)this._Unknown2.Count);
            foreach (var unknown2 in this._Unknown2)
            {
                unknown2.Write(writer);
            }
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
        }
    }
}
