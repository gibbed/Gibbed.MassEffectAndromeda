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
using Gibbed.IO;
using Superbundle = Gibbed.Frostbite3.VfsFormats.Superbundle;

namespace Gibbed.Frostbite3.Unpacking
{
    public class ChunkLoader
    {
        private readonly ChunkLookup _Lookup;
        private readonly Dictionary<string, byte[]> _CryptoKeys;

        public ChunkLoader(ChunkLookup lookup)
        {
            if (lookup == null)
            {
                throw new ArgumentNullException("lookup");
            }

            this._Lookup = lookup;
            this._CryptoKeys = new Dictionary<string, byte[]>();
        }

        public void AddCryptoKey(string keyId, string keyText)
        {
            if (this._CryptoKeys.ContainsKey(keyId) == true)
            {
                throw new InvalidOperationException();
            }

            var keyBytes = Helpers.GetBytesFromHexString(keyText);
            this._CryptoKeys.Add(keyId, keyBytes);
        }

        public void Load(Superbundle.IDataInfo dataInfo, Stream output)
        {
            if (dataInfo.Size == dataInfo.OriginalSize)
            {
                if (dataInfo.InlineData != null)
                {
                    if (dataInfo.InlineData.Length != dataInfo.Size)
                    {
                        throw new InvalidOperationException();
                    }

                    output.WriteBytes(dataInfo.InlineData);
                }
                else
                {
                    var chunkInfo = this._Lookup.GetChunkVariant(dataInfo);
                    if (chunkInfo == null)
                    {
                        throw new InvalidOperationException();
                    }

                    Load(chunkInfo, output);
                }
            }
            else
            {
                MemoryStream temp;

                if (dataInfo.InlineData != null)
                {
                    if (dataInfo.InlineData.Length != dataInfo.Size)
                    {
                        throw new InvalidOperationException();
                    }

                    temp = new MemoryStream(dataInfo.InlineData, false);
                }
                else
                {
                    var chunkInfo = this._Lookup.GetChunkVariant(dataInfo);
                    if (chunkInfo == null)
                    {
                        throw new InvalidOperationException();
                    }

                    temp = new MemoryStream();
                    Load(chunkInfo, temp);
                    temp.Position = 0;
                }

                using (temp)
                {
                    Decompress(temp, output, dataInfo.OriginalSize);
                }
            }
        }

        public void Load(ChunkLookup.IChunkVariantInfo chunkVariantInfo, long size, Stream output)
        {
            if (chunkVariantInfo == null)
            {
                throw new ArgumentNullException("chunkVariantInfo");
            }

            if (chunkVariantInfo.Size == size)
            {
                Load(chunkVariantInfo, output);
            }
            else
            {
                using (var temp = new MemoryStream())
                {
                    Load(chunkVariantInfo, temp);
                    if (temp.Length != chunkVariantInfo.Size)
                    {
                        throw new InvalidOperationException();
                    }
                    temp.Position = 0;
                    Decompress(temp, output, size);
                }
            }
        }

        private void Load(ChunkLookup.IChunkVariantInfo chunkVariantInfo, Stream output)
        {
            using (var file = MemoryMappedFile.CreateFromFile(chunkVariantInfo.DataPath, FileMode.Open))
            using (var input = file.CreateViewStream(chunkVariantInfo.Offset, chunkVariantInfo.Size.Align(16)))
            {
                if (chunkVariantInfo.CryptoInfo.HasValue == false)
                {
                    output.WriteFromStream(input, chunkVariantInfo.Size);
                    return;
                }

                var keyId = chunkVariantInfo.CryptoInfo.Value.KeyId;
                byte[] keyBytes;
                if (this._CryptoKeys.TryGetValue(keyId, out keyBytes) == false)
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
                        output.WriteFromStream(cryptoStream, chunkVariantInfo.Size);
                    }
                }
            }
        }

        private static void Decompress(Stream input, Stream output, long originalSize)
        {
            var remaining = originalSize;
            var compressedBytes = new byte[0];
            var uncompressedBytes = new byte[0];

            while (remaining > 0)
            {
                var uncompressedBlockSize = input.ReadValueS32(Endian.Big);
                var compressionType = input.ReadValueU16(Endian.Big);
                var compressedBlockSize = input.ReadValueU16(Endian.Big);

                if (uncompressedBytes.Length < uncompressedBlockSize)
                {
                    Array.Resize(ref uncompressedBytes, uncompressedBlockSize);
                }

                switch (compressionType) // might be flags+type?
                {
                    case 0x70:
                    {
                        if (compressedBlockSize != uncompressedBlockSize)
                        {
                            throw new InvalidOperationException();
                        }

                        output.WriteFromStream(input, compressedBlockSize);
                        remaining -= compressedBlockSize;
                        break;
                    }

                    case 0x71:
                    {
                        if (compressedBlockSize != 0)
                        {
                            throw new InvalidOperationException();
                        }

                        output.WriteFromStream(input, uncompressedBlockSize);
                        remaining -= uncompressedBlockSize;
                        break;
                    }

                    case 0x0F70:
                    {
                        if (compressedBytes.Length < compressedBlockSize)
                        {
                            Array.Resize(ref compressedBytes, compressedBlockSize);
                        }

                        var read = input.Read(compressedBytes, 0, compressedBlockSize);
                        if (read != compressedBlockSize)
                        {
                            throw new EndOfStreamException();
                        }

                        var result = Zstd.Decompress(
                            compressedBytes,
                            0,
                            compressedBlockSize,
                            uncompressedBytes,
                            0,
                            uncompressedBlockSize);
                        if (Zstd.IsError(result) == true)
                        {
                            throw new InvalidOperationException();
                        }

                        if (result != (uint)uncompressedBlockSize)
                        {
                            throw new InvalidOperationException();
                        }

                        output.Write(uncompressedBytes, 0, uncompressedBlockSize);
                        remaining -= uncompressedBlockSize;
                        break;
                    }

                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
            }
        }
    }
}
