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
using System.Linq;
using System.Text;
using Gibbed.Frostbite3.Common;
using Gibbed.IO;

namespace Gibbed.Frostbite3.VfsFormats
{
    public class CatalogFile
    {
        public const ulong Signature = 0x6E61794E6E61794E; // 'NyanNyan'

        private readonly List<ChunkEntry> _ChunkEntries;
        private readonly List<EncryptedChunkEntry> _EncryptedChunkEntries;

        public CatalogFile()
        {
            this._ChunkEntries = new List<ChunkEntry>();
            this._EncryptedChunkEntries = new List<EncryptedChunkEntry>();
        }

        public List<ChunkEntry> ChunkEntries
        {
            get { return this._ChunkEntries; }
        }

        public List<EncryptedChunkEntry> EncryptedChunkEntries
        {
            get { return this._EncryptedChunkEntries; }
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
            var patchCount = input.ReadValueU32(endian);
            var encryptedChunkCount = input.ReadValueU32(endian);
            var unknown1C = input.ReadValueS32(endian);
            var unknown20 = input.ReadValueS32(endian);
            var unknown24 = input.ReadValueS32(endian);

            if (patchCount != 0 || unknown1C != 0 || unknown20 != 0 || unknown24 != 0)
            {
                throw new FormatException();
            }

            instance.ChunkEntries.Clear();

            for (int i = 0; i < chunkCount; i++)
            {
                var chunk = ChunkEntry.Read(input, endian);

                if (chunk.IsEncrypted == true)
                {
                    throw new FormatException();
                }

                instance.ChunkEntries.Add(chunk);
            }

            for (int i = 0; i < encryptedChunkCount; i++)
            {
                var encryptedChunk = EncryptedChunkEntry.Read(input, endian);

                if (encryptedChunk.Chunk.IsEncrypted == false)
                {
                    throw new FormatException();
                }

                if (encryptedChunk.Chunk.Size != encryptedChunk.CryptoInfo.Size)
                {
                    throw new FormatException();
                }

                instance.EncryptedChunkEntries.Add(encryptedChunk);
            }

            if (input.Position != input.Length)
            {
                throw new FormatException();
            }

            return instance;
        }

        public struct ChunkEntry
        {
            // ReSharper disable InconsistentNaming
            public SHA1 SHA1;
            // ReSharper restore InconsistentNaming
            public uint Offset;
            public uint Size;
            public uint TailSize;
            public byte DataIndex;
            public bool IsEncrypted;

            public static ChunkEntry Read(Stream input, Endian endian)
            {
                var instance = new ChunkEntry();
                instance.SHA1 = new SHA1(input.ReadBytes(20));
                instance.Offset = input.ReadValueU32(endian);
                instance.Size = input.ReadValueU32(endian);
                instance.TailSize = input.ReadValueU32(endian);
                instance.DataIndex = input.ReadValueU8();
                instance.IsEncrypted = input.ReadValueB8();
                var padding = input.ReadBytes(2);
                if (padding.Any(b => b != 0) == true)
                {
                    throw new FormatException();
                }
                return instance;
            }

            public override string ToString()
            {
                return this.SHA1.ToString();
            }
        }

        public struct EncryptedChunkEntry
        {
            public ChunkEntry Chunk;
            public CryptoInfo CryptoInfo;

            public static EncryptedChunkEntry Read(Stream input, Endian endian)
            {
                EncryptedChunkEntry instance;
                instance.Chunk = ChunkEntry.Read(input, endian);
                instance.CryptoInfo = CryptoInfo.Read(input, endian);
                return instance;
            }

            public override string ToString()
            {
                return this.Chunk.ToString();
            }
        }

        public struct CryptoInfo
        {
            public uint Size;
            public string KeyId;
            public byte[] Unknown;

            public static CryptoInfo Read(Stream input, Endian endian)
            {
                CryptoInfo instance;
                instance.Size = input.ReadValueU32(endian);
                instance.KeyId = input.ReadString(8, true, Encoding.ASCII);
                instance.Unknown = input.ReadBytes(32);
                return instance;
            }
        }
    }
}
