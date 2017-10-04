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

namespace Gibbed.Frostbite3.Common
{
    internal static class MathHelpers
    {
        public static int BitScanReverse(this ulong value)
        {
            unchecked
            {
                int r = 0;

                if (value > 0x00000000FFFFFFFF)
                {
                    value >>= 32;
                    r += 32;
                }
                if (value > 0x000000000000FFFF)
                {
                    value >>= 16;
                    r += 16;
                }
                if (value > 0x00000000000000FF)
                {
                    value >>= 08;
                    r += 08;
                }
                if (value > 0x000000000000000F)
                {
                    value >>= 04;
                    r += 04;
                }
                if (value > 0x0000000000000003)
                {
                    value >>= 02;
                    r += 02;
                }
                if (value > 0x0000000000000001)
                {
                    r += 01;
                }

                return r;
            }
        }
    }
}
