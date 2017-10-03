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
    public class LocalizedStringFile : IEnumerable<KeyValuePair<uint, string>>
    {
        public const uint Signature = 0xD78B40EB;

        #region Fields
        private Endian _Endian;
        private List<int> _Nodes;
        private Dictionary<uint, int> _Entries;
        private List<uint> _Data;
        #endregion

        public LocalizedStringFile()
        {
            this._Nodes = new List<int>();
            this._Entries = new Dictionary<uint, int>();
            this._Data = new List<uint>();
        }

        #region Properties
        public Endian Endian
        {
            get { return this._Endian; }
            set { this._Endian = value; }
        }
        #endregion

        public void Serialize(Stream output)
        {
            throw new NotSupportedException();
        }

        public void Deserialize(Stream input)
        {
            var basePosition = input.Position;
            var totalSize = input.Length - basePosition;

            var magic = input.ReadValueU32(Endian.Little);
            if (magic != Signature && magic.Swap() != Signature)
            {
                throw new FormatException();
            }
            var endian = magic == Signature ? Endian.Little : Endian.Big;

            var unknown04 = input.ReadValueU32(endian);

            var headerSize = input.ReadValueU32(endian);

            var unknown0C = input.ReadValueU32(endian);
            var unknown10 = input.ReadValueU32(endian);
            var unknown14 = input.ReadValueU32(endian);

            var nodeCount = input.ReadValueU32(endian);
            var nodeDataOffset = input.ReadValueU32(endian);
            var entryCount = input.ReadValueU32(endian);
            var entryDataOffset = input.ReadValueU32(endian);

            var unknown28Count = input.ReadValueU32(endian); // 8 bytes per entry
            var unknown28Offset = input.ReadValueU32(endian);
            var unknown30Count = input.ReadValueU32(endian); // 8 bytes per entry
            var unknown30Offset = input.ReadValueU32(endian);
            uint unknown38Count = 0;
            uint unknown38Offset = 0;
            if (unknown30Count != 0)
            {
                unknown38Count = input.ReadValueU32(endian); // 8 bytes per entry
                unknown38Offset = input.ReadValueU32(endian);
            }

            input.Position = basePosition + nodeDataOffset;
            var nodes = new int[nodeCount];
            for (uint i = 0; i < nodeCount; i++)
            {
                nodes[i] = input.ReadValueS32(endian);
            }

            var entries = new Entry[entryCount];
            if (entryCount > 0)
            {
                input.Position = basePosition + entryDataOffset;
                for (uint i = 0; i < entryCount; i++)
                {
                    Entry entry;
                    entry.Id = input.ReadValueU32(endian);
                    entry.DataIndex = input.ReadValueS32(endian);
                    entries[i] = entry;
                }
            }

            var dataCount = (totalSize - headerSize) / 4;
            var data = new uint[dataCount];
            if (dataCount > 0)
            {
                input.Position = basePosition + headerSize;
                for (long i = 0; i < dataCount; i++)
                {
                    data[i] = input.ReadValueU32(endian);
                }
            }

            this._Nodes.Clear();
            this._Entries.Clear();
            this._Data.Clear();
            this._Endian = endian;
            this._Nodes.AddRange(nodes);
            foreach (var entry in entries)
            {
                this._Entries.Add(entry.Id, entry.DataIndex);
            }
            this._Data.AddRange(data);
        }

        private struct Entry
        {
            public uint Id;
            public int DataIndex;
        }

        private string Load(int dataIndex)
        {
            var stringBuilder = new StringBuilder();
            var startPosition = (this._Nodes.Count >> 1) - 1;
            while (true)
            {
                var currentPosition = startPosition;
                int nodeIndex;
                do
                {
                    nodeIndex = (int)((this._Data[dataIndex >> 5] >> (dataIndex & 0x1F)) & 1) + 2 * currentPosition;
                    currentPosition = this._Nodes[nodeIndex];
                    dataIndex++;
                }
                while (currentPosition >= 0);
                if (currentPosition == -1)
                {
                    break;
                }
                stringBuilder.Append((char)(-1 - currentPosition));
            }
            return stringBuilder.ToString();
        }

        public string Get(uint id)
        {
            int dataIndex;
            if (this._Entries.TryGetValue(id, out dataIndex) == false)
            {
                return null;
            }
            return this.Load(dataIndex);
        }

        public IEnumerator<KeyValuePair<uint, string>> GetEnumerator()
        {
            return this._Entries.Select(e => new KeyValuePair<uint, string>(e.Key, this.Load(e.Value))).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
