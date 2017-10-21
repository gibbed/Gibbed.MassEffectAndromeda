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
using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GalaxyDataUnknown0
    {
        #region Fields
        private readonly List<KeyValuePair<Guid, uint>> _Unknown1;
        private readonly List<KeyValuePair<Guid, uint>> _Unknown2;
        #endregion

        public GalaxyDataUnknown0()
        {
            this._Unknown1 = new List<KeyValuePair<Guid, uint>>();
            this._Unknown2 = new List<KeyValuePair<Guid, uint>>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public List<KeyValuePair<Guid, uint>> Unknown1
        {
            get { return this._Unknown1; }
        }

        [JsonProperty("unknown2")]
        public List<KeyValuePair<Guid, uint>> Unknown2
        {
            get { return this._Unknown2; }
        }
        #endregion

        internal void Read(IBitReader reader, ushort version)
        {
            reader.PushFrameLength(24);
            ReadList(reader, this._Unknown1, version);
            ReadList(reader, this._Unknown2, version);
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            WriteList(writer, this._Unknown1);
            WriteList(writer, this._Unknown2);
            writer.PopFrameLength();
        }

        private static void ReadList(IBitReader reader, List<KeyValuePair<Guid, uint>> list, ushort version)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            list.Clear();
            if (version >= 1)
            {
                var count = reader.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    var key = reader.ReadGuid();
                    var value = reader.ReadUInt32();
                    list.Add(new KeyValuePair<Guid, uint>(key, value));
                }
            }
        }

        private static void WriteList(IBitWriter writer, List<KeyValuePair<Guid, uint>> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            writer.WriteUInt32((uint)list.Count);
            foreach (var kv in list)
            {
                writer.WriteGuid(kv.Key);
                writer.WriteUInt32(kv.Value);
            }
        }
    }
}
