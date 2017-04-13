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
using System.Reflection;
using System.Text;
using Gibbed.Frostbite3.ResourceFormats;
using Gibbed.IO;
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;
using Partition = Gibbed.Frostbite3.ResourceFormats.Partition;

namespace Gibbed.Frostbite3.Dynamic
{
    public class PartitionReader : IDisposable
    {
        private readonly PartitionFile _Partition;
        private readonly Dictionary<int, Type> _EnumTypes;
        private readonly object _DataLock;
        private readonly MemoryStream _Data;
        private readonly PartitionFlattenedType[] _FlattenedTypes;
        private readonly PartitionInstance[] _Instances;

        public PartitionReader(PartitionFile partition, params Type[] enumTypes)
        {
            if (partition == null)
            {
                throw new ArgumentNullException("partition");
            }

            this._Partition = partition;
            this._EnumTypes = this.DiscoverEnumTypes(enumTypes);
            this._DataLock = new object();
            this._Data = this._Partition.DataBytes == null ? null : new MemoryStream(this._Partition.DataBytes, false);
            this._FlattenedTypes = this.FlattenTypes().ToArray();
            this._Instances = this.CreateInstances().ToArray();
        }

        ~PartitionReader()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this._Data != null)
                {
                    this._Data.Dispose();
                }
            }
        }

        private Dictionary<int, Type> DiscoverEnumTypes(Type[] types)
        {
            if (types == null)
            {
                return null;
            }

            var lookup = new Dictionary<int, Type>();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttributes(typeof(PartitionEnumAttribute), false)
                                    .OfType<PartitionEnumAttribute>()
                                    .FirstOrDefault();
                if (attribute == null)
                {
                    throw new InvalidOperationException(type + " is not attributed with PartitionEnumAttribute");
                }

                if ((attribute.Options & PartitionEnumOptions.Validate) != 0)
                {
                    var typeNameHash = DJB.Compute(attribute.TypeName);
                    var typeIndex = this._Partition.TypeDefinitionEntries.FindIndex(td => td.NameHash == typeNameHash);
                    if (typeIndex >= 0)
                    {
                        var typeDefinition = this._Partition.TypeDefinitionEntries[typeIndex];
                        ValidateEnumType(type, typeDefinition);
                        lookup.Add(typeIndex, type);
                    }
                }
            }
            return lookup;
        }

        private void ValidateEnumType(Type type, Partition.TypeDefinitionEntry typeDefinition)
        {
            var fieldNames = new Dictionary<uint, string>();
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = fieldInfo.GetCustomAttributes(typeof(PartitionEnumMemberAttribute), false)
                                         .OfType<PartitionEnumMemberAttribute>()
                                         .FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }
                fieldNames.Add((uint)fieldInfo.GetRawConstantValue(), attribute.MemberName);
            }

            for (int i = typeDefinition.FieldStartIndex, o = 0; o < typeDefinition.FieldCount; i++, o++)
            {
                var fieldDefinition = this._Partition.FieldDefinitionEntries[i];
                string fieldName;
                if (fieldNames.TryGetValue(fieldDefinition.DataOffset, out fieldName) == false)
                {
                    throw new InvalidOperationException("enum " + type + " does not have a member for " +
                                                        fieldDefinition.DataOffset);
                }
                if (fieldName != fieldDefinition.Name)
                {
                    throw new InvalidOperationException("enum " + type + " does has the wrong member name for " +
                                                        fieldDefinition.DataOffset + " (" + fieldDefinition.Name +
                                                        " vs " + fieldName + ")");
                }
            }
        }

        private IEnumerable<PartitionFlattenedType> FlattenTypes()
        {
            for (int i = 0; i < this._Partition.TypeDefinitionEntries.Count; i++)
            {
                yield return new PartitionFlattenedType(this._Partition, i);
            }
        }

        private IEnumerable<PartitionInstance> CreateInstances()
        {
            const int dataOffsetDelta = -8;

            var endian = this._Partition.Endian;
            long offset = (int)this._Partition.StringTableSize;
            for (int i = 0; i < this._Partition.InstanceEntries.Count; i++)
            {
                var instance = this._Partition.InstanceEntries[i];
                var typeDefintiion = this._Partition.TypeDefinitionEntries[instance.TypeIndex];
                var typeSize = (typeDefintiion.DataSize + dataOffsetDelta).Align(typeDefintiion.Alignment);
                var flattenedType = this._FlattenedTypes[instance.TypeIndex];
                for (int j = 0; j < instance.Count; j++)
                {
                    offset = offset.Align(typeDefintiion.Alignment);

                    var guid = Guid.Empty;
                    if (i < this._Partition.NamedInstanceCount)
                    {
                        this._Data.Position = offset;
                        guid = this._Data.ReadValueGuid(endian);
                        offset += 16;
                    }

                    yield return new PartitionInstance(this, guid, offset + dataOffsetDelta, flattenedType);
                    offset += typeSize;
                }
            }
        }

        public dynamic GetObject(int instanceIndex)
        {
            if (instanceIndex < 0 || instanceIndex >= this._Partition.InstanceEntries.Count)
            {
                throw new ArgumentOutOfRangeException("instanceIndex");
            }
            return this._Instances[instanceIndex].ToObject();
        }

        public dynamic GetObject(Guid instanceGuid)
        {
            var instance = this._Instances.FirstOrDefault(i => i.Guid == instanceGuid);
            return instance == null ? null : instance.ToObject();
        }

        public IEnumerable<dynamic> GetObjects()
        {
            return this._Instances.Select(i => i.ToObject());
        }

        public IEnumerable<dynamic> GetObjectsOfType(uint typeNameHash)
        {
            var typeIndex = this._Partition.TypeDefinitionEntries.FindIndex(tde => tde.NameHash == typeNameHash);
            if (typeIndex < 0)
            {
                throw new ArgumentException("could not find type 0x" + typeNameHash.ToString("X8") + " in partition",
                                            "typeNameHash");
            }
            return this._Instances.Where(i => i.Type.Index == typeIndex).Select(i => i.ToObject());
        }

        public IEnumerable<dynamic> GetObjectsOfType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName) == true)
            {
                throw new ArgumentNullException("typeName");
            }

            var typeNameHash = DJB.Compute(typeName);
            var typeIndex = this._Partition.TypeDefinitionEntries.FindIndex(tde => tde.NameHash == typeNameHash);
            if (typeIndex < 0)
            {
                throw new ArgumentException("could not find type '" + typeName + "' in partition", "typeName");
            }
            return this._Instances.Where(i => i.Type.Index == typeIndex).Select(i => i.ToObject());
        }

        internal object ReadField(long dataOffset, Partition.FieldDefinitionEntry field)
        {
            lock (this._DataLock)
            {
                this._Data.Position = dataOffset + field.DataOffset;
                return this.ReadData(field.TypeIndex, field.Flags);
            }
        }

        private object ReadData(int typeIndex, Partition.DefinitionFlags flags)
        {
            var endian = this._Partition.Endian;
            switch (flags.DataType)
            {
                case Partition.DataType.Boolean:
                {
                    return this._Data.ReadValueB8();
                }

                case Partition.DataType.Int8:
                {
                    return this._Data.ReadValueS8();
                }

                case Partition.DataType.UInt8:
                {
                    return this._Data.ReadValueU8();
                }

                case Partition.DataType.Int16:
                {
                    return this._Data.ReadValueS16(endian);
                }

                case Partition.DataType.UInt16:
                {
                    return this._Data.ReadValueU16(endian);
                }

                case Partition.DataType.Int32:
                {
                    return this._Data.ReadValueS32(endian);
                }

                case Partition.DataType.UInt32:
                {
                    return this._Data.ReadValueU32(endian);
                }

                case Partition.DataType.Float32:
                {
                    return this._Data.ReadValueF32(endian);
                }

                case Partition.DataType.String2:
                {
                    var stringOffset = this._Data.ReadValueU32(endian);
                    if (stringOffset == 0xFFFFFFFFu)
                    {
                        return null;
                    }
                    this._Data.Position = stringOffset;
                    return this._Data.ReadStringZ(Encoding.UTF8);
                }

                case Partition.DataType.Class:
                {
                    var value = this._Data.ReadValueU32(endian);
                    if (value == 0)
                    {
                        return null;
                    }
                    if ((value & 0x80000000u) != 0)
                    {
                        value &= ~0x80000000u;
                        return this._Partition.ImportEntries[(int)value];
                    }
                    return this._Instances[(int)value - 1].ToObject();
                }

                case Partition.DataType.Value:
                {
                    var type = this._FlattenedTypes[typeIndex];
                    return new PartitionInstance(this, Guid.Empty, this._Data.Position, type).ToObject();
                }

                case Partition.DataType.Enum:
                {
                    var typeDefinition = this._Partition.TypeDefinitionEntries[typeIndex];
                    object value;
                    switch (typeDefinition.DataSize)
                    {
                        case 4:
                        {
                            value = this._Data.ReadValueU32(endian);
                            break;
                        }

                        default:
                        {
                            throw new NotSupportedException("unsupported enum size");
                        }
                    }

                    Type enumType;
                    if (this._EnumTypes == null || this._EnumTypes.TryGetValue(typeIndex, out enumType) == false)
                    {
                        return value;
                    }
                    return Enum.ToObject(enumType, Convert.ToUInt32(value));

                    /*
                    for (int i = typeDefinition.FieldStartIndex, o = 0; o < typeDefinition.FieldCount; i++, o++)
                    {
                        var fieldDefinition = this._Partition.FieldDefinitionEntries[i];
                        if (fieldDefinition.DataOffset == value)
                        {
                            return fieldDefinition.Name;
                        }
                    }
                    return null;
                    */
                }

                case Partition.DataType.List:
                {
                    var arrayIndex = this._Data.ReadValueU32(endian);
                    if (arrayIndex == 0xFFFFFFFFu)
                    {
                        return null;
                    }
                    var arrayEntry = this._Partition.ArrayEntries[(int)arrayIndex];
                    var arrayTypeDefinition = this._Partition.TypeDefinitionEntries[arrayEntry.TypeIndex];
                    if (arrayTypeDefinition.DataSize != 4)
                    {
                        throw new InvalidOperationException("array should have a size of 4");
                    }
                    if (arrayTypeDefinition.FieldCount != 1)
                    {
                        throw new InvalidOperationException("array should only have one member");
                    }
                    var arrayMemberField = this._Partition.FieldDefinitionEntries[arrayTypeDefinition.FieldStartIndex];
                    var arrayMemberSize = this.GetFieldDefinitionSize(arrayTypeDefinition.FieldStartIndex);
                    var items = new dynamic[arrayEntry.ItemCount];
                    for (int i = 0; i < arrayEntry.ItemCount; i++)
                    {
                        this._Data.Position = this._Partition.StringTableSize +
                                              this._Partition.ArrayOffset +
                                              arrayEntry.DataOffset + (i * arrayMemberSize);
                        items[i] = this.ReadData(arrayMemberField.TypeIndex, arrayMemberField.Flags);
                    }
                    return items;
                }
            }

            throw new NotSupportedException("unsupported data type " + flags.DataType);
        }

        private int GetFieldDefinitionSize(int fieldIndex)
        {
            var fieldDefinition = this._Partition.FieldDefinitionEntries[fieldIndex];
            switch (fieldDefinition.Flags.DataType)
            {
                case Partition.DataType.Boolean:
                case Partition.DataType.Int8:
                case Partition.DataType.UInt8:
                {
                    return 1;
                }

                case Partition.DataType.Int16:
                case Partition.DataType.UInt16:
                {
                    return 2;
                }

                case Partition.DataType.Int32:
                case Partition.DataType.UInt32:
                case Partition.DataType.Float32:
                {
                    return 4;
                }

                case Partition.DataType.String2:
                {
                    return 4;
                }

                case Partition.DataType.Class:
                {
                    return 4;
                }

                case Partition.DataType.Value:
                {
                    return this._Partition.TypeDefinitionEntries[fieldDefinition.TypeIndex].DataSize;
                }

                case Partition.DataType.Enum:
                {
                    return this._Partition.TypeDefinitionEntries[fieldDefinition.TypeIndex].DataSize;
                }

                case Partition.DataType.List:
                {
                    return 4;
                }
            }

            throw new NotSupportedException("unsupported data type for get size");
        }
    }
}
