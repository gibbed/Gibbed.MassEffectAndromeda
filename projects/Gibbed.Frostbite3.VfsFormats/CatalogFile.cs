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
using System.IO;
using Gibbed.IO;
using Gibbed.Frostbite3.Common;

namespace Gibbed.Frostbite3.VfsFormats
{
    public class CatalogFile
    {
        public const ulong Signature = 0x6E61794E6E61794E; // 'NyanNyan'

        private readonly List<ChunkEntry> _ChunkEntries;
        private readonly List<Unknown3Entry> _Unknown3Entries; 

        public CatalogFile()
        {
            this._ChunkEntries = new List<ChunkEntry>();
            this._Unknown3Entries = new List<Unknown3Entry>();
        }

        public List<ChunkEntry> ChunkEntries
        {
            get { return this._ChunkEntries; }
        }

        public List<Unknown3Entry> Unknown3s
        {
            get { return this._Unknown3Entries; }
        }

        public static CatalogFile Read(string path)
        {
            using (var input = File.OpenRead(path))
            {
                return FileLayerHelper.ReadObject(input, Read);
            }
        }

        public static CatalogFile Read(Stream input)
        {
            const Endian endian = Endian.Little;

            var magic1 = input.ReadValueU64(endian);
            var magic2 = input.ReadValueU64(endian);
            if (magic1 != Signature || magic2 != Signature)
            {
                throw new FormatException();
            }

            var instance = new CatalogFile();

            var chunkCount = input.ReadValueU32(endian);
            var unknown2Count = input.ReadValueU32(endian);
            var unknown3Count = input.ReadValueU32(endian);
            var unknown1C = input.ReadValueU32(endian);
            var unknown20 = input.ReadValueU32(endian);
            var unknown24 = input.ReadValueU32(endian);

            if (unknown2Count != 0 || unknown1C != 0 || unknown20 != 0 || unknown24 != 0)
            {
                throw new FormatException();
            }

            instance.ChunkEntries.Clear();

            for (int i = 0; i < chunkCount; i++)
            {
                var entry = new ChunkEntry();
                entry.SHA1 = new SHA1(input.ReadBytes(20));
                entry.Offset = input.ReadValueU32(endian);
                entry.Size = input.ReadValueU32(endian);
                entry.TailSize = input.ReadValueU32(endian);
                entry.DataIndex = input.ReadValueU32(endian);
                instance.ChunkEntries.Add(entry);
            }

            for (int i = 0; i < unknown3Count; i++)
            {
                var entry = new Unknown3Entry();
                entry.Unknown0 = new SHA1(input.ReadBytes(20));
                entry.Unknown1 = input.ReadValueU32(endian);
                entry.Unknown2 = input.ReadValueU32(endian);
                entry.Unknown3 = input.ReadValueU32(endian);
                entry.Unknown4 = input.ReadValueU32(endian);
                entry.Unknown5 = input.ReadValueU32(endian);
                entry.Unknown6 = new SHA1(input.ReadBytes(20));
                entry.Unknown7 = new SHA1(input.ReadBytes(20));
                instance.Unknown3s.Add(entry);
            }

            if (input.Position != input.Length)
            {
                throw new FormatException();
            }

            return instance;
        }

        public class ChunkEntry
        {
            // ReSharper disable InconsistentNaming
            public SHA1 SHA1;
            // ReSharper restore InconsistentNaming
            public uint Offset;
            public uint Size;
            public uint TailSize;
            public uint DataIndex;

            public override string ToString()
            {
                return this.SHA1.ToString();
            }
        }

        public class Unknown3Entry
        {
            public SHA1 Unknown0;
            public uint Unknown1;
            public uint Unknown2;
            public uint Unknown3;
            public uint Unknown4;
            public uint Unknown5;
            public SHA1 Unknown6;
            public SHA1 Unknown7;
        }
    }
}
