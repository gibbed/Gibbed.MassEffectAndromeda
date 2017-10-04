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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class WorldMapUnknown0
    {
        private readonly byte[] _Unknown1;
        private readonly List<WorldMapUnknown1> _Unknown2;

        public WorldMapUnknown0()
        {
            this._Unknown1 = new byte[16];
            this._Unknown2 = new List<WorldMapUnknown1>();
        }

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            reader.ReadBytes(this._Unknown1);
            this._Unknown2.Clear();
            var unknown2Count = reader.ReadUInt16();
            for (int i = 0; i < unknown2Count; i++)
            {
                var unknown2 = new WorldMapUnknown1();
                unknown2.Read(reader);
                this._Unknown2.Add(unknown2);
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteBytes(this._Unknown1);
            writer.WriteUInt16((ushort)this._Unknown2.Count);
            foreach (var unknown2 in this._Unknown2)
            {
                unknown2.Write(writer);
            }
            writer.PopFrameLength();
        }
    }
}
