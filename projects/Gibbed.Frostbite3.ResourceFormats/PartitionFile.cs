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

        #region Fields
        private Endian _Endian;
        private Guid _Guid;
        private readonly List<Partition.ImportEntry> _ImportEntries;
        private byte[] _NameTableBytes;
        private readonly List<Partition.FieldDefinitionEntry> _FieldDefinitionEntries;
        private readonly List<Partition.TypeDefinitionEntry> _TypeDefinitionEntries;
        private readonly List<Partition.InstanceEntry> _InstanceEntries;
        private readonly List<Partition.ArrayEntry> _ArrayEntries;
        private ushort _NamedInstanceCount;
        private ushort _ExportedInstanceCount;
        private uint _StringTableSize;
        private uint _ArrayOffset;
        private byte[] _DataBytes;
        #endregion

        public PartitionFile()
        {
            this._ImportEntries = new List<Partition.ImportEntry>();
            this._FieldDefinitionEntries = new List<Partition.FieldDefinitionEntry>();
            this._TypeDefinitionEntries = new List<Partition.TypeDefinitionEntry>();
            this._InstanceEntries = new List<Partition.InstanceEntry>();
            this._ArrayEntries = new List<Partition.ArrayEntry>();
        }

        #region Properties
        public Endian Endian
        {
            get { return this._Endian; }
            set { this._Endian = value; }
        }

        public Guid Guid
        {
            get { return this._Guid; }
            set { this._Guid = value; }
        }

        public List<Partition.ImportEntry> ImportEntries
        {
            get { return this._ImportEntries; }
        }

        public byte[] NameTableBytes
        {
            get { return this._NameTableBytes; }
            set { this._NameTableBytes = value; }
        }

        public List<Partition.FieldDefinitionEntry> FieldDefinitionEntries
        {
            get { return this._FieldDefinitionEntries; }
        }

        public List<Partition.TypeDefinitionEntry> TypeDefinitionEntries
        {
            get { return this._TypeDefinitionEntries; }
        }

        public List<Partition.InstanceEntry> InstanceEntries
        {
            get { return this._InstanceEntries; }
        }

        public List<Partition.ArrayEntry> ArrayEntries
        {
            get { return this._ArrayEntries; }
        }

        public ushort NamedInstanceCount
        {
            get { return this._NamedInstanceCount; }
            set { this._NamedInstanceCount = value; }
        }

        public ushort ExportedInstanceCount
        {
            get { return this._ExportedInstanceCount; }
            set { this._ExportedInstanceCount = value; }
        }

        public uint StringTableSize
        {
            get { return this._StringTableSize; }
            set { this._StringTableSize = value; }
        }

        public uint ArrayOffset
        {
            get { return this._ArrayOffset; }
            set { this._ArrayOffset = value; }
        }

        public byte[] DataBytes
        {
            get { return this._DataBytes; }
            set { this._DataBytes = value; }
        }
        #endregion

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
            var namedInstanceCount = input.ReadValueU16(endian);
            var exportedInstanceCount = input.ReadValueU16(endian);
            var typeDefinitionCount = input.ReadValueU16(endian);
            var fieldDefinitionCount = input.ReadValueU16(endian);
            var nameTableSize = input.ReadValueU16(endian);
            var stringTableSize = input.ReadValueU32(endian);
            var arrayCount = input.ReadValueU32(endian);
            var arrayOffset = input.ReadValueU32(endian);
            var guid = input.ReadValueGuid(endian);
            input.Seek(8, SeekOrigin.Current);

            var computedHeaderSize = 64 +
                                     (importCount * 32) +
                                     nameTableSize +
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

            var nameTableBytes = input.ReadBytes(nameTableSize);
            var names = new List<string>();
            using (var data = new MemoryStream(nameTableBytes, false))
            {
                while (data.Position < data.Length)
                {
                    names.Add(data.ReadStringZ(Encoding.UTF8));
                }
            }

            // TODO(gibbed): do this better.
            var hashesToNames = names.Distinct().ToDictionary(Common.Hashing.DJB.Compute, s => s);

            var fieldDefinitionEntries = new Partition.FieldDefinitionEntry[fieldDefinitionCount];
            for (int i = 0; i < fieldDefinitionCount; i++)
            {
                var fieldDefinitionEntry =
                    fieldDefinitionEntries[i] = Partition.FieldDefinitionEntry.Read(input, endian);
                if (fieldDefinitionEntry.Flags.UnknownFlags2 != 0)
                {
                    throw new FormatException();
                }
                fieldDefinitionEntries[i].Name = hashesToNames[fieldDefinitionEntries[i].NameHash];
            }

            var typeDefinitionEntries = new Partition.TypeDefinitionEntry[typeDefinitionCount];
            for (int i = 0; i < typeDefinitionCount; i++)
            {
                var typeDefinitionEntry =
                    typeDefinitionEntries[i] = Partition.TypeDefinitionEntry.Read(input, endian);
                if (typeDefinitionEntry.Flags.UnknownFlags2 != 0)
                {
                    throw new FormatException();
                }
                typeDefinitionEntries[i].Name = hashesToNames[typeDefinitionEntries[i].NameHash];
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

            var dataBytes = input.ReadBytes(dataSize);

            this._ImportEntries.Clear();
            this._FieldDefinitionEntries.Clear();
            this._TypeDefinitionEntries.Clear();
            this._InstanceEntries.Clear();
            this._ArrayEntries.Clear();

            this._Endian = endian;
            this._Guid = guid;
            this._ImportEntries.AddRange(importEntries);
            this._NameTableBytes = nameTableBytes;
            this._FieldDefinitionEntries.AddRange(fieldDefinitionEntries);
            this._TypeDefinitionEntries.AddRange(typeDefinitionEntries);
            this._InstanceEntries.AddRange(instanceEntries);
            this._ArrayEntries.AddRange(arrayEntries);
            this._NamedInstanceCount = namedInstanceCount;
            this._ExportedInstanceCount = exportedInstanceCount;
            this._StringTableSize = stringTableSize;
            this._ArrayOffset = arrayOffset;
            this._DataBytes = dataBytes;
        }
    }
}
