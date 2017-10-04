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
    public class PartyUnknown3
    {
        private uint _Unknown1;
        private Transform _Unknown2;
        private bool _Unknown3;
        private Guid _Unknown4;
        private Guid _Unknown5;
        private Guid _Unknown6;

        internal void Read(IBitReader reader, uint version)
        {
            reader.PushFrameLength(20);
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2 = reader.ReadTransform();
            if (version >= 5)
            {
                this._Unknown3 = reader.ReadBoolean();
                if (this._Unknown3 == true)
                {
                    this._Unknown4 = reader.ReadGuid();
                    this._Unknown5 = reader.ReadGuid();
                    this._Unknown6 = reader.ReadGuid();
                }
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer, uint version)
        {
            writer.PushFrameLength(20);
            writer.WriteUInt32(this._Unknown1);
            writer.WriteTransform(this._Unknown2);
            if (version >= 5)
            {
                writer.WriteBoolean(this._Unknown3);
                if (this._Unknown3 == true)
                {
                    writer.WriteGuid(this._Unknown4);
                    writer.WriteGuid(this._Unknown5);
                    writer.WriteGuid(this._Unknown6);
                }
            }
            writer.PopFrameLength();
        }
    }
}
