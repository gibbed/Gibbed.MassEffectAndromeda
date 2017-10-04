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
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class NotificationsUnknown2
    {
        private uint _Unknown1;
        private uint _Unknown2;
        private readonly NotificationsUnknown5 _Unknown3;
        private readonly NotificationsUnknown5 _Unknown4;
        private uint _Unknown5;
        private uint _Unknown6;
        private uint _Unknown7;

        public NotificationsUnknown2()
        {
            this._Unknown3 = new NotificationsUnknown5();
            this._Unknown4 = new NotificationsUnknown5();
        }

        internal void Read(IBitReader reader, ushort version)
        {
            reader.PushFrameLength(24);
            this._Unknown1 = reader.ReadUInt32();
            this._Unknown2 = reader.ReadUInt32();
            reader.PushFrameLength(24);
            this._Unknown3.Read(reader, version);
            reader.PopFrameLength();
            reader.PushFrameLength(24);
            this._Unknown4.Read(reader, version);
            reader.PopFrameLength();
            this._Unknown5 = reader.ReadUInt32();
            this._Unknown6 = reader.ReadUInt32();
            this._Unknown7 = reader.ReadUInt32();
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer, ushort version)
        {
            throw new NotImplementedException();
        }
    }
}
