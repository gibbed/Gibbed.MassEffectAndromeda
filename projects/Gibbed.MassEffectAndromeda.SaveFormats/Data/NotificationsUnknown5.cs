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
    public class NotificationsUnknown5
    {
        internal void Read(IBitReader reader, ushort version)
        {
            if (version < 2)
            {
                var unknown0 = reader.ReadUInt32();
                for (int i = 0; i < unknown0; i++)
                {
                    var unknown1 = reader.ReadUInt32();
                }
                for (int i = 0; i < unknown0; i++)
                {
                    var unknown2 = reader.ReadUInt8();
                }
            }
            else if (version < 3)
            {
                var unknown3 = reader.ReadUInt32();
                for (int i = 0; i < unknown3; i++)
                {
                    var unknown4 = reader.ReadUInt32();
                    if (unknown4 == 2)
                    {
                        var unknown5 = reader.ReadString();
                    }
                    else
                    {
                        var unknown6 = reader.ReadUInt32();
                    }
                }
            }
            else
            {
                var unknown7 = reader.ReadUInt16();
                for (int i = 0; i < unknown7; i++)
                {
                    reader.PushFrameLength(24);

                    var unknown10 = reader.ReadUInt32();
                    if (unknown10 == 2)
                    {
                        var unknown11 = reader.ReadString();
                    }
                    else
                    {
                        var unknown12 = reader.ReadUInt32();
                    }

                    reader.PopFrameLength();
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
