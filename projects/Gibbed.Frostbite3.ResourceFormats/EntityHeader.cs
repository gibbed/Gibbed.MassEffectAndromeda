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
using System.IO;
using Gibbed.IO;

namespace Gibbed.Frostbite3.ResourceFormats
{
    public struct EntityHeader
    {
        public const uint Signature = 0x0FB2D1CE;

        public uint HeaderSize;
        public uint DataSize;
        public uint Unknown0C;
        public ushort Unknown10;
        public ushort Unknown12;
        public ushort Unknown14;
        public ushort Unknown16;
        public ushort Unknown18;
        public ushort StringTableSize;
        public uint Unknown1C;
        public uint Unknown20;
        public uint Unknown24;
        public Guid Unknown28;

        public static EntityHeader Read(Stream input)
        {
            const Endian endian = Endian.Little;

            var basePosition = input.Position;

            var magic = input.ReadValueU32(endian);
            if (magic != Signature)
            {
                throw new FormatException();
            }

            var headerSize = input.ReadValueU32(endian);
            if (basePosition + headerSize > input.Length)
            {
                throw new EndOfStreamException();
            }

            var dataSize = input.ReadValueU32(endian);
            if (basePosition + headerSize + dataSize > input.Length)
            {
                throw new EndOfStreamException();
            }

            EntityHeader instance;
            instance.HeaderSize = headerSize;
            instance.DataSize = dataSize;
            instance.Unknown0C = input.ReadValueU32(endian);
            instance.Unknown10 = input.ReadValueU16(endian);
            instance.Unknown12 = input.ReadValueU16(endian);
            instance.Unknown14 = input.ReadValueU16(endian);
            instance.Unknown16 = input.ReadValueU16(endian);
            instance.Unknown18 = input.ReadValueU16(endian);
            instance.StringTableSize = input.ReadValueU16(endian);
            instance.Unknown1C = input.ReadValueU32(endian);
            instance.Unknown20 = input.ReadValueU32(endian);
            instance.Unknown24 = input.ReadValueU32(endian);
            instance.Unknown28 = input.ReadValueGuid(endian);

            // += Unknown0C * 32
            // += StringTableSize
            // += Unknown18 * 16

            return instance;
        }
    }
}
