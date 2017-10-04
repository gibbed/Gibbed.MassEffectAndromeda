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
    internal static class BitReaderHelpers
    {
        public static void ReadStringList(this IBitReader reader, List<string> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            list.Clear();
            var count = reader.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadString());
            }
        }

        public static void ReadStringDictionary(this IBitReader reader, Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            dictionary.Clear();
            var count = reader.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                reader.PushFrameLength(24);
                var key = reader.ReadString();
                var value = reader.ReadString();
                reader.PopFrameLength();
                dictionary.Add(key, value);
            }
        }
    }
}
