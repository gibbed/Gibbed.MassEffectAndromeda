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
using System.IO.MemoryMappedFiles;
using System.Linq;
using Gibbed.Frostbite3.Common;
using Gibbed.Frostbite3.VfsFormats;

namespace Gibbed.Frostbite3.Unbundling
{
    internal class ChunkLoader : IDisposable
    {
        private readonly CatalogFile _Catalog;
        private readonly Dictionary<byte, MemoryMappedFile> _Files;
        private readonly Dictionary<SHA1Hash, List<ChunkVariantInfo>> _ChunkLookup;

        public ChunkLoader(CatalogFile catalog,
                           IEnumerable<KeyValuePair<byte, MemoryMappedFile>> files,
                           Dictionary<SHA1Hash, List<ChunkVariantInfo>> chunkLookup)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException("catalog");
            }

            this._Catalog = catalog;
            this._Files = new Dictionary<byte, MemoryMappedFile>();
            this._ChunkLookup = new Dictionary<SHA1Hash, List<ChunkVariantInfo>>();

            foreach (var kv in files)
            {
                this._Files[kv.Key] = kv.Value;
            }

            foreach (var kv in chunkLookup)
            {
                this._ChunkLookup[kv.Key] = new List<ChunkVariantInfo>(kv.Value);
            }
        }

        ~ChunkLoader()
        {
            this.Dispose(false);
        }

        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        public CatalogFile Catalog
        {
            get { return this._Catalog; }
        }

        public Dictionary<byte, MemoryMappedFile> Files
        {
            get { return this._Files; }
        }

        public Dictionary<SHA1Hash, List<ChunkVariantInfo>> ChunkLookup
        {
            get { return this._ChunkLookup; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this._Files != null)
                {
                    foreach (var dataStream in this._Files.Values)
                    {
                        dataStream.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static ChunkLoader Load(string basePath)
        {
            var catalogPath = Path.Combine(basePath, "cas.cat");
            if (File.Exists(catalogPath) == false)
            {
                return null;
            }

            Logger.Info("Reading catalog '{0}'", catalogPath);
            var catalog = CatalogFile.Read(catalogPath);

            var chunkDataIndices = catalog.NormalEntries.Select(ce => ce.DataIndex);
            var encryptedChunkDataIndices = catalog.EncryptedEntries.Select(ece => ece.Entry.DataIndex);

            var files = new Dictionary<byte, MemoryMappedFile>();
            foreach (var index in chunkDataIndices.Concat(encryptedChunkDataIndices).Distinct().OrderBy(v => v))
            {
                var dataPath = Path.Combine(basePath, string.Format("cas_{0:D2}.cas", index));
                if (File.Exists(dataPath) == false)
                {
                    foreach (var stream in files.Values)
                    {
                        stream.Dispose();
                    }
                    return null;
                }

                var temp = File.OpenRead(dataPath);
                files[index] = MemoryMappedFile.CreateFromFile(
                    temp,
                    null,
                    0,
                    MemoryMappedFileAccess.Read,
                    null,
                    HandleInheritability.None,
                    false);
            }

            var chunkLookup = new Dictionary<SHA1Hash, List<ChunkVariantInfo>>();

            for (int i = 0; i < catalog.NormalEntries.Count; i++)
            {
                var entry = catalog.NormalEntries[i];

                List<ChunkVariantInfo> chunkVariants;
                if (chunkLookup.TryGetValue(entry.Id, out chunkVariants) == false)
                {
                    chunkVariants = chunkLookup[entry.Id] = new List<ChunkVariantInfo>();
                }

                chunkVariants.Add(new ChunkVariantInfo(i, ChunkVariantType.Normal));
            }

            for (int i = 0; i < catalog.PatchEntries.Count; i++)
            {
                var entry = catalog.PatchEntries[i];

                List<ChunkVariantInfo> chunkVariants;
                if (chunkLookup.TryGetValue(entry.Id, out chunkVariants) == false)
                {
                    chunkVariants = chunkLookup[entry.Id] = new List<ChunkVariantInfo>();
                }

                chunkVariants.Add(new ChunkVariantInfo(i, ChunkVariantType.Patch));
            }

            for (int i = 0; i < catalog.EncryptedEntries.Count; i++)
            {
                var entry = catalog.EncryptedEntries[i];

                List<ChunkVariantInfo> chunkVariants;
                if (chunkLookup.TryGetValue(entry.Entry.Id, out chunkVariants) == false)
                {
                    chunkVariants = chunkLookup[entry.Entry.Id] = new List<ChunkVariantInfo>();
                }

                chunkVariants.Add(new ChunkVariantInfo(i, ChunkVariantType.Encrypted));
            }

            return new ChunkLoader(catalog, files, chunkLookup);
        }
    }
}
