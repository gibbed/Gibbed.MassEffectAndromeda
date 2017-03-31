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
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.IO;

namespace Gibbed.Frostbite3.ResourceFormats
{
    public class PartitionFile
    {
        public const uint Signature = 0x0FB2D1CE;

        public void Deserialize(Stream input)
        {
            var basePosition = input.Position;

            var magic = input.ReadValueU32(Endian.Little);
            if (magic != Signature && magic.Swap() != Signature)
            {
                throw new FormatException();
            }
            var endian = magic == Signature ? Endian.Little : Endian.Big;

            var headerSize = input.ReadValueU32(endian);
            if (basePosition + headerSize > input.Length)
            {
                throw new EndOfStreamException();
            }

            var dataSize = input.ReadValueU32(endian);
            if (basePosition + headerSize + dataSize > input.Length)
            {
                throw new EndOfStreamException();
            }

            var importCount = input.ReadValueU32(endian);
            var instanceCount = input.ReadValueU16(endian);
            var unknown12 = input.ReadValueU16(endian);
            var unknown14 = input.ReadValueU16(endian);
            var typeDefinitionCount = input.ReadValueU16(endian);
            var fieldDefinitionCount = input.ReadValueU16(endian);
            var stringTableSize = input.ReadValueU16(endian);
            var unknown1C = input.ReadValueU32(endian);
            var arrayCount = input.ReadValueU32(endian);
            var unknown24 = input.ReadValueU32(endian);
            var unknown28 = input.ReadValueGuid(endian);
            input.Seek(8, SeekOrigin.Current);

            var computedHeaderSize = 64 +
                                     (importCount * 32) +
                                     stringTableSize +
                                     (fieldDefinitionCount * 16) +
                                     (typeDefinitionCount * 16) +
                                     (instanceCount * 4).Align(16) +
                                     (arrayCount * 12).Align(16);
            if (computedHeaderSize != headerSize)
            {
                throw new FormatException();
            }

            var importEntries = new Partition.ImportEntry[importCount];
            for (int i = 0; i < importCount; i++)
            {
                importEntries[i] = Partition.ImportEntry.Read(input, endian);
            }

            var stringTableBytes = input.ReadBytes(stringTableSize);
            var strings = new List<string>();
            using (var data = new MemoryStream(stringTableBytes, false))
            {
                while (data.Position < data.Length)
                {
                    strings.Add(data.ReadStringZ(Encoding.UTF8));
                }
            }

            // TODO(gibbed): do this better.
            var hashesToStrings = strings.Distinct().ToDictionary(Common.Hashing.DJB.Compute, s => s);

            var fieldDefinitionEntries = new Partition.FieldDefinitionEntry[fieldDefinitionCount];
            for (int i = 0; i < fieldDefinitionCount; i++)
            {
                var fieldDefinitionEntry = fieldDefinitionEntries[i] = Partition.FieldDefinitionEntry.Read(input, endian);
                if (fieldDefinitionEntry.Flags.UnknownFlags2 != 0)
                {
                    throw new FormatException();
                }
                fieldDefinitionEntries[i].Name = hashesToStrings[fieldDefinitionEntries[i].NameHash];
            }

            var typeDefinitionEntries = new Partition.TypeDefinitionEntry[typeDefinitionCount];
            for (int i = 0; i < typeDefinitionCount; i++)
            {
                var typeDefinitionEntry = typeDefinitionEntries[i] = Partition.TypeDefinitionEntry.Read(input, endian);
                if (typeDefinitionEntry.Flags.UnknownFlags2 != 0)
                {
                    throw new FormatException();
                }
                typeDefinitionEntries[i].Name = hashesToStrings[typeDefinitionEntries[i].NameHash];
            }

            var instancePadding = (instanceCount * 4).Padding(16);
            var instanceEntries = new Partition.InstanceEntry[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                instanceEntries[i] = Partition.InstanceEntry.Read(input, endian);
            }
            input.Seek(instancePadding, SeekOrigin.Current);

            var arrayPadding = (arrayCount * 12).Padding(16);
            var arrayEntries = new Partition.ArrayEntry[arrayCount];
            for (int i = 0; i < arrayCount; i++)
            {
                arrayEntries[i] = Partition.ArrayEntry.Read(input, endian);
            }
            input.Seek(arrayPadding, SeekOrigin.Current);

            if (basePosition + headerSize != input.Position)
            {
                throw new FormatException();
            }

            throw new NotImplementedException();
        }
    }
}
