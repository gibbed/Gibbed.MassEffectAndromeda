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

using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    // ServerLootContainerEntity
    [JsonObject(MemberSerialization.OptIn)]
    [Agent(_AgentName)]
    public class LootContainerAgent : Agent
    {
        private const string _AgentName = "ServerLootContainerSaveGameAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly List<Data.LootContainer> _LootContainers;
        private readonly List<KeyValuePair<uint, uint>> _Unknown1;
        #endregion

        public LootContainerAgent()
            : base(3)
        {
            this._LootContainers = new List<Data.LootContainer>();
            this._Unknown1 = new List<KeyValuePair<uint, uint>>();
        }

        #region Properties
        [JsonProperty("loot_containers")]
        public List<Data.LootContainer> LootContainers
        {
            get { return this._LootContainers; }
        }

        [JsonProperty("unknown1")]
        public List<KeyValuePair<uint, uint>> Unknown1
        {
            get { return this._Unknown1; }
        }
        #endregion

        internal override void Read2(IBitReader reader)
        {
            base.Read2(reader);
            this._LootContainers.Clear();
            var lootContainerCount = reader.ReadUInt16();
            for (int i = 0; i < lootContainerCount; i++)
            {
                var lootContainer = new Data.LootContainer();
                lootContainer.Read(reader);
                this._LootContainers.Add(lootContainer);
            }
            this._Unknown1.Clear();
            var unknown1Count = reader.ReadUInt16();
            for (int i = 0; i < unknown1Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown1Key = reader.ReadUInt32();
                var unknown1Value = reader.ReadUInt32();
                reader.PopFrameLength();
                this._Unknown1.Add(new KeyValuePair<uint, uint>(unknown1Key, unknown1Value));
            }
        }

        internal override void Write2(IBitWriter writer)
        {
            base.Write2(writer);
            writer.WriteUInt16((ushort)this._LootContainers.Count);
            foreach (var lootContainer in this._LootContainers)
            {
                lootContainer.Write(writer);
            }
            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var kv in this._Unknown1)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32(kv.Value);
                writer.PopFrameLength();
            }
        }
    }
}
