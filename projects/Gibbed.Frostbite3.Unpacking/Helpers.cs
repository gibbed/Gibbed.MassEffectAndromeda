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
using System.IO;

namespace Gibbed.Frostbite3.Unpacking
{
    public class Helpers
    {
        public static string FilterPath(string name)
        {
            name = name.Replace('/', Path.DirectorySeparatorChar);
            return name;
        }

        public static string FilterName(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            path = path.Replace(Path.AltDirectorySeparatorChar, '/');
            return path;
        }

        public static int CompareName(string left, string right)
        {
            return string.Compare(left, right, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);
        }

        public static byte[] GetBytesFromHexString(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new ArgumentOutOfRangeException("s");
            }

            var length = hex.Length >> 1;
            var bytes = new byte[length];
            for (int i = 0, o = 0; o < length; i += 2, o++)
            {
                bytes[o] = (byte)((GetHexValue(hex[i + 0]) << 4) + (GetHexValue(hex[i + 1])));
            }
            return bytes;
        }

        public static int GetHexValue(char hex)
        {
            var val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
