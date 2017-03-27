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

namespace Gibbed.Frostbite3.VfsFormats
{
    internal static class FileLayerHelper
    {
        public static T ReadObject<T>(Stream input, Func<Stream, T> callback)
        {
            T instance = default(T);
            Read(input, s => instance = callback(s));
            return instance;
        }

        public static void Read(Stream input, Action<Stream> callback)
        {
            Stream data = input;

            if (FileLayers.OoaObfuscationLayer.IsObfuscated(data) == true)
            {
                var temp = FileLayers.OoaObfuscationLayer.Deobfuscate(data);
                if (data != input)
                {
                    data.Dispose();
                }
                data = temp;
            }

            if (FileLayers.DiceObfuscationLayer.IsObfuscated(data) == true)
            {
                var temp = FileLayers.DiceObfuscationLayer.Deobfuscate(data);
                if (data != input)
                {
                    data.Dispose();
                }
                data = temp;
            }
            else if (FileLayers.DiceSignatureLayer.IsSigned(data) == true)
            {
                FileLayers.DiceSignatureLayer.SkipSignature(data);
            }

            callback(data ?? input);

            if (data != null && data != input)
            {
                data.Dispose();
            }
        }
    }
}
