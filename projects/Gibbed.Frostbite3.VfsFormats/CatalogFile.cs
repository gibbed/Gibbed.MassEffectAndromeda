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

        private readonly List<NormalEntry> _NormalEntries;
        private readonly List<PatchEntry> _PatchEntries;
        private readonly List<EncryptedChunkEntry> _EncryptedEntries;

        public CatalogFile()
        {
            this._NormalEntries = new List<NormalEntry>();
            this._PatchEntries = new List<PatchEntry>();
            this._EncryptedEntries = new List<EncryptedChunkEntry>();
        }

        public List<NormalEntry> NormalEntries
        {
            get { return this._NormalEntries; }
        }

        public List<PatchEntry> PatchEntries
        {
            get { return this._PatchEntries; }
        }

        public List<EncryptedChunkEntry> EncryptedEntries
        {
            get { return this._EncryptedEntries; }
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

            var normalCount = input.ReadValueU32(endian);
            var patchCount = input.ReadValueU32(endian);
            var encryptedCount = input.ReadValueU32(endian);
            var unknown1C = input.ReadValueS32(endian); // patch version related?
            var unknown20 = input.ReadValueS32(endian); // patch version related?
            var unknown24 = input.ReadValueS32(endian); // patch version related?

            if (patchCount == 0)
            {
                if (unknown1C != 0 || unknown20 != 0 || unknown24 != 0)
                {
                    throw new FormatException();
                }
            }
            else
            {
                if (unknown1C != -1 || unknown20 != -2 || unknown24 != -1)
                {
                    //throw new FormatException();
                }
            }

            instance.NormalEntries.Clear();
            for (int i = 0; i < normalCount; i++)
            {
                var entry = NormalEntry.Read(input, endian);
                if (entry.IsEncrypted == true)
                {
                    throw new FormatException();
                }
                instance.NormalEntries.Add(entry);
            }

            instance.EncryptedEntries.Clear();
            for (int i = 0; i < encryptedCount; i++)
            {
                var encryptedEntry = EncryptedChunkEntry.Read(input, endian);
                if (encryptedEntry.Entry.IsEncrypted == false)
                {
                    throw new FormatException();
                }
                if (encryptedEntry.Entry.Size != encryptedEntry.CryptoInfo.Size)
                {
                    throw new FormatException();
                }
                instance.EncryptedEntries.Add(encryptedEntry);
            }

            instance.PatchEntries.Clear();
            for (int i = 0; i < patchCount; i++)
            {
                var entry = PatchEntry.Read(input, endian);
                instance.PatchEntries.Add(entry);
            }

            if (input.Position != input.Length)
            {
                throw new FormatException();
            }

            return instance;
        }

        public struct NormalEntry
        {
            // ReSharper disable InconsistentNaming
            public SHA1Hash Id;
            // ReSharper restore InconsistentNaming
            public uint Offset;
            public uint Size;
            public uint TailSize;
            public byte DataIndex;
            public bool IsEncrypted;

            public static NormalEntry Read(Stream input, Endian endian)
            {
                var instance = new NormalEntry();
                instance.Id = new SHA1Hash(input.ReadBytes(20));
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
                return this.Id.ToString();
            }
        }

        public struct PatchEntry
        {
            public SHA1Hash Id;
            public SHA1Hash BaseId;
            public SHA1Hash DeltaId;

            public static PatchEntry Read(Stream input, Endian endian)
            {
                PatchEntry instance;
                instance.Id = new SHA1Hash(input.ReadBytes(20));
                instance.BaseId = new SHA1Hash(input.ReadBytes(20));
                instance.DeltaId = new SHA1Hash(input.ReadBytes(20));
                return instance;
            }

            public override string ToString()
            {
                return string.Format("{0} = {1} + {2}", this.Id, this.BaseId, this.DeltaId);
            }
        }

        public struct EncryptedChunkEntry
        {
            public NormalEntry Entry;
            public CryptoInfo CryptoInfo;

            public static EncryptedChunkEntry Read(Stream input, Endian endian)
            {
                EncryptedChunkEntry instance;
                instance.Entry = NormalEntry.Read(input, endian);
                instance.CryptoInfo = CryptoInfo.Read(input, endian);
                return instance;
            }

            public override string ToString()
            {
                return this.Entry.ToString();
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
