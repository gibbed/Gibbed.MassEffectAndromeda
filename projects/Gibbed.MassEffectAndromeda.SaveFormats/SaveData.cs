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
using System.ComponentModel;
using System.Linq;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveData : BaseSaveData, INotifyPropertyChanged
    {
        #region Fields
        private DateTime _Timestamp;
        private string _SaveFileName;
        private uint _UserBuildInfo;
        private string _LevelName;
        private uint _Unknown1;
        private readonly List<string> _PreloadedBundleNames;
        private readonly Dictionary<string, string> _LayerInclusion;
        private readonly List<Data.BundleHeapInfo> _BundleHeaps;
        private readonly List<Data.SaveDataUnknown0> _Unknown2;
        private Guid _LevelHash;
        private ushort _Unknown3;
        private ushort _Unknown4;
        private bool _Unknown5;
        private string _Unknown6;
        private string _Unknown7;
        private string _SaveName;
        private uint _Unknown8;
        private Guid _Unknown9;
        private uint _Unknown10;
        private readonly List<uint> _Unknown11;
        private readonly List<uint> _Unknown12;
        private readonly Dictionary<byte, Agent> _Agents;
        private readonly List<IComponentContainerAgent> _ComponentContainerAgents;
        private readonly List<Entities.RawEntity> _Entities;
        private ushort _Unknown13;
        #endregion

        public string Name
        {
            get { return "SaveData"; }
        }

        public SaveData()
        {
            this._PreloadedBundleNames = new List<string>();
            this._LayerInclusion = new Dictionary<string, string>();
            this._BundleHeaps = new List<Data.BundleHeapInfo>();
            this._Unknown2 = new List<Data.SaveDataUnknown0>();
            this._Unknown11 = new List<uint>();
            this._Unknown12 = new List<uint>();
            this._Agents = new Dictionary<byte, Agent>();
            this._ComponentContainerAgents = new List<IComponentContainerAgent>();
            this._Entities = new List<Entities.RawEntity>();
        }

        #region Properties
        [JsonProperty("timestamp")]
        public DateTime Timestamp
        {
            get { return this._Timestamp; }
            set
            {
                this._Timestamp = value;
                this.NotifyPropertyChanged("Timestamp");
            }
        }

        [JsonProperty("save_file_name")]
        public string SaveFileName
        {
            get { return this._SaveFileName; }
            set
            {
                this._SaveFileName = value;
                this.NotifyPropertyChanged("SaveFileName");
            }
        }

        [JsonProperty("user_build_info")]
        public uint UserBuildInfo
        {
            get { return this._UserBuildInfo; }
            set
            {
                this._UserBuildInfo = value;
                this.NotifyPropertyChanged("UserBuildInfo");
            }
        }

        [JsonProperty("level_name")]
        public string LevelName
        {
            get { return this._LevelName; }
            set
            {
                this._LevelName = value;
                this.NotifyPropertyChanged("LevelName");
            }
        }

        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set
            {
                this._Unknown1 = value;
                this.NotifyPropertyChanged("Unknown1");
            }
        }

        [JsonProperty("preloaded_bundle_names")]
        public List<string> PreloadedBundleNames
        {
            get { return this._PreloadedBundleNames; }
        }

        [JsonProperty("layer_inclusion")]
        public Dictionary<string, string> LayerInclusion
        {
            get { return this._LayerInclusion; }
        }

        [JsonProperty("bundle_heaps")]
        public List<Data.BundleHeapInfo> BundleHeaps
        {
            get { return this._BundleHeaps; }
        }

        [JsonProperty("unknown2")]
        public List<Data.SaveDataUnknown0> Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("level_hash")]
        public Guid LevelHash
        {
            get { return this._LevelHash; }
            set
            {
                this._LevelHash = value;
                this.NotifyPropertyChanged("LevelHash");
            }
        }

        [JsonProperty("unknown3")]
        public ushort Unknown3
        {
            get { return this._Unknown3; }
            set
            {
                this._Unknown3 = value;
                this.NotifyPropertyChanged("Unknown3");
            }
        }

        [JsonProperty("unknown4")]
        public ushort Unknown4
        {
            get { return this._Unknown4; }
            set
            {
                this._Unknown4 = value;
                this.NotifyPropertyChanged("Unknown4");
            }
        }

        [JsonProperty("unknown5")]
        public bool Unknown5
        {
            get { return this._Unknown5; }
            set
            {
                this._Unknown5 = value;
                this.NotifyPropertyChanged("Unknown5");
            }
        }

        [JsonProperty("unknown6")]
        public string Unknown6
        {
            get { return this._Unknown6; }
            set
            {
                this._Unknown6 = value;
                this.NotifyPropertyChanged("Unknown6");
            }
        }

        [JsonProperty("unknown7")]
        public string Unknown7
        {
            get { return this._Unknown7; }
            set
            {
                this._Unknown7 = value;
                this.NotifyPropertyChanged("Unknown7");
            }
        }

        [JsonProperty("save_name")]
        public string SaveName
        {
            get { return this._SaveName; }
            set
            {
                this._SaveName = value;
                this.NotifyPropertyChanged("SaveName");
            }
        }

        [JsonProperty("unknown8")]
        public uint Unknown8
        {
            get { return this._Unknown8; }
            set
            {
                this._Unknown8 = value;
                this.NotifyPropertyChanged("Unknown8");
            }
        }

        [JsonProperty("unknown9")]
        public Guid Unknown9
        {
            get { return this._Unknown9; }
            set
            {
                this._Unknown9 = value;
                this.NotifyPropertyChanged("Unknown9");
            }
        }

        [JsonProperty("unknown10")]
        public uint Unknown10
        {
            get { return this._Unknown10; }
            set
            {
                this._Unknown10 = value;
                this.NotifyPropertyChanged("Unknown10");
            }
        }

        [JsonProperty("unknown11")]
        public List<uint> Unknown11
        {
            get { return this._Unknown11; }
        }

        [JsonProperty("unknown12")]
        public List<uint> Unknown12
        {
            get { return this._Unknown12; }
        }

        [JsonProperty("agents")]
        public Dictionary<byte, Agent> Agents
        {
            get { return this._Agents; }
        }

        [JsonProperty("component_container_agents")]
        public List<IComponentContainerAgent> ComponentContainerAgents
        {
            get { return this._ComponentContainerAgents; }
        }

        [JsonProperty("entities")]
        public List<Entities.RawEntity> Entities
        {
            get { return this._Entities; }
        }

        [JsonProperty("unknown13")]
        public ushort Unknown13
        {
            get { return this._Unknown13; }
            set
            {
                this._Unknown13 = value;
                this.NotifyPropertyChanged("Unknown13");
            }
        }
        #endregion

        private static DateTime ImportTimestamp(ulong timestamp)
        {
            timestamp -= 210866803200ul; // unix epoch
            timestamp *= 10000000ul; // to ticks
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime += new TimeSpan((long)timestamp);
            return dateTime.ToLocalTime();
        }

        private static ulong ExportTimestamp(DateTime dateTime)
        {
            var span = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = (ulong)(span.Ticks / 10000000);
            timestamp += 210866803200ul;
            return timestamp;
        }

        public override void Read(IBitReader reader)
        {
            base.Read(reader);

            this._Timestamp = ImportTimestamp(reader.ReadUInt64());
            this._SaveFileName = reader.ReadString();
            var gameVersion = reader.ReadUInt16();
            var saveVersion = reader.ReadUInt16();
            var unknown1 = reader.ReadUInt16(); // 10
            var unknown2 = reader.ReadUInt16(); // 0
            this._UserBuildInfo = reader.ReadUInt32();

            if (gameVersion != 3)
            {
                throw new SaveFormatException("unsupported save data game version");
            }

            if (saveVersion < 20 || saveVersion > 22)
            {
                throw new SaveFormatException("unsupported save data save version");
            }

            if (unknown1 != 10 || unknown2 != 0)
            {
                throw new SaveFormatException("unsupported save data header data");
            }

            this._LevelName = reader.ReadString();
            this._Unknown1 = reader.ReadUInt32();
            reader.ReadStringList(this._PreloadedBundleNames);
            reader.ReadStringDictionary(this._LayerInclusion);

            var bundleHeapCount = reader.ReadUInt16(12);
            this._BundleHeaps.Clear();
            for (int i = 0; i < bundleHeapCount; i++)
            {
                reader.PushFrameLength(24);
                var bundleHeap = new Data.BundleHeapInfo();
                bundleHeap.Unknown1 = reader.ReadString();
                bundleHeap.Unknown2 = reader.ReadUInt32();
                bundleHeap.Unknown3 = reader.ReadUInt32();
                bundleHeap.Type = (Data.BundleHeapType)reader.ReadUInt8();
                bundleHeap.Unknown4 = reader.ReadUInt32();
                bundleHeap.Unknown5 = reader.ReadBoolean();
                bundleHeap.Unknown6 = reader.ReadUInt8();
                bundleHeap.Unknown7 = reader.ReadUInt8();
                bundleHeap.Unknown8 = reader.ReadUInt32();
                if (saveVersion < 17)
                {
                    reader.SkipBoolean();
                }
                this._BundleHeaps.Add(bundleHeap);
                reader.PopFrameLength();
            }

            var unknown3Count = reader.ReadUInt16(12);
            this._Unknown2.Clear();
            for (int i = 0; i < unknown3Count; i++)
            {
                reader.PushFrameLength(24);
                var instance = new Data.SaveDataUnknown0();
                instance.Unknown1 = reader.ReadUInt16();
                instance.Unknown2 = reader.ReadUInt16();
                instance.Unknown3 = reader.ReadUInt8();
                this._Unknown2.Add(instance);
                reader.PopFrameLength();
            }

            // not actually a Guid, but we will borrow it for sake of simplicity
            this._LevelHash = reader.ReadGuid();

            var unknown5 = reader.ReadUInt32(20);
            if (unknown5 != 0)
            {
                throw new NotImplementedException();
            }
            reader.SkipBits((int)unknown5); // TODO(gibbed): position move

            this._Unknown3 = reader.ReadUInt16();
            this._Unknown4 = this._Unknown3 >= 5 ? reader.ReadUInt16() : (ushort)0;
            this._Unknown5 = reader.ReadBoolean();

            reader.PushFramePosition(26);

            var componentDataPosition = reader.ReadUInt32(26);

            var entityBulkDataLength = reader.ReadUInt32();
            var entityBulkDataPosition = reader.Position;
            reader.SkipBits((int)entityBulkDataLength);

            var unknown9 = reader.Position;

            reader.PushFrameLength(saveVersion >= 18 ? 24 : 6);
            reader.PushFrameLength(24);
            this._Unknown6 = reader.ReadString();
            this._Unknown7 = reader.ReadString();
            this._SaveName = reader.ReadString();
            reader.PopFrameLength();

            this._Unknown8 = reader.ReadUInt32();
            this._Unknown9 = reader.ReadGuid();
            this._Unknown10 = reader.ReadUInt32();
            reader.PopFrameLength();

            var unknown16 = reader.ReadUInt32(26);
            if (unknown9 != unknown16)
            {
                throw new FormatException();
            }

            var agentBulkDataLength = reader.ReadUInt32();
            var agentBulkDataPosition = reader.Position;
            reader.SkipBits((int)agentBulkDataLength);

            var agentDataCount = reader.ReadUInt8(4);
            var agentDataOffsetLookup = new Dictionary<byte, uint[]>();
            for (int i = 0; i < agentDataCount; i++)
            {
                var index = reader.ReadUInt8();
                var agentDataOffsets = new uint[5];
                for (int j = 0; j < 5; j++)
                {
                    var agentDataOffset = agentDataOffsets[j] = reader.ReadUInt32(26);
                    if (agentDataOffset != 0)
                    {
                        if (agentDataOffset < agentBulkDataPosition ||
                            agentDataOffset > agentBulkDataPosition + agentBulkDataLength)
                        {
                            throw new FormatException();
                        }
                    }
                }
                agentDataOffsetLookup.Add(index, agentDataOffsets);
            }

            reader.PushFrameLength(24);

            var entityDataCount = reader.ReadUInt16();
            var entityDataOffsets = new Dictionary<uint, Tuple<uint?, uint?>>();
            for (int i = 0; i < entityDataCount; i++)
            {
                var entityId = reader.ReadUInt32();
                uint? entityData0Offset = null;
                uint? entityData1Offset = null;

                var entityHasData0 = reader.ReadBoolean();
                if (entityHasData0 == true)
                {
                    var entityDataOffset = reader.ReadUInt32(26);
                    if (entityDataOffset < entityBulkDataPosition ||
                        entityDataOffset > entityBulkDataPosition + entityBulkDataLength)
                    {
                        throw new FormatException();
                    }
                    entityData0Offset = entityDataOffset;
                }

                var entityHasData1 = reader.ReadBoolean();
                if (entityHasData1 == true)
                {
                    var entityDataOffset = reader.ReadUInt32(26);
                    if (entityDataOffset < entityBulkDataPosition ||
                        entityDataOffset > entityBulkDataPosition + entityBulkDataLength)
                    {
                        throw new FormatException();
                    }
                    entityData1Offset = entityDataOffset;
                }

                entityDataOffsets.Add(entityId, new Tuple<uint?, uint?>(entityData0Offset, entityData1Offset));
            }

            var unknown17Count = reader.ReadUInt16();
            this._Unknown11.Clear();
            for (int i = 0; i < unknown17Count; i++)
            {
                this._Unknown11.Add(reader.ReadUInt32());
            }

            var unknown18Count = reader.ReadUInt16();
            for (int i = 0; i < unknown18Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown19 = reader.ReadUInt32();
                var unknown20Count = reader.ReadUInt16();
                var unknown20 = new uint[unknown20Count];
                for (int j = 0; j < unknown20Count; j++)
                {
                    unknown20[j] = reader.ReadUInt32();
                }
                reader.PopFrameLength();
                throw new NotImplementedException();
            }

            var unknown12Count = reader.ReadUInt16();
            this._Unknown12.Clear();
            for (int i = 0; i < unknown12Count; i++)
            {
                this._Unknown12.Add(reader.ReadUInt32());
            }

            reader.PopFrameLength();

            reader.PopFramePosition();

            reader.PushFrameLength(24);
            var agentCount = reader.ReadUInt8(4);
            this._Agents.Clear();
            for (int i = 0; i < agentCount; i++)
            {
                var nameHash = reader.ReadUInt32();
                var index = reader.ReadUInt8();
                var agent = AgentFactory.Create(nameHash);
                agent.Read0(reader, index);
                this._Agents.Add(index, agent);
            }
            reader.PopFrameLength();

            if (reader.FrameCount != 0)
            {
                throw new SaveFormatException();
            }

            if (reader.Position != componentDataPosition)
            {
                throw new SaveFormatException();
            }

            reader.PushFrameLength(32);
            this._ComponentContainerAgents.Clear();
            this._Unknown13 = reader.ReadUInt16();
            if (this._Unknown13 == 1)
            {
                var componentCount = reader.ReadUInt16();
                for (int i = 0; i < componentCount; i++)
                {
                    reader.PushFrameLength(24);
                    var componentNameHash = reader.ReadUInt32();
                    var componentVersion = reader.ReadUInt16();
                    var component = ComponentContainerAgentFactory.Create(componentNameHash);
                    component.Read(reader, componentVersion);
                    reader.PopFrameLength();
                    this._ComponentContainerAgents.Add(component);
                }
            }
            reader.PopFrameLength();

            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 0, (r, t) => t.Read1(r, this._Unknown4));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 1, (r, t) => t.Read2(r));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 2, (r, t) => t.Read3(r, this._Unknown4));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 3, (r, t) => t.Read4(r));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 4, (r, t) => t.Read5(r));

            /*
            var entities = new List<Entities.Entity>();
            foreach (var kv in entityDataOffsets)
            {
                var entityDataFirstOffset = kv.Value.Item1;
                var entityDataSecondOffset = kv.Value.Item2;

                var entity = Entities.EntityFactory.Create(kv.Key);

                if (entityDataFirstOffset.HasValue == true)
                {
                    reader.Position = (int)entityDataFirstOffset.Value;
                    reader.PushFrameLength(24);
                    entity.Read0(reader);
                    reader.PopFrameLength();
                }

                if (entityDataSecondOffset.HasValue == true)
                {
                    reader.Position = (int)entityDataSecondOffset.Value;
                    reader.PushFrameLength(24);
                    entity.Read1(reader);
                    reader.PopFrameLength();
                }

                entities.Add(entity);
            }
            */

            this._Entities.Clear();
            foreach (var kv in entityDataOffsets)
            {
                var entityData0Offset = kv.Value.Item1;
                var entityData1Offset = kv.Value.Item2;
                var rawEntity = new Entities.RawEntity();
                rawEntity.Id = kv.Key;
                if (entityData0Offset.HasValue == true)
                {
                    reader.Position = (int)entityData0Offset.Value;
                    rawEntity.Data0Length = (int)reader.ReadUInt32(24);
                    rawEntity.Data0Bytes = reader.ReadBits(rawEntity.Data0Length);
                }
                if (entityData1Offset.HasValue == true)
                {
                    reader.Position = (int)entityData1Offset.Value;
                    rawEntity.Data1Length = (int)reader.ReadUInt32(24);
                    rawEntity.Data1Bytes = reader.ReadBits(rawEntity.Data1Length);
                }
                this._Entities.Add(rawEntity);
            }

            if (reader.FrameCount != 0)
            {
                throw new SaveFormatException();
            }

            if (reader.HasUnreadBits() == true)
            {
                throw new SaveFormatException();
            }
        }

        public override void Write(IBitWriter writer)
        {
            base.Write(writer);

            writer.WriteUInt64(ExportTimestamp(this._Timestamp));
            writer.WriteString(this._SaveFileName);
            writer.WriteUInt16(3); // gameVersion
            writer.WriteUInt16(22); // saveVersion
            writer.WriteUInt16(10); // ??
            writer.WriteUInt16(0); // ??
            writer.WriteUInt32(this._UserBuildInfo);
            writer.WriteString(this._LevelName);
            writer.WriteUInt32(this._Unknown1);
            writer.WriteStringList(this._PreloadedBundleNames);
            writer.WriteStringDictionary(this._LayerInclusion);

            writer.WriteUInt16((ushort)this._BundleHeaps.Count, 12);
            foreach (var instance in this._BundleHeaps)
            {
                writer.PushFrameLength(24);
                writer.WriteString(instance.Unknown1);
                writer.WriteUInt32(instance.Unknown2);
                writer.WriteUInt32(instance.Unknown3);
                writer.WriteUInt8((byte)instance.Type);
                writer.WriteUInt32(instance.Unknown4);
                writer.WriteBoolean(instance.Unknown5);
                writer.WriteUInt8(instance.Unknown6);
                writer.WriteUInt8(instance.Unknown7);
                writer.WriteUInt32(instance.Unknown8);
                writer.PopFrameLength();
            }

            writer.WriteUInt16((ushort)this._Unknown2.Count, 12);
            foreach (var instance in this._Unknown2)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt16(instance.Unknown1);
                writer.WriteUInt16(instance.Unknown2);
                writer.WriteUInt8(instance.Unknown3);
                writer.PopFrameLength();
            }

            writer.WriteGuid(this._LevelHash);

            writer.WriteUInt32(0, 20); // unknown5

            writer.WriteUInt16(this._Unknown3);
            if (this._Unknown3 >= 5)
            {
                writer.WriteUInt16(this._Unknown4);
            }
            writer.WriteBoolean(this._Unknown5);

            writer.PushFramePosition(26);

            var componentDataPositionPosition = writer.Position;
            writer.WriteUInt32(0, 26);

            var entityDataOffsets = new List<Tuple<Entities.RawEntity, uint?, uint?>>();
            writer.PushFrameLength(32);
            foreach (var rawEntity in this._Entities)
            {
                uint? entityData0Offset = null;
                if (rawEntity.Data0Bytes != null)
                {
                    entityData0Offset = (uint)writer.Position;
                    writer.PushFrameLength(24);
                    writer.WriteBits(rawEntity.Data0Bytes, rawEntity.Data0Length);
                    writer.PopFrameLength();
                }
                uint? entityData1Offset = null;
                if (rawEntity.Data1Bytes != null)
                {
                    entityData1Offset = (uint)writer.Position;
                    writer.PushFrameLength(24);
                    writer.WriteBits(rawEntity.Data1Bytes, rawEntity.Data1Length);
                    writer.PopFrameLength();
                }
                entityDataOffsets.Add(
                    new Tuple<Entities.RawEntity, uint?, uint?>(
                        rawEntity,
                        entityData0Offset,
                        entityData1Offset));
            }
            writer.PopFrameLength();

            var unknown9 = writer.Position;
            writer.PushFrameLength(24);
            writer.PushFrameLength(24);
            writer.WriteString(this._Unknown6);
            writer.WriteString(this._Unknown7);
            writer.WriteString(this._SaveName);
            writer.PopFrameLength();
            writer.WriteUInt32(this._Unknown8);
            writer.WriteGuid(this._Unknown9);
            writer.WriteUInt32(this._Unknown10);
            writer.PopFrameLength();
            writer.WriteUInt32((uint)unknown9, 26);

            var agentDataOffsetLookup = new Dictionary<byte, uint[]>();
            writer.PushFrameLength(32);
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 0, (w, t) => t.Write1(w, this._Unknown4));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 1, (w, t) => t.Write2(w));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 2, (w, t) => t.Write3(w, this._Unknown4));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 3, (w, t) => t.Write4(w));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 4, (w, t) => t.Write5(w));
            writer.PopFrameLength();

            writer.WriteUInt8((byte)agentDataOffsetLookup.Count, 4);
            foreach (var kv in agentDataOffsetLookup)
            {
                writer.WriteUInt8(kv.Key);
                for (int j = 0; j < 5; j++)
                {
                    writer.WriteUInt32(kv.Value[j], 26);
                }
            }

            writer.PushFrameLength(24);

            writer.WriteUInt16((ushort)entityDataOffsets.Count);
            foreach (var tuple in entityDataOffsets)
            {
                var rawEntity = tuple.Item1;
                var entityData0Offset = tuple.Item2;
                var entityData1Offset = tuple.Item3;
                writer.WriteUInt32(rawEntity.Id);
                writer.WriteBoolean(entityData0Offset.HasValue);
                if (entityData0Offset.HasValue == true)
                {
                    writer.WriteUInt32(entityData0Offset.Value, 26);
                }
                writer.WriteBoolean(entityData1Offset.HasValue);
                if (entityData1Offset.HasValue == true)
                {
                    writer.WriteUInt32(entityData1Offset.Value, 26);
                }
            }

            writer.WriteUInt16((ushort)this._Unknown11.Count);
            foreach (var unknown17 in this._Unknown11)
            {
                writer.WriteUInt32(unknown17);
            }

            writer.WriteUInt16(0); // unknown18

            writer.WriteUInt16((ushort)this._Unknown12.Count);
            foreach (var unknown21 in this._Unknown12)
            {
                writer.WriteUInt32(unknown21);
            }

            writer.PopFrameLength();

            writer.PopFramePosition();

            writer.PushFrameLength(24);
            writer.WriteUInt8((byte)this._Agents.Count, 4);
            foreach (var kv in this._Agents)
            {
                var index = kv.Key;
                var agent = kv.Value;
                var nameHash = AgentFactory.GetNameHash(agent.AgentName);
                writer.WriteUInt32(nameHash);
                writer.WriteUInt8(index);
                agent.Write0(writer, index);
            }
            writer.PopFrameLength();

            if (writer.FrameCount != 0)
            {
                throw new SaveFormatException();
            }

            var componentDataPosition = writer.Position;
            writer.Position = componentDataPositionPosition;
            writer.WriteUInt32((uint)componentDataPosition, 26);
            writer.Position = componentDataPosition;

            writer.PushFrameLength(32);
            writer.WriteUInt16(this._Unknown13);
            if (this._Unknown13 == 1)
            {
                writer.WriteUInt16((ushort)this._ComponentContainerAgents.Count);
                foreach (var component in this._ComponentContainerAgents)
                {
                    writer.PushFrameLength(24);
                    var componentNameHash = ComponentContainerAgentFactory.GetNameHash(component.AgentName);
                    writer.WriteUInt32(componentNameHash);
                    writer.WriteUInt16(component.AgentVersion);
                    component.Write(writer);
                    writer.PopFrameLength();
                }
            }
            writer.PopFrameLength();

            if (writer.FrameCount != 0)
            {
                throw new SaveFormatException();
            }
        }

        private delegate void ReadAgentDataAction(IBitReader reader, Agent agent);

        private static void ReadAgentData(IBitReader reader,
                                          Dictionary<byte, Agent> agents,
                                          Dictionary<byte, uint[]> agentDataOffsets,
                                          byte index,
                                          ReadAgentDataAction action)
        {
            foreach (var kv in agentDataOffsets)
            {
                var agent = agents[kv.Key];
                var offsets = kv.Value;

                if (offsets[index] == 0)
                {
                    continue;
                }

                reader.Position = (int)offsets[index];

                /*
                reader.PushFrameLength(24);
                action(reader, agent);
                reader.PopFrameLength();
                */

                var length = reader.ReadUInt32(24);
                var dataPosition = reader.Position;
                var dataBytes = reader.ReadBits((int)length);
                //File.WriteAllBytes("agent_" + kv.Key + "_" + index + "_IN.bin", dataBytes);
                var dataReader = new BitReader(dataBytes, dataPosition);
                action(dataReader, agent);
            }
        }

        private delegate void WriteAgentDataAction(IBitWriter writer, Agent agent);

        private static void WriteAgentData(IBitWriter writer,
                                           Dictionary<byte, Agent> agents,
                                           Dictionary<byte, uint[]> agentDataOffsets,
                                           byte index,
                                           WriteAgentDataAction action)
        {
            foreach (var kv in agents.OrderBy(kv => kv.Key))
            {
                var agent = kv.Value;

                uint[] offsets;
                if (agentDataOffsets.TryGetValue(kv.Key, out offsets) == false)
                {
                    offsets = agentDataOffsets[kv.Key] = new uint[5];
                }

                var dataWriter = new BitWriter(0x10000, writer.Position + 24);
                action(dataWriter, agent);

                if (dataWriter.Position > 0)
                {
                    var dataBytes = dataWriter.GetBytes();

                    //File.WriteAllBytes("agent_" + kv.Key + "_" + index + "_OUT.bin", dataBytes);

                    offsets[index] = (uint)writer.Position;
                    writer.PushFrameLength(24);
                    writer.WriteBits(dataBytes, dataWriter.Position);
                    writer.PopFrameLength();
                }
            }
        }

        public T GetAgent<T>()
            where T: Agent
        {
            return this._Agents.Values.OfType<T>().FirstOrDefault();
        }

        public T GetComponentContainerAgent<T>()
            where T: IComponentContainerAgent
        {
            return this._ComponentContainerAgents.OfType<T>().FirstOrDefault();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
