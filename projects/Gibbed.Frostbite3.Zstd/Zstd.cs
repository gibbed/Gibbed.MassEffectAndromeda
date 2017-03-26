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
    public class Zstd
    {
        private static readonly bool _Is64Bit;

        static Zstd()
        {
            _Is64Bit = Marshal.SizeOf(IntPtr.Zero) == 8;
        }

        public static bool IsError(ulong code)
        {
            return (_Is64Bit == true ? ZstdNative64.IsError(code) : ZstdNative32.IsError((uint)code)) != 0;
        }

        public static ulong FindDecompressedSize(byte[] inputBytes, int inputOffset, int inputCount)
        {
            if (inputBytes == null)
            {
                throw new ArgumentNullException("inputBytes");
            }

            if (inputOffset < 0 || inputOffset >= inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputOffset");
            }

            if (inputCount <= 0 || inputOffset + inputCount > inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputCount");
            }

            ulong result;
            var inputHandle = GCHandle.Alloc(inputBytes, GCHandleType.Pinned);
            var inputAddress = inputHandle.AddrOfPinnedObject() + inputOffset;
            result = _Is64Bit == true
                         ? ZstdNative64.FindDecompressedSize(inputAddress, (ulong)inputCount)
                         : ZstdNative32.FindDecompressedSize(inputAddress, (uint)inputCount);
            inputHandle.Free();
            return result;
        }

        public static ulong GetFrameContentSize(byte[] inputBytes, int inputOffset, int inputCount)
        {
            if (inputBytes == null)
            {
                throw new ArgumentNullException("inputBytes");
            }

            if (inputOffset < 0 || inputOffset >= inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputOffset");
            }

            if (inputCount <= 0 || inputOffset + inputCount > inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputCount");
            }

            ulong result;
            var inputHandle = GCHandle.Alloc(inputBytes, GCHandleType.Pinned);
            var inputAddress = inputHandle.AddrOfPinnedObject() + inputOffset;
            result = _Is64Bit == true
                         ? ZstdNative64.GetFrameContentSize(inputAddress, (ulong)inputCount)
                         : ZstdNative32.GetFrameContentSize(inputAddress, (uint)inputCount);
            inputHandle.Free();
            return result;
        }

        public static ulong Decompress(byte[] inputBytes,
                                       int inputOffset,
                                       int inputCount,
                                       byte[] outputBytes,
                                       int outputOffset,
                                       int outputCount)
        {
            if (inputBytes == null)
            {
                throw new ArgumentNullException("inputBytes");
            }

            if (inputOffset < 0 || inputOffset >= inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputOffset");
            }

            if (inputCount <= 0 || inputOffset + inputCount > inputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("inputCount");
            }

            if (outputBytes == null)
            {
                throw new ArgumentNullException("outputBytes");
            }

            if (outputOffset < 0 || outputOffset >= outputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("outputOffset");
            }

            if (outputCount <= 0 || outputOffset + outputCount > outputBytes.Length)
            {
                throw new ArgumentOutOfRangeException("outputCount");
            }

            ulong result;
            var outputHandle = GCHandle.Alloc(outputBytes, GCHandleType.Pinned);
            var outputAddress = outputHandle.AddrOfPinnedObject() + outputOffset;
            var inputHandle = GCHandle.Alloc(inputBytes, GCHandleType.Pinned);
            var inputAddress = inputHandle.AddrOfPinnedObject() + inputOffset;
            result = _Is64Bit == true
                         ? ZstdNative64.Decompress(outputAddress, (ulong)outputCount, inputAddress, (ulong)inputCount)
                         : ZstdNative32.Decompress(outputAddress, (uint)outputCount, inputAddress, (uint)inputCount);
            inputHandle.Free();
            outputHandle.Free();
            return result;
        }
    }
}
