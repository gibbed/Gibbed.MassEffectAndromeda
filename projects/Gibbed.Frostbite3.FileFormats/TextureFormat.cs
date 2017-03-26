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

namespace Gibbed.Frostbite3.FileFormats
{
    public enum TextureFormat : uint
    {
        // ReSharper disable InconsistentNaming
        Invalid = 0,
        R8_UNORM = 6,
        R8G8B8A8_UNORM = 18,
        R10G10B10A2_UNORM = 27,
        R9G9B9E5_FLOAT = 29,
        R16G16B16A16_FLOAT = 40,
        R32G32B32A32_FLOAT = 51,
        BC1_UNORM = 54,
        BC1_SRGB = 55,
        BC2_UNORM = 58,
        BC2_SRGB = 59,
        BC3_UNORM = 60,
        BC3_SRGB = 61,
        BC4_UNORM = 62,
        BC5_UNORM = 63,
        BC6U_FLOAT = 64,
        BC7_UNORM = 66,
        BC7_SRGB = 67,
        // ReSharper restore InconsistentNaming
    }
}
