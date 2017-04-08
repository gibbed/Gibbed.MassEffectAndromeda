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
using System.Globalization;
using System.Text;

namespace Gibbed.Frostbite3.Common
{
    public struct SHA1Hash : IEquatable<SHA1Hash>, IComparable<SHA1Hash>
    {
        private readonly uint _A;
        private readonly uint _B;
        private readonly uint _C;
        private readonly uint _D;
        private readonly uint _E;

        public SHA1Hash(byte[] b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            if (b.Length != 20)
            {
                throw new ArgumentException("", "b");
            }

            this._A = (uint)b[0] << 24 | (uint)b[1] << 16 | (uint)b[2] << 8 | b[3];
            this._B = (uint)b[4] << 24 | (uint)b[5] << 16 | (uint)b[6] << 8 | b[7];
            this._C = (uint)b[8] << 24 | (uint)b[9] << 16 | (uint)b[10] << 8 | b[11];
            this._D = (uint)b[12] << 24 | (uint)b[13] << 16 | (uint)b[14] << 8 | b[15];
            this._E = (uint)b[16] << 24 | (uint)b[17] << 16 | (uint)b[18] << 8 | b[19];
        }

        public static bool operator ==(SHA1Hash left, SHA1Hash right)
        {
            return left.Equals(right) == true;
        }

        public static bool operator !=(SHA1Hash left, SHA1Hash right)
        {
            return left.Equals(right) == false;
        }

        public byte[] ToByteArray()
        {
            var b = new byte[20];
            b[0] = (byte)(this._A >> 24);
            b[1] = (byte)(this._A >> 16);
            b[2] = (byte)(this._A >> 8);
            b[3] = (byte)this._A;
            b[4] = (byte)(this._B >> 24);
            b[5] = (byte)(this._B >> 16);
            b[6] = (byte)(this._B >> 8);
            b[7] = (byte)this._B;
            b[8] = (byte)(this._C >> 24);
            b[9] = (byte)(this._C >> 16);
            b[10] = (byte)(this._C >> 8);
            b[11] = (byte)this._C;
            b[12] = (byte)(this._D >> 24);
            b[13] = (byte)(this._D >> 16);
            b[14] = (byte)(this._D >> 8);
            b[15] = (byte)this._D;
            b[16] = (byte)(this._E >> 24);
            b[17] = (byte)(this._E >> 16);
            b[18] = (byte)(this._E >> 8);
            b[19] = (byte)this._E;
            return b;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(40, 40);
            sb.Append(this._A.ToString("X8", CultureInfo.InvariantCulture));
            sb.Append(this._B.ToString("X8", CultureInfo.InvariantCulture));
            sb.Append(this._C.ToString("X8", CultureInfo.InvariantCulture));
            sb.Append(this._D.ToString("X8", CultureInfo.InvariantCulture));
            sb.Append(this._E.ToString("X8", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this._A;
                hashCode = (hashCode * 397) ^ (int)this._B;
                hashCode = (hashCode * 397) ^ (int)this._C;
                hashCode = (hashCode * 397) ^ (int)this._D;
                hashCode = (hashCode * 397) ^ (int)this._E;
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) == true)
            {
                return false;
            }
            return obj is SHA1Hash && Equals((SHA1Hash)obj) == true;
        }

        public bool Equals(SHA1Hash other)
        {
            return this._A == other._A &&
                   this._B == other._B &&
                   this._C == other._C &&
                   this._D == other._D &&
                   this._E == other._E;
        }

        public int CompareTo(SHA1Hash other)
        {
            if (this._A != other._A)
            {
                return this._A < other._A ? -1 : 1;
            }

            if (this._B != other._B)
            {
                return this._B < other._B ? -1 : 1;
            }

            if (this._C != other._C)
            {
                return this._C < other._C ? -1 : 1;
            }

            if (this._D != other._D)
            {
                return this._D < other._D ? -1 : 1;
            }

            if (this._E != other._E)
            {
                return this._E < other._E ? -1 : 1;
            }

            return 0;
        }
    }
}
