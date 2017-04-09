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
using System.Security.Cryptography;
using Gibbed.Frostbite3.Common;
using Gibbed.IO;
using Layout = Gibbed.Frostbite3.VfsFormats.Layout;

namespace Gibbed.Frostbite3.Unbundling
{
    public class InstallChunkManager : IDisposable
    {
        public delegate bool TryGetCryptoKeyDelegate(string keyId, out byte[] keyBytes);

        private readonly TryGetCryptoKeyDelegate _TryGetCryptoKey;
        private readonly List<ChunkLoader> _ChunkLoaders;

        private InstallChunkManager(TryGetCryptoKeyDelegate tryGetCryptoKey, IEnumerable<ChunkLoader> chunkLoaders)
        {
            this._TryGetCryptoKey = tryGetCryptoKey;
            this._ChunkLoaders = new List<ChunkLoader>();

            if (chunkLoaders != null)
            {
                this._ChunkLoaders.AddRange(chunkLoaders);
            }
        }

        ~InstallChunkManager()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this._ChunkLoaders != null)
                {
                    foreach (var chunkLoader in this._ChunkLoaders)
                    {
                        chunkLoader.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static InstallChunkManager Initialize(TryGetCryptoKeyDelegate tryGetCryptoKey,
                                                       Layout.InstallChunk installChunk,
                                                       IEnumerable<DataSource> sources)
        {
            if (installChunk == null)
            {
                throw new ArgumentNullException("installChunk");
            }

            var chunkLoaders = new List<ChunkLoader>();

            foreach (var source in sources)
            {
                var chunkPath = Path.Combine(source.Path, Helpers.FilterPath(installChunk.InstallBundle));
                var chunkLoader = ChunkLoader.Load(chunkPath);
                if (chunkLoader == null)
                {
                    continue;
                }
                chunkLoaders.Add(chunkLoader);
            }

            return new InstallChunkManager(tryGetCryptoKey, chunkLoaders);
        }

        private bool FindChunk(SHA1Hash id, out ChunkLoader chunkLoader, out List<ChunkVariantInfo> chunkVariants)
        {
            foreach (var candidate in this._ChunkLoaders)
            {
                if (candidate.ChunkLookup.TryGetValue(id, out chunkVariants) == true)
                {
                    chunkLoader = candidate;
                    return true;
                }
            }

            chunkLoader = null;
            chunkVariants = null;
            return false;
        }

        public bool LoadChunk(SHA1Hash id, long variantSize, long originalSize, Stream output)
        {
            return this.LoadChunk(id, variantSize, originalSize, output, false);
        }

        public bool LoadChunk(SHA1Hash id, long variantSize, long originalSize, Stream output, bool noPatching)
        {
            ChunkLoader chunkLoader;
            List<ChunkVariantInfo> chunkVariants;
            if (this.FindChunk(id, out chunkLoader, out chunkVariants) == false)
            {
                return false;
            }

            foreach (var chunkVariant in chunkVariants)
            {
                var type = chunkVariant.Type;
                var index = chunkVariant.Index;

                if (type == ChunkVariantType.Normal)
                {
                    if (this.LoadNormalChunk(chunkLoader, index, variantSize, originalSize, output) == true)
                    {
                        return true;
                    }
                }
                else if (noPatching == false && type == ChunkVariantType.Patch)
                {
                    if (this.LoadPatchChunk(chunkLoader, index, variantSize, originalSize, output) == true)
                    {
                        return true;
                    }
                }
                else if (type == ChunkVariantType.Encrypted)
                {
                    if (this.LoadEncryptedChunk(chunkLoader, index, variantSize, originalSize, output) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool LoadNormalChunk(ChunkLoader chunkLoader,
                                     int index,
                                     long variantSize,
                                     long originalSize,
                                     Stream output)
        {
            var entry = chunkLoader.Catalog.NormalEntries[index];
            if ((variantSize != 0 || entry.TailSize != 0) && variantSize != entry.Size)
            {
                return false;
            }

            var file = chunkLoader.Files[entry.DataIndex];
            using (var input = file.CreateViewStream(entry.Offset, entry.Size, MemoryMappedFileAccess.Read))
            {
                if (originalSize == 0 || originalSize == entry.Size)
                {
                    output.WriteFromStream(input, entry.Size);
                }
                else
                {
                    CompressionHelper.Decompress(input, output, originalSize);
                }
            }
            return true;
        }

        private bool LoadPatchChunk(ChunkLoader chunkLoader,
                                    int index,
                                    long variantSize,
                                    long originalSize,
                                    Stream output)
        {
            var patch = chunkLoader.Catalog.PatchEntries[index];

            using (var input = new MemoryStream())
            using (var delta = new MemoryStream())
            {
                if (this.LoadChunk(patch.BaseId, 0, 0, input, true) == false)
                {
                    throw new DataLoadException(string.Format("could not load base chunk '{0}' for chunk '{1}'",
                                                              patch.BaseId,
                                                              patch.Id));
                }

                if (this.LoadChunk(patch.DeltaId, 0, 0, delta, true) == false)
                {
                    throw new DataLoadException(string.Format("could not load delta chunk '{0}' for chunk '{1}'",
                                                              patch.DeltaId,
                                                              patch.Id));
                }

                input.Position = 0;
                delta.Position = 0;
                return PatchHelper.Patch(input, delta, output);
            }
        }

        private bool LoadEncryptedChunk(ChunkLoader chunkLoader,
                                        int index,
                                        long variantSize,
                                        long originalSize,
                                        Stream output)
        {
            var encryptedEntry = chunkLoader.Catalog.EncryptedEntries[index];
            if ((variantSize != 0 || encryptedEntry.Entry.TailSize != 0) && variantSize != encryptedEntry.Entry.Size)
            {
                return false;
            }

            var entry = encryptedEntry.Entry;
            var alignedSize = CryptoAlign(entry.Size, 16);
            var file = chunkLoader.Files[entry.DataIndex];
            using (var input = file.CreateViewStream(entry.Offset, alignedSize, MemoryMappedFileAccess.Read))
            {
                var keyId = encryptedEntry.CryptoInfo.KeyId;
                byte[] keyBytes;
                if (_TryGetCryptoKey == null || this._TryGetCryptoKey(keyId, out keyBytes) == false)
                {
                    throw new ChunkCryptoKeyMissingException(keyId);
                }

                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = keyBytes; // yes, it's that stupid
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var decryptor = aes.CreateDecryptor())
                    using (var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                    {
                        if (originalSize == 0 || originalSize == entry.Size)
                        {
                            output.WriteFromStream(cryptoStream, entry.Size);
                        }
                        else
                        {
                            CompressionHelper.Decompress(cryptoStream, output, originalSize);
                        }
                    }
                }
            }
            return true;
        }

        private static uint CryptoAlign(uint value, uint align)
        {
            return value + (align - (value % align));
        }

        public bool LoadData(VfsFormats.Superbundle.IDataInfo dataInfo, Stream output)
        {
            var basePosition = output.Position;
            var result = this.LoadChunk(dataInfo.SHA1, dataInfo.Size, dataInfo.OriginalSize, output);
            if (result == true && (output.Position - basePosition) != dataInfo.OriginalSize)
            {
                throw new InvalidOperationException();
            }
            return result;
        }
    }
}
