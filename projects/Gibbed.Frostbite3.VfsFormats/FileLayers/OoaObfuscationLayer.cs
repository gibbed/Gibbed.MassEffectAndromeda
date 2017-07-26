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
using System.Linq;
using System.Text;
using Gibbed.IO;

namespace Gibbed.Frostbite3.VfsFormats.FileLayers
{
    internal class OoaObfuscationLayer
    {
        public const string Signature = "@e!adnXd$^!rfOsrDyIrI!xVgHeA!6Vc";

        private static readonly byte[] _SignatureBytes;

        static OoaObfuscationLayer()
        {
            _SignatureBytes = Encoding.UTF8.GetBytes(Signature);
        }

        public static bool IsObfuscated(Stream input)
        {
            var basePosition = input.Position;
            if (basePosition + 36 > input.Length)
            {
                return false;
            }

            input.Position = input.Length - 36;
            var tail = input.ReadBytes(36);
            input.Position = basePosition;

            if (tail.Skip(4).SequenceEqual(_SignatureBytes) == false)
            {
                return false;
            }

            return true;
        }

        public static MemoryStream Deobfuscate(Stream input)
        {
            var basePosition = input.Position;

            input.Position = input.Length - 36;
            var tail = input.ReadBytes(36);

            if (tail.Skip(4).SequenceEqual(_SignatureBytes) == false)
            {
                throw new FormatException();
            }

            var setupLength = BitConverter.ToInt32(tail, 0);
            input.Position = basePosition + (input.Length - setupLength);

            var setupBytes = input.ReadBytes(setupLength);
            var setupData = Bogocrypt.Crypt(setupBytes, 0, setupLength);

            var fileSize = BitConverter.ToUInt32(setupData, 402);
            var tailSize = input.Length - fileSize;
            Array.Copy(BitConverter.GetBytes(tailSize), 0, setupData, 406, 4);

            var version = BitConverter.ToUInt32(setupData, 0);
            if (version > 4)
            {
                throw new FormatException();
            }

            if (fileSize > int.MaxValue)
            {
                throw new FormatException();
            }

            var mode = setupData[4];
            switch (mode)
            {
                case 1:
                {
                    throw new NotImplementedException();
                }

                case 2:
                {
                    var seed = setupData[5];
                    var magic = seed;
                    input.Position = basePosition;
                    var bytes = input.ReadBytes((int)fileSize);
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        var b = bytes[i];
                        bytes[i] = (byte)(b ^ magic);
                        magic = (byte)((b ^ seed) - i);
                    }
                    return new MemoryStream(bytes, false);
                }

                case 3:
                {
                    input.Position = basePosition;
                    var bytes = input.ReadBytes((int)fileSize);
                    return new MemoryStream(bytes, false);
                }
            }
            throw new NotSupportedException();
        }

        internal static class Bogocrypt
        {
            public static byte[] Crypt(byte[] buffer, int offset, int count)
            {
                var data = new byte[count];
                Array.Copy(buffer, offset, data, 0, count);
                Crypt(data);
                return data;
            }

            private static void Crypt(byte[] data)
            {
                var length = data.Length;

                var length1 = BitConverter.ToUInt16(data, 392);
                uint a = 0;
                for (int i = 0, o = 410; i < length1; i++, o++)
                {
                    a = data[o] ^ 2 * (data[o] + a);
                }

                var b = (data[405] ^ 2 *
                         (data[405] + (data[404] ^ 2 *
                                       (data[404] + (data[403] ^ 2 *
                                                     (data[403] + (data[402] ^ 2 *
                                                                   data[402]))))))) +
                        (data[3] ^ 2 *
                         (data[3] + (data[2] ^ 2 *
                                     (data[2] + (data[1] ^ 2 *
                                                 (data[1] + (data[0] ^ 2 *
                                                             data[0]))))))) +
                        (data[391] ^ 2 * data[391]) + a +
                        (data[397] ^ 2 *
                         (data[397] + (data[396] ^ 2 *
                                       (data[396] + (data[395] ^ 2 *
                                                     (data[395] + (data[394] ^ 2 *
                                                                   data[394])))))));
                var c = (data[409] ^ 2 *
                         (data[409] + (data[408] ^ 2 *
                                       (data[408] +
                                        (data[407] ^ 2 *
                                         (data[407] + (data[406] ^ 2 *
                                                       data[406]))))))) + b;

                uint d = 0;
                for (int i = 0, o = 4; i < 129; i++, o += 3)
                {
                    var j = data[o + 0];
                    var k = data[o + 1];
                    var l = data[o + 2];
                    d = l ^ 2 * (l + (k ^ 2 * (k + (j ^ 2 * (j + d)))));
                }

                var length2 = BitConverter.ToUInt16(data, 392);
                if (length2 > 0)
                {
                    CryptSection(data, 410, length2, 0x45EA1278);
                }

                data[391] ^= 0xB9;
                CryptSection(data, 394, 4, 0x45EA1278);
                CryptSection(data, 0, 4, 0x45EA1278);
                CryptSection(data, 402, 4, 0x45EA1278);
                CryptSection(data, 406, 4, 0x45EA1278);
                CryptSection(data, 4, 387, 0x45EA1278);
                Array.Copy(BitConverter.GetBytes(length), 0, data, 398, 4);

                var e = (byte)(d + c);
                switch (BitConverter.ToUInt16(data, 325))
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                    {
                        data[5] ^= e;
                        break;
                    }
                }
            }

            private static void CryptSection(byte[] data, int offset, int count, uint seed)
            {
                var hash = seed;
                for (int i = 0, j = 0, o = offset; i < count; i++, o++)
                {
                    var b = (byte)(data[o] ^ (hash + (byte)(hash >> 8) + (byte)(hash >> 16) + (byte)(hash >> 24)));
                    data[o] = b;

                    var c = hash.RotateLeft(b & 31);
                    hash = ((uint)((b | ((b | ((b | (b << 8)) << 8)) << 8)) + c)).RotateLeft(1);

                    if (j > 16)
                    {
                        hash *= 2;
                        j = 0;
                    }
                    j++;
                }
            }
        }
    }
}
