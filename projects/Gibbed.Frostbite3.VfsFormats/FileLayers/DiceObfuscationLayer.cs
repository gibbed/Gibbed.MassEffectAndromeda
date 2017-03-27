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
using Gibbed.IO;

namespace Gibbed.Frostbite3.VfsFormats.FileLayers
{
    internal class DiceObfuscationLayer
    {
        public const uint Signature = 0x00CED100;

        public static bool IsObfuscated(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            input.Seek(-4, SeekOrigin.Current);
            return magic == Signature;
        }

        public static MemoryStream Deobfuscate(Stream input)
        {
            const Endian endian = Endian.Little;

            var magic = input.ReadValueU32(endian);
            if (magic != Signature)
            {
                throw new FormatException();
            }

            var unknown0 = input.ReadValueU32(endian);
            if (unknown0 != 0)
            {
                throw new FormatException();
            }

            //var signature = input.ReadBytes(288);
            input.Seek(288, SeekOrigin.Current);
            var key = input.ReadBytes(257);
            var padding = input.ReadBytes(3);
            
            if (padding.Any(b => b != 0) == true)
            {
                throw new FormatException();
            }

            var bytes = input.ReadBytes((int)(input.Length - input.Position));
            int length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                bytes[i] ^= key[i % key.Length];
                bytes[i] ^= 0x7B;
            }

            return new MemoryStream(bytes, false);
        }
    }
}
