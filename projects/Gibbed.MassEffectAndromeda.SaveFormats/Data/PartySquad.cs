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
using System.ComponentModel;
using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PartySquad : INotifyPropertyChanged
    {
        #region Fields
        private readonly List<uint> _Unknown1;
        private readonly List<PartyMemberRawData> _MemberRawData;
        private readonly List<PartyMemberSnapshot> _MemberSnapshots;
        private readonly List<uint> _Unknown2;
        private readonly List<uint> _Unknown3;
        private readonly uint[] _Unknown4;
        private readonly List<PartySquadUnknown3> _Unknown5;
        private uint _Unknown6;
        private readonly List<uint> _Unknown7;
        private Transform _Unknown8;
        #endregion

        public PartySquad()
        {
            this._Unknown1 = new List<uint>();
            this._MemberRawData = new List<PartyMemberRawData>();
            this._MemberSnapshots = new List<PartyMemberSnapshot>();
            this._Unknown2 = new List<uint>();
            this._Unknown3 = new List<uint>();
            this._Unknown4 = new uint[4];
            this._Unknown5 = new List<PartySquadUnknown3>();
            this._Unknown7 = new List<uint>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public List<uint> Unknown1
        {
            get { return this._Unknown1; }
        }

        [JsonProperty("member_raw_data")]
        public List<PartyMemberRawData> MemberRawData
        {
            get { return this._MemberRawData; }
        }

        [JsonProperty("member_snapshots")]
        public List<PartyMemberSnapshot> MemberSnapshots
        {
            get { return this._MemberSnapshots; }
        }

        [JsonProperty("unknown2")]
        public List<uint> Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("unknown3")]
        public List<uint> Unknown3
        {
            get { return this._Unknown3; }
        }

        [JsonProperty("unknown4")]
        public uint[] Unknown4
        {
            get { return this._Unknown4; }
        }

        [JsonProperty("unknown5")]
        public List<PartySquadUnknown3> Unknown5
        {
            get { return this._Unknown5; }
        }

        [JsonProperty("unknown6")]
        public uint Unknown6
        {
            get { return this._Unknown6; }
            set
            {
                this._Unknown6 = value;
                this.NotifyPropertyChanged("Unknown6");
            }
        }

        [JsonProperty("unknown7")]
        public List<uint> Unknown7
        {
            get { return this._Unknown7; }
        }

        [JsonProperty("unknown8")]
        public Transform Unknown8
        {
            get { return this._Unknown8; }
            set
            {
                this._Unknown8 = value;
                this.NotifyPropertyChanged("Unknown8");
            }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            var version = reader.ReadUInt32();
            if (version > 12)
            {
                throw new SaveFormatException("unsupported version");
            }

            this._Unknown1.Clear();
            this._MemberRawData.Clear();
            this._MemberSnapshots.Clear();
            this._Unknown2.Clear();
            this._Unknown3.Clear();
            this._Unknown5.Clear();
            this._Unknown7.Clear();
            if (version >= 1)
            {
                var unknown1Count = reader.ReadUInt16();
                for (int i = 0; i < unknown1Count; i++)
                {
                    reader.PushFrameLength(24);
                    this._Unknown1.Add(reader.ReadUInt32());
                    if (version < 2)
                    {
                        throw new NotImplementedException();
                    }
                    reader.PopFrameLength();
                }

                var progressionDataCount = reader.ReadUInt16();
                for (int i = 0; i < progressionDataCount; i++)
                {
                    var progressionData = new PartyMemberRawData();
                    progressionData.Read(reader, version);
                    this._MemberRawData.Add(progressionData);
                }

                if (version >= 7)
                {
                    var progressionSnapshotCount = reader.ReadUInt16();
                    for (int i = 0; i < progressionSnapshotCount; i++)
                    {
                        var progressionSnapshot = new PartyMemberSnapshot();
                        progressionSnapshot.Read(reader, version);
                        this._MemberSnapshots.Add(progressionSnapshot);
                    }
                }

                if (version < 4)
                {
                    var unknown2Count = reader.ReadUInt16();
                    for (int i = 0; i < unknown2Count; i++)
                    {
                        this._Unknown2.Add(reader.ReadUInt32());
                    }
                    var unknown3Count = reader.ReadUInt16();
                    for (int i = 0; i < unknown3Count; i++)
                    {
                        this._Unknown3.Add(reader.ReadUInt32());
                    }
                }

                if (version >= 10)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this._Unknown4[i] = reader.ReadUInt32();
                    }
                }

                var unknown5Count = reader.ReadUInt16();
                for (int i = 0; i < unknown5Count; i++)
                {
                    var unknown5 = new PartySquadUnknown3();
                    unknown5.Read(reader, version);
                    this._Unknown5.Add(unknown5);
                }

                if (version >= 3)
                {
                    this._Unknown6 = reader.ReadUInt32();
                }

                if (version >= 8)
                {
                    var unknown7Count = reader.ReadUInt16();
                    for (int i = 0; i < unknown7Count; i++)
                    {
                        this._Unknown7.Add(reader.ReadUInt32());
                    }
                }

                if (version >= 12)
                {
                    this._Unknown8 = reader.ReadTransform();
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt32(12); // version

            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var unknown1 in this._Unknown1)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(unknown1);
                writer.PopFrameLength();
            }

            writer.WriteUInt16((ushort)this._MemberRawData.Count);
            foreach (var progressionData in this._MemberRawData)
            {
                progressionData.Write(writer);
            }

            writer.WriteUInt16((ushort)this._MemberSnapshots.Count);
            foreach (var progressionSnapshot in this._MemberSnapshots)
            {
                progressionSnapshot.Write(writer);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.WriteUInt32(this._Unknown4[i]);
            }

            writer.WriteUInt16((ushort)this._Unknown5.Count);
            foreach (var unknown5 in this._Unknown5)
            {
                unknown5.Write(writer);
            }

            writer.WriteUInt32(this._Unknown6);

            writer.WriteUInt16((ushort)this._Unknown7.Count);
            foreach (var unknown7 in this._Unknown7)
            {
                writer.WriteUInt32(unknown7);
            }

            writer.WriteTransform(this._Unknown8);
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
