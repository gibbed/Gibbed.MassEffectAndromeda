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

namespace Gibbed.Frostbite3.Unbundling
{
    internal static class CompressionHelper
    {
        public static void Decompress(Stream input, Stream output, long originalSize)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            var remaining = originalSize;
            var work = new byte[0];
            var buffer = new byte[0];

            while (remaining > 0)
            {
                var header = CompressionHeader.Read(input);
                if (header.CompressionFlags == CompressionFlags.None)
                {
                    throw new NotSupportedException();
                    header.CompressionType = header.CompressedBlockSize == header.UncompressedBlockSize
                                                 ? CompressionType.None
                                                 : CompressionType.Zlib;
                    header.CompressionFlags = (CompressionFlags)7;
                }
                var compressedBlockSize = header.CompressedBlockSize;
                var uncompressedBlockSize = header.UncompressedBlockSize;

                if (buffer.Length < uncompressedBlockSize)
                {
                    Array.Resize(ref buffer, uncompressedBlockSize);
                }

                switch (header.CompressionType)
                {
                    case CompressionType.None:
                    {
                        if (compressedBlockSize != uncompressedBlockSize)
                        {
                            throw new InvalidOperationException();
                        }

                        output.WriteFromStream(input, compressedBlockSize);
                        remaining -= compressedBlockSize;
                        break;
                    }

                    case CompressionType.Zstd:
                    {
                        if (work.Length < compressedBlockSize)
                        {
                            Array.Resize(ref work, compressedBlockSize);
                        }

                        var read = input.Read(work, 0, compressedBlockSize);
                        if (read != compressedBlockSize)
                        {
                            throw new EndOfStreamException();
                        }

                        var result = Zstd.Decompress(work, 0, compressedBlockSize, buffer, 0, uncompressedBlockSize);
                        if (Zstd.IsError(result) == true)
                        {
                            throw new InvalidOperationException();
                        }

                        if (result != (uint)uncompressedBlockSize)
                        {
                            throw new InvalidOperationException();
                        }

                        output.Write(buffer, 0, uncompressedBlockSize);
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

        public static int DecompressBlock(Stream input, byte[] buffer, byte[] work)
        {
            return DecompressBlock(input, buffer, 0, buffer.Length, work);
        }

        public static int DecompressBlock(Stream input, byte[] buffer, int offset, int count, byte[] work)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0 || offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (work == null)
            {
                throw new ArgumentNullException("work");
            }

            var header = CompressionHeader.Read(input);
            /*
            if (header.CompressionFlags == CompressionFlags.None)
            {
                throw new NotSupportedException();
                header.CompressionType = header.CompressedBlockSize == header.UncompressedBlockSize
                                             ? CompressionType.None
                                             : CompressionType.Zlib;
                header.CompressionFlags = (CompressionFlags)7;
            }
            */

            if (header.UncompressedBlockSize > count)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            switch (header.CompressionType)
            {
                case CompressionType.None:
                {
                    if (header.CompressedBlockSize != header.UncompressedBlockSize)
                    {
                        throw new InvalidOperationException();
                    }

                    return input.Read(buffer, offset, Math.Min(count, header.CompressedBlockSize));
                }

                case CompressionType.Zstd:
                {
                    var read = input.Read(work, 0, header.CompressedBlockSize);
                    if (read != header.CompressedBlockSize)
                    {
                        throw new EndOfStreamException();
                    }

                    var result = Zstd.Decompress(work, 0, header.CompressedBlockSize, buffer, offset, count);
                    if (Zstd.IsError(result) == true)
                    {
                        throw new InvalidOperationException();
                    }
                    return (int)result;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        public static void SkipBlock(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var header = CompressionHeader.Read(input);
            if (header.CompressionFlags == CompressionFlags.None)
            {
                throw new NotSupportedException();
                header.CompressionType = header.CompressedBlockSize == header.UncompressedBlockSize
                                             ? CompressionType.None
                                             : CompressionType.Zlib;
                header.CompressionFlags = (CompressionFlags)7;
            }

            switch (header.CompressionType)
            {
                case CompressionType.None:
                case CompressionType.Zstd:
                {
                    input.Seek(header.CompressedBlockSize, SeekOrigin.Current);
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
