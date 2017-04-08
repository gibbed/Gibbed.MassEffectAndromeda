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

using System.IO;
using Gibbed.IO;

namespace Gibbed.Frostbite3.Unbundling
{
    internal struct CompressionHeader
    {
        public int UncompressedBlockSize;
        private uint _RawCompressionFlags;

        public CompressionType CompressionType
        {
            get { return (CompressionType)(this._RawCompressionFlags >> 24); }
            set
            {
                this._RawCompressionFlags &= ~0xFF000000u;
                this._RawCompressionFlags |= ((uint)value & 0xFFu) << 24;
            }
        }

        public CompressionFlags CompressionFlags
        {
            get { return (CompressionFlags)((this._RawCompressionFlags >> 20) & 0xFu); }
            set
            {
                this._RawCompressionFlags &= ~0x00F00000u;
                this._RawCompressionFlags |= ((uint)value & 0xFu) << 20;
            }
        }

        public int CompressedBlockSize
        {
            get { return (int)(this._RawCompressionFlags & 0xFFFFFu); }
            set
            {
                this._RawCompressionFlags &= ~0x000FFFFFu;
                this._RawCompressionFlags |= (uint)value & 0xFFFFFu;
            }
        }

        public static CompressionHeader Read(Stream input)
        {
            const Endian endian = Endian.Big;
            CompressionHeader instance;
            instance.UncompressedBlockSize = input.ReadValueS32(endian);
            instance._RawCompressionFlags = input.ReadValueU32(endian);
            return instance;
        }

        public void Write(Stream output)
        {
            const Endian endian = Endian.Big;
            output.WriteValueS32(this.UncompressedBlockSize, endian);
            output.WriteValueU32(this._RawCompressionFlags, endian);
        }
    }
}
