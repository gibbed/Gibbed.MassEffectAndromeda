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
using System.Linq;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Plot
    {
        #region Fields
        private readonly Dictionary<Guid, bool> _Bools;
        private readonly Dictionary<Guid, int> _Ints;
        private readonly Dictionary<Guid, float> _Floats;
        private readonly Dictionary<Guid, Transform> _Transforms;
        private readonly Dictionary<uint, Guid> _Unknown;
        #endregion

        public Plot()
        {
            this._Bools = new Dictionary<Guid, bool>();
            this._Ints = new Dictionary<Guid, int>();
            this._Floats = new Dictionary<Guid, float>();
            this._Transforms = new Dictionary<Guid, Transform>();
            this._Unknown = new Dictionary<uint, Guid>();
        }

        #region Properties
        [JsonProperty("bools")]
        public Dictionary<Guid, bool> Bools
        {
            get { return this._Bools; }
        }

        [JsonProperty("ints")]
        public Dictionary<Guid, int> Ints
        {
            get { return this._Ints; }
        }

        [JsonProperty("floats")]
        public Dictionary<Guid, float> Floats
        {
            get { return this._Floats; }
        }

        [JsonProperty("transforms")]
        public Dictionary<Guid, Transform> Transforms
        {
            get { return this._Transforms; }
        }

        [JsonProperty("unknown")]
        public Dictionary<uint, Guid> Unknown
        {
            get { return this._Unknown; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._Bools.Clear();
            this._Ints.Clear();
            this._Floats.Clear();
            this._Transforms.Clear();
            this._Unknown.Clear();
            reader.PushFrameLength(24);
            var version = reader.ReadUInt16();
            if (version >= 2)
            {
                var marker = reader.ReadString();
                if (marker != "BioMetrics_PlotStartMarker")
                {
                    throw new SaveFormatException();
                }
            }
            if (version >= 1)
            {
                var falseCount = reader.ReadUInt32(version >= 4 ? 31 : 16);
                for (int i = 0; i < falseCount; i++)
                {
                    var key = reader.ReadGuid();
                    this._Bools.Add(key, false);
                }
                var trueCount = reader.ReadUInt32(version >= 4 ? 31 : 16);
                for (int i = 0; i < trueCount; i++)
                {
                    var key = reader.ReadGuid();
                    this._Bools.Add(key, true);
                }
                var intCount = reader.ReadUInt16();
                for (int i = 0; i < intCount; i++)
                {
                    var key = reader.ReadGuid();
                    var value = reader.ReadInt32();
                    this._Ints.Add(key, value);
                }
                var floatCount = reader.ReadUInt16();
                for (int i = 0; i < floatCount; i++)
                {
                    var key = reader.ReadGuid();
                    var value = reader.ReadFloat32();
                    this._Floats.Add(key, value);
                }
                var transformCount = reader.ReadUInt16();
                for (int i = 0; i < transformCount; i++)
                {
                    var key = reader.ReadGuid();
                    var value = reader.ReadTransform();
                    this._Transforms.Add(key, value);
                }
            }
            if (version >= 3)
            {
                var unknownCount = reader.ReadUInt16();
                for (int i = 0; i < unknownCount; i++)
                {
                    reader.PushFrameLength(24);
                    var key = reader.ReadUInt32();
                    var value = reader.ReadGuid();
                    reader.PopFrameLength();
                    this._Unknown.Add(key, value);
                }
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            var comparer = new MemoryGuidComparer();
            writer.PushFrameLength(24);
            writer.WriteUInt16(4);
            writer.WriteString("BioMetrics_PlotStartMarker");
            var falseKeys = this._Bools.Where(kv => kv.Value == false).Select(kv => kv.Key).ToArray();
            writer.WriteUInt32((uint)falseKeys.Length, 31);
            foreach (var key in falseKeys.OrderBy(v => v, comparer))
            {
                writer.WriteGuid(key);
            }
            var trueKeys = this._Bools.Where(kv => kv.Value == true).Select(kv => kv.Key).ToArray();
            writer.WriteUInt32((uint)trueKeys.Length, 31);
            foreach (var key in trueKeys.OrderBy(v => v, comparer))
            {
                writer.WriteGuid(key);
            }
            writer.WriteUInt16((ushort)this._Ints.Count);
            foreach (var kv in this._Ints.OrderBy(kv => kv.Key, comparer))
            {
                writer.WriteGuid(kv.Key);
                writer.WriteInt32(kv.Value);
            }
            writer.WriteUInt16((ushort)this._Floats.Count);
            foreach (var kv in this._Floats.OrderBy(kv => kv.Key, comparer))
            {
                writer.WriteGuid(kv.Key);
                writer.WriteFloat32(kv.Value);
            }
            writer.WriteUInt16((ushort)this._Transforms.Count);
            foreach (var kv in this._Transforms.OrderBy(kv => kv.Key, comparer))
            {
                writer.WriteGuid(kv.Key);
                writer.WriteTransform(kv.Value);
            }
            writer.WriteUInt16((ushort)this._Unknown.Count);
            foreach (var kv in this._Unknown)
            {
                writer.WriteUInt32(kv.Key);
                writer.WriteGuid(kv.Value);
            }
            writer.PopFrameLength();
        }

        private class MemoryGuidComparer : IComparer<Guid>
        {
            public int Compare(Guid x, Guid y)
            {
                var xb = x.ToByteArray();
                var yb = y.ToByteArray();
                for (int i = 0; i < 16; i++)
                {
                    if (xb[i] < yb[i])
                    {
                        return -1;
                    }
                    if (xb[i] > yb[i])
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }
    }
}
