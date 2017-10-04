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
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    public class SaveData : BaseSaveData
    {
        #region Fields
        private ulong _Timestamp;
        private string _LevelName;
        private uint _UserBuildInfo;
        private string _Unknown1;
        private uint _Unknown2;
        private readonly List<string> _PreloadedBundleNames;
        private readonly Dictionary<string, string> _LayerInclusion;
        private readonly List<Data.BundleHeapInfo> _BundleHeaps;
        private readonly List<Data.SaveDataUnknown0> _Unknown3;
        private Guid _Unknown4;
        private ushort _Unknown6;
        private ushort _Unknown7;
        private bool _Unknown8;
        private string _Unknown10;
        private string _Unknown11;
        private string _Unknown12;
        private uint _Unknown13;
        private byte[] _Unknown14;
        private uint _Unknown15;
        private readonly List<uint> _Unknown17;
        private readonly List<uint> _Unknown21;
        private readonly Dictionary<byte, Agent> _Agents;
        private readonly List<IComponentContainerAgent> _ComponentContainerAgents;
        private readonly List<Entities.RawEntity> _Entities;
        private ushort _Unknown22;
        #endregion

        public SaveData()
        {
            this._PreloadedBundleNames = new List<string>();
            this._LayerInclusion = new Dictionary<string, string>();
            this._BundleHeaps = new List<Data.BundleHeapInfo>();
            this._Unknown3 = new List<Data.SaveDataUnknown0>();
            this._Unknown17 = new List<uint>();
            this._Unknown21 = new List<uint>();
            this._Agents = new Dictionary<byte, Agent>();
            this._ComponentContainerAgents = new List<IComponentContainerAgent>();
            this._Entities = new List<Entities.RawEntity>();
        }

        internal override void Read(IBitReader reader)
        {
            base.Read(reader);

            this._Timestamp = reader.ReadUInt64();
            this._LevelName = reader.ReadString();
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

            this._Unknown1 = reader.ReadString();
            this._Unknown2 = reader.ReadUInt32();
            reader.ReadStringList(this._PreloadedBundleNames);
            reader.ReadStringDictionary(this._LayerInclusion);

            var bundleHeapCount = reader.ReadUInt16(12);
            this._BundleHeaps.Clear();
            for (int i = 0; i < bundleHeapCount; i++)
            {
                reader.PushFrameLength(24);
                Data.BundleHeapInfo bundleHeap;
                bundleHeap.Unknown0 = reader.ReadString();
                bundleHeap.Unknown1 = reader.ReadUInt32();
                bundleHeap.Unknown2 = reader.ReadUInt32();
                bundleHeap.Type = (Data.BundleHeapType)reader.ReadUInt8();
                bundleHeap.Unknown3 = reader.ReadUInt32();
                bundleHeap.Unknown4 = reader.ReadBoolean();
                bundleHeap.Unknown5 = reader.ReadUInt8();
                bundleHeap.Unknown6 = reader.ReadUInt8();
                bundleHeap.Unknown7 = reader.ReadUInt32();
                if (saveVersion < 17)
                {
                    reader.SkipBoolean();
                }
                this._BundleHeaps.Add(bundleHeap);
                reader.PopFrameLength();
            }

            var unknown3Count = reader.ReadUInt16(12);
            this._Unknown3.Clear();
            for (int i = 0; i < unknown3Count; i++)
            {
                reader.PushFrameLength(24);
                Data.SaveDataUnknown0 instance;
                instance.Unknown0 = reader.ReadUInt16();
                instance.Unknown1 = reader.ReadUInt16();
                instance.Unknown2 = reader.ReadUInt8();
                this._Unknown3.Add(instance);
                reader.PopFrameLength();
            }

            this._Unknown4 = reader.ReadGuid();

            var unknown5 = reader.ReadUInt32(20);
            if (unknown5 != 0)
            {
                throw new NotImplementedException();
            }
            reader.SkipBits((int)unknown5); // TODO(gibbed): position move

            this._Unknown6 = reader.ReadUInt16();
            this._Unknown7 = this._Unknown6 >= 5 ? reader.ReadUInt16() : (ushort)0;
            this._Unknown8 = reader.ReadBoolean();

            reader.PushFramePosition(26);

            var componentDataPosition = reader.ReadUInt32(26);

            var entityBulkDataLength = reader.ReadUInt32();
            var entityBulkDataPosition = reader.Position;
            reader.SkipBits((int)entityBulkDataLength);

            var unknown9 = reader.Position;

            reader.PushFrameLength(saveVersion >= 18 ? 24 : 6);
            reader.PushFrameLength(24);
            this._Unknown10 = reader.ReadString();
            this._Unknown11 = reader.ReadString();
            this._Unknown12 = reader.ReadString();
            reader.PopFrameLength();

            this._Unknown13 = reader.ReadUInt32();
            this._Unknown14 = reader.ReadBytes(16);
            this._Unknown15 = reader.ReadUInt32();
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
            this._Unknown17.Clear();
            for (int i = 0; i < unknown17Count; i++)
            {
                this._Unknown17.Add(reader.ReadUInt32());
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

            var unknown21Count = reader.ReadUInt16();
            this._Unknown21.Clear();
            for (int i = 0; i < unknown21Count; i++)
            {
                this._Unknown21[i] = reader.ReadUInt32();
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
            this._Unknown22 = reader.ReadUInt16();
            if (this._Unknown22 == 1)
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

            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 0, (r, t) => t.Read1(r, this._Unknown7));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 1, (r, t) => t.Read2(r));
            ReadAgentData(reader, this._Agents, agentDataOffsetLookup, 2, (r, t) => t.Read3(r, this._Unknown7));
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

        internal override void Write(IBitWriter writer)
        {
            base.Write(writer);

            writer.WriteUInt64(this._Timestamp);
            writer.WriteString(this._LevelName);
            writer.WriteUInt16(3); // gameVersion
            writer.WriteUInt16(22); // saveVersion
            writer.WriteUInt16(10); // ??
            writer.WriteUInt16(0); // ??
            writer.WriteUInt32(this._UserBuildInfo);
            writer.WriteString(this._Unknown1);
            writer.WriteUInt32(this._Unknown2);
            writer.WriteStringList(this._PreloadedBundleNames);
            writer.WriteStringDictionary(this._LayerInclusion);

            writer.WriteUInt16((ushort)this._BundleHeaps.Count, 12);
            foreach (var instance in this._BundleHeaps)
            {
                writer.PushFrameLength(24);
                writer.WriteString(instance.Unknown0);
                writer.WriteUInt32(instance.Unknown1);
                writer.WriteUInt32(instance.Unknown2);
                writer.WriteUInt8((byte)instance.Type);
                writer.WriteUInt32(instance.Unknown3);
                writer.WriteBoolean(instance.Unknown4);
                writer.WriteUInt8(instance.Unknown5);
                writer.WriteUInt8(instance.Unknown6);
                writer.WriteUInt32(instance.Unknown7);
                writer.PopFrameLength();
            }

            writer.WriteUInt16((ushort)this._Unknown3.Count, 12);
            foreach (var instance in this._Unknown3)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt16(instance.Unknown0);
                writer.WriteUInt16(instance.Unknown1);
                writer.WriteUInt8(instance.Unknown2);
                writer.PopFrameLength();
            }

            writer.WriteGuid(this._Unknown4);

            writer.WriteUInt32(0, 20); // unknown5

            writer.WriteUInt16(this._Unknown6);
            if (this._Unknown6 >= 5)
            {
                writer.WriteUInt16(this._Unknown7);
            }
            writer.WriteBoolean(this._Unknown8);

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
            writer.WriteString(this._Unknown10);
            writer.WriteString(this._Unknown11);
            writer.WriteString(this._Unknown12);
            writer.PopFrameLength();
            writer.WriteUInt32(this._Unknown13);
            writer.WriteBytes(this._Unknown14); // 16 bytes
            writer.WriteUInt32(this._Unknown15);
            writer.PopFrameLength();
            writer.WriteUInt32((uint)unknown9, 26);

            var agentDataOffsetLookup = new Dictionary<byte, uint[]>();
            writer.PushFrameLength(32);
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 0, (w, t) => t.Write1(w, this._Unknown7));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 1, (w, t) => t.Write2(w));
            WriteAgentData(writer, this._Agents, agentDataOffsetLookup, 2, (w, t) => t.Write3(w, this._Unknown7));
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

            writer.WriteUInt16((ushort)this._Unknown17.Count);
            foreach (var unknown17 in this._Unknown17)
            {
                writer.WriteUInt32(unknown17);
            }

            writer.WriteUInt16(0); // unknown18

            writer.WriteUInt16((ushort)this._Unknown21.Count);
            foreach (var unknown21 in this._Unknown21)
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
            writer.WriteUInt16(this._Unknown22);
            if (this._Unknown22 == 1)
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
    }
}
