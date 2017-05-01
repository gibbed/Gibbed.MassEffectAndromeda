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

using System.Text;

namespace Gibbed.Frostbite3.ResourceFormats.Partition
{
    public struct DefinitionFlags
    {
        public ushort RawFlags;

        public DefinitionFlags(ushort rawFlags)
        {
            this.RawFlags = rawFlags;
        }

        public byte UnknownFlags
        {
            get { return (byte)(this.RawFlags & 0x3); }
        }

        public DataKind DataKind
        {
            get { return (DataKind)((this.RawFlags >> 2) & 0x3); }
        }

        public DataType DataType
        {
            get { return (DataType)((this.RawFlags >> 4) & 0x1F); }
        }

        public byte UnknownFlags2
        {
            get { return (byte)((this.RawFlags >> 9) & 0x3); }
        }

        public DataFlags DataFlags
        {
            get { return (DataFlags)((this.RawFlags >> 11) & 0x1F); }
        }

        public static explicit operator DefinitionFlags(ushort value)
        {
            return new DefinitionFlags(value);
        }

        public bool IsVoid
        {
            get
            {
                return this.DataType == DataType.Void &&
                       this.DataKind == DataKind.None &&
                       this.DataFlags == DataFlags.None;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.DataType);

            if (this.DataKind != DataKind.None || this.DataFlags != DataFlags.None)
            {
                sb.Append(" (");

                if (this.DataKind != DataKind.None)
                {
                    sb.Append(this.DataKind);

                    if (this.DataFlags != DataFlags.None)
                    {
                        sb.Append("; ");
                    }
                }

                if (this.DataFlags != DataFlags.None)
                {
                    sb.Append(this.DataFlags);
                }

                sb.Append(")");
            }

            if (this.UnknownFlags != 0)
            {
                sb.Append(" UNK=");
                sb.Append(this.UnknownFlags);
            }

            return sb.ToString();
        }
    }
}
