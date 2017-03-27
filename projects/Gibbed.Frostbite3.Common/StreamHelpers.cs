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

namespace Gibbed.Frostbite3.Common
{
    public static class StreamHelpers
    {
        public static uint ReadPackedValueUInt32(this Stream stream)
        {
            uint value = 0;
            int shift = 0;
            byte b;
            do
            {
                if (shift > 32)
                {
                    throw new InvalidOperationException();
                }

                b = stream.ReadValueU8();
                value |= (uint)(b & 0x7F) << shift;
                shift += 7;
            }
            while ((b & 0x80) == 0x80);
            return value;
        }
    }
}
