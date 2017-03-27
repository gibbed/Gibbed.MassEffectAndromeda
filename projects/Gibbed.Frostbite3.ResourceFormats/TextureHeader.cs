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
using System.Text;
using Gibbed.IO;

namespace Gibbed.Frostbite3.ResourceFormats
{
    public struct TextureHeader
    {
        public uint Unknown00;
        public uint Unknown04;
        public TextureType Type;
        public TextureFormat Format;
        public uint Unknown10;
        public ushort Unknown14;
        public ushort Width;
        public ushort Height;
        public ushort Depth;
        public ushort Unknown1C; // array size?
        public byte MipCount;
        public byte FirstMipIndex;
        public Guid DataChunkId;
        public uint[] MipSizes;
        public uint TotalSize;
        public byte[] Unknown70;
        public uint Unknown80;
        public string Unknown84;

        public static TextureHeader Read(Stream input)
        {
            const Endian endian = Endian.Little;

            TextureHeader instance;
            instance.Unknown00 = input.ReadValueU32(endian);
            instance.Unknown04 = input.ReadValueU32(endian);
            instance.Type = (TextureType)input.ReadValueU32(endian);
            instance.Format = (TextureFormat)input.ReadValueU32(endian);
            instance.Unknown10 = input.ReadValueU32(endian);
            instance.Unknown14 = input.ReadValueU16(endian);
            instance.Width = input.ReadValueU16(endian);
            instance.Height = input.ReadValueU16(endian);
            instance.Depth = input.ReadValueU16(endian);
            instance.Unknown1C = input.ReadValueU16(endian);
            instance.MipCount = input.ReadValueU8();
            instance.FirstMipIndex = input.ReadValueU8();
            instance.DataChunkId = input.ReadValueGuid(Endian.Big);
            instance.MipSizes = new uint[15];
            for (int i = 0; i < 15; i++)
            {
                instance.MipSizes[i] = input.ReadValueU32(endian);
            }
            instance.TotalSize = input.ReadValueU32(endian);
            instance.Unknown70 = input.ReadBytes(16);
            instance.Unknown80 = input.ReadValueU32(endian);
            instance.Unknown84 = input.ReadString(16, true, Encoding.UTF8);
            return instance;
        }
    }
}
