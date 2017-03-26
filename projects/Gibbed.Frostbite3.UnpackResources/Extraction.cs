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
using Gibbed.Frostbite3.FileFormats;
using Gibbed.IO;

namespace Gibbed.Frostbite3.UnpackResources
{
    internal static class Extraction
    {
        public static void Extract(ICatalogEntryInfo catalogInfo, long uncompressedSize, Stream output)
        {
            if (catalogInfo == null)
            {
                throw new ArgumentNullException("catalogInfo");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            using (var input = File.OpenRead(catalogInfo.DataPath))
            {
                input.Position = catalogInfo.Offset;
                Decompress(input, output, uncompressedSize);
            }
        }

        public static void Extract(SuperbundleFile.IDataEntry bundleInfo, ICatalogEntryInfo catalogInfo, Stream output)
        {
            if (bundleInfo == null)
            {
                throw new ArgumentNullException("bundleInfo");
            }

            if (catalogInfo == null)
            {
                throw new ArgumentNullException("catalogInfo");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            Stream input;

            if (bundleInfo.InlineData != null)
            {
                if (bundleInfo.InlineData.Length != bundleInfo.Size)
                {
                    throw new InvalidOperationException();
                }

                input = new MemoryStream(bundleInfo.InlineData, false);
            }
            else
            {
                input = File.OpenRead(catalogInfo.DataPath);
                input.Position = catalogInfo.Offset;
            }

            using (input)
            {
                if (bundleInfo.Size == bundleInfo.OriginalSize)
                {
                    output.WriteFromStream(input, bundleInfo.Size);
                }
                else
                {
                    Decompress(input, output, bundleInfo.OriginalSize);
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
