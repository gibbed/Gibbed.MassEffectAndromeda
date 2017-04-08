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
    internal class PatchHelper
    {
        public static void Patch(Stream input, Stream delta, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (delta == null)
            {
                throw new ArgumentNullException("delta");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            const Endian endian = Endian.Big;

            var work = new byte[0x10000];
            var inputBytes = new byte[0x10000];
            var deltaBytes = new byte[0x10000];

            while (delta.Position < delta.Length)
            {
                var rawFlags = delta.ReadValueU32(endian);
                var count = (int)(rawFlags & 0xFFFFFFF);
                var type = (PatchType)(rawFlags >> 28);

                switch (type)
                {
                    case PatchType.CompressedChange:
                    {
                        var inputRead = CompressionHelper.DecompressBlock(input, inputBytes, work);
                        int o = 0;
                        for (int i = 0; i < count; i++)
                        {
                            var inputOffset = delta.ReadValueU16(endian);
                            var inputSkipLength = delta.ReadValueU16(endian);
                            var inputCopyLength = inputOffset - o;
                            if (o + inputCopyLength > inputRead)
                            {
                                throw new EndOfStreamException();
                            }
                            output.Write(inputBytes, o, inputCopyLength);
                            var deltaRead = CompressionHelper.DecompressBlock(delta, deltaBytes, work);
                            output.Write(deltaBytes, 0, deltaRead);
                            o = inputOffset + inputSkipLength;
                        }
                        output.Write(inputBytes, o, inputRead - o);
                        break;
                    }

                    case PatchType.UncompressedChange:
                    {
                        var inputRead = CompressionHelper.DecompressBlock(input, inputBytes, work);
                        var totalSize = 1 + delta.ReadValueU16(endian);
                        var basePosition = output.Position;
                        using (var temp = delta.ReadToMemoryStream(count))
                        {
                            temp.Position = 0;
                            int o = 0;
                            while (temp.Position < temp.Length)
                            {
                                var inputOffset = temp.ReadValueU16(endian);
                                var inputSkipLength = temp.ReadValueU8();
                                var deltaCopyLength = temp.ReadValueU8();

                                var inputCopyLength = inputOffset - o;
                                if (o + inputCopyLength > inputRead)
                                {
                                    throw new EndOfStreamException();
                                }
                                output.Write(inputBytes, o, inputCopyLength);

                                if (deltaCopyLength > 0)
                                {
                                    var deltaRead = temp.Read(deltaBytes, 0, deltaCopyLength);
                                    if (deltaRead != deltaCopyLength)
                                    {
                                        throw new EndOfStreamException();
                                    }
                                    output.Write(deltaBytes, 0, deltaCopyLength);
                                }

                                o = inputOffset + inputSkipLength;
                            }
                            output.Write(inputBytes, o, inputRead - o);
                        }
                        if ((output.Position - basePosition) != totalSize)
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    }

                    case PatchType.Add:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var deltaRead = CompressionHelper.DecompressBlock(delta, deltaBytes, work);
                            output.Write(deltaBytes, 0, deltaRead);
                        }
                        break;
                    }

                    case PatchType.Remove:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            CompressionHelper.SkipBlock(input);
                        }
                        break;
                    }

                    default:
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (delta.Position != delta.Length)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
