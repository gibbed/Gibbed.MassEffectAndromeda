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
using Newtonsoft.Json;
using InfoManager = Gibbed.MassEffectAndromeda.GameInfo.InfoManager;
using ItemData = Gibbed.MassEffectAndromeda.SaveFormats.Items.ItemData;
using ItemDataFactory = Gibbed.MassEffectAndromeda.SaveFormats.Items.ItemDataFactory;
using ItemDefinition = Gibbed.MassEffectAndromeda.GameInfo.ItemDefinition;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerMEInventoryComponent
    [JsonObject(MemberSerialization.OptIn)]
    public class InventoryComponent
    {
        #region Fields
        private readonly List<RawItemData> _Items;
        #endregion

        public InventoryComponent()
        {
            this._Items = new List<RawItemData>();
        }

        #region Properties
        [JsonProperty("items")]
        public List<RawItemData> Items
        {
            get { return this._Items; }
        }
        #endregion

        public void Read(IBitReader reader, ushort version)
        {
            var count = reader.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                reader.PushFrameLength(24);
                RawItemData itemData;
                itemData.Id = reader.ReadUInt32();
                itemData.DataLength = (int)reader.ReadUInt32(24);
                itemData.DataBytes = reader.ReadBits(itemData.DataLength);
                this._Items.Add(itemData);
                reader.PopFrameLength();
            }
        }

        public void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._Items.Count);
            foreach (var itemData in this._Items)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(itemData.Id);
                writer.WriteUInt32((uint)itemData.DataLength, 24);
                writer.WriteBits(itemData.DataBytes, itemData.DataLength);
                writer.PopFrameLength();
            }
        }

        public struct RawItemData
        {
            public uint Id;
            public int DataLength;
            public byte[] DataBytes;
        }

        public static ItemData ReadItemData(IBitReader reader, int version)
        {
            if (version < 6)
            {
                throw new NotSupportedException();
            }

            var unknown1 = reader.ReadUInt32();
            var partitionName = reader.ReadString();

            ItemDefinition itemDefinition;
            partitionName = partitionName.ToLowerInvariant();
            if (InfoManager.Items.TryGetValue(partitionName, out itemDefinition) == false)
            {
                return null;
            }

            var itemData = ItemDataFactory.Create(itemDefinition.Type);
            itemData.Unknown1 = unknown1;
            itemData.PartitionName = partitionName;
            itemData.Read(reader, version);
            return itemData;
        }
    }
}
