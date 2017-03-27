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
using Gibbed.Frostbite3.ResourceFormats;

namespace Gibbed.Frostbite3.UnpackResources
{
    internal static class DDSHelpers
    {
        // ReSharper disable InconsistentNaming
        public static uint GetFourCC(TextureFormat format)
            // ReSharper restore InconsistentNaming
        {
            switch (format)
            {
                case TextureFormat.BC1_UNORM:
                case TextureFormat.BC1_SRGB:
                {
                    return 0x31545844; // 'DXT1'
                }

                case TextureFormat.BC2_UNORM:
                case TextureFormat.BC2_SRGB:
                {
                    return 0x33545844; // 'DXT3'
                }

                case TextureFormat.BC3_UNORM:
                case TextureFormat.BC3_SRGB:
                {
                    return 0x35545844; // 'DXT5'
                }

                case TextureFormat.BC4_UNORM:
                {
                    return 0x31495441; // 'ATI1'
                }

                case TextureFormat.BC5_UNORM:
                {
                    return 0x32495441; // 'ATI2'
                }
            }

            throw new NotSupportedException();
        }

        // ReSharper disable InconsistentNaming
        public static uint GetDXGIFormat(TextureFormat format)
            // ReSharper restore InconsistentNaming
        {
            switch (format)
            {
                case (TextureFormat)29:
                {
                    // probably not right
                    return 67;
                }

                case (TextureFormat)64:
                {
                    return 95;
                }

                case TextureFormat.BC7_UNORM:
                {
                    return 98;
                }

                case TextureFormat.BC7_SRGB:
                {
                    return 99;
                }
            }

            throw new NotSupportedException();
        }
    }
}
