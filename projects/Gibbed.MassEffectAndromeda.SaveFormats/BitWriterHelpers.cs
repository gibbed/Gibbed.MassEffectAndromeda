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
using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    internal static class BitWriterHelpers
    {
        public static void WriteStringList(this IBitWriter writer, List<string> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (list.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException("too many items in string list");
            }

            writer.WriteUInt16((ushort)list.Count);
            foreach (var item in list)
            {
                writer.WriteString(item);
            }
        }

        public static void WriteStringDictionary(this IBitWriter writer, Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (dictionary.Count > ushort.MaxValue)
            {
                throw new InvalidOperationException("too many items in string dictionary");
            }

            writer.WriteUInt16((ushort)dictionary.Count);
            foreach (var kv in dictionary)
            {
                writer.PushFrameLength(24);
                writer.WriteString(kv.Key);
                writer.WriteString(kv.Value);
                writer.PopFrameLength();
            }
        }
    }
}
