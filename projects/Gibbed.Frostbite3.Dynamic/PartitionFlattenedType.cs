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
using System.Linq;
using Gibbed.Frostbite3.ResourceFormats;
using Partition = Gibbed.Frostbite3.ResourceFormats.Partition;

namespace Gibbed.Frostbite3.Dynamic
{
    internal class PartitionFlattenedType
    {
        private readonly int _Index;
        private readonly Partition.FieldDefinitionEntry[] _Fields;

        public PartitionFlattenedType(PartitionFile partition, int typeIndex)
        {
            if (partition == null)
            {
                throw new ArgumentNullException("partition");
            }

            if (typeIndex < 0 || typeIndex >= partition.TypeDefinitionEntries.Count)
            {
                throw new ArgumentOutOfRangeException("typeIndex");
            }

            this._Index = typeIndex;
            this._Fields = FlattenFields(partition, typeIndex).ToArray();
        }

        public int Index
        {
            get { return this._Index; }
        }

        public Partition.FieldDefinitionEntry[] Fields
        {
            get { return this._Fields; }
        }

        private static IEnumerable<Partition.FieldDefinitionEntry> FlattenFields(PartitionFile partition,
                                                                                 int typeIndex)
        {
            const uint baseTypeNameHash = 0x0002B581; // hash of "$"

            var fields = new LinkedList<Partition.FieldDefinitionEntry>();

            var typeDefinition = partition.TypeDefinitionEntries[typeIndex];
            for (int i = typeDefinition.FieldStartIndex, o = 0; o < typeDefinition.FieldCount; i++, o++)
            {
                fields.AddLast(partition.FieldDefinitionEntries[i]);
            }

            var current = fields.First;
            while (current != null)
            {
                var parentField = current.Value;
                if (parentField.NameHash != baseTypeNameHash ||
                    parentField.Flags.DataType != Partition.DataType.Void ||
                    parentField.Flags.DataKind != Partition.DataKind.None ||
                    parentField.Flags.DataFlags != Partition.DataFlags.None)
                {
                    current = current.Next;
                    continue;
                }

                typeDefinition = partition.TypeDefinitionEntries[parentField.TypeIndex];
                for (int i = typeDefinition.FieldStartIndex, o = 0; o < typeDefinition.FieldCount; i++, o++)
                {
                    fields.AddBefore(current, partition.FieldDefinitionEntries[i]);
                }
                fields.Remove(current);
                current = fields.First;
            }

            foreach (var field in fields)
            {
                yield return field;
            }
        }
    }
}
