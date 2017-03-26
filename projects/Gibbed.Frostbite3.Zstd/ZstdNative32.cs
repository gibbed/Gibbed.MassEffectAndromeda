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
using System.Runtime.InteropServices;

namespace Gibbed.Frostbite3
{
    internal static class ZstdNative32
    {
        public const string DllName = "zstd32";
        public const CallingConvention Cdecl = CallingConvention.Cdecl;

        [DllImport(DllName, EntryPoint = "ZSTD_isError", CallingConvention = Cdecl)]
        public static extern uint IsError(uint code);

        [DllImport(DllName, EntryPoint = "ZSTD_findDecompressedSize", CallingConvention = Cdecl)]
        public static extern ulong FindDecompressedSize(IntPtr inputBuffer, uint inputSize);

        [DllImport(DllName, EntryPoint = "ZSTD_getFrameContentSize", CallingConvention = Cdecl)]
        public static extern ulong GetFrameContentSize(IntPtr inputBuffer, uint inputSize);

        [DllImport(DllName, EntryPoint = "ZSTD_decompress", CallingConvention = Cdecl)]
        public static extern uint Decompress(IntPtr outputBuffer,
                                             uint outputSize,
                                             IntPtr inputBuffer,
                                             uint inputSize);
    }
}
