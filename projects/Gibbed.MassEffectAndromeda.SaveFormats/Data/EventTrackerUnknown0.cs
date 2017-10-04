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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class EventTrackerUnknown0
    {
        #region Fields
        private readonly List<KeyValuePair<uint, byte[]>> _Unknown1;
        private readonly List<KeyValuePair<uint, byte[]>> _Unknown2;
        #endregion

        public EventTrackerUnknown0()
        {
            this._Unknown1 = new List<KeyValuePair<uint, byte[]>>();
            this._Unknown2 = new List<KeyValuePair<uint, byte[]>>();
        }

        #region Properties
        public List<KeyValuePair<uint, byte[]>> Unknown1
        {
            get { return this._Unknown1; }
        }

        public List<KeyValuePair<uint, byte[]>> Unknown2
        {
            get { return this._Unknown2; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown1.Clear();
            var unknown1Count = reader.ReadUInt16();
            for (int i = 0; i < unknown1Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown1Key = reader.ReadUInt32();
                var unknown1Value = reader.ReadBytes(16);
                reader.PopFrameLength();
                this._Unknown1.Add(new KeyValuePair<uint, byte[]>(unknown1Key, unknown1Value));
            }
            this._Unknown2.Clear();
            var unknown2Count = reader.ReadUInt16();
            for (int i = 0; i < unknown2Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown2Key = reader.ReadUInt32();
                var unknown2Value = reader.ReadBytes(16);
                reader.PopFrameLength();
                this._Unknown2.Add(new KeyValuePair<uint, byte[]>(unknown2Key, unknown2Value));
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var kv in this._Unknown1)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(kv.Key);
                writer.WriteBytes(kv.Value, 0, 16);
                writer.PopFrameLength();
            }
            writer.WriteUInt16((ushort)this._Unknown2.Count);
            foreach (var kv in this._Unknown2)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(kv.Key);
                writer.WriteBytes(kv.Value, 0, 16);
                writer.PopFrameLength();
            }
            writer.PopFrameLength();
        }
    }
}
