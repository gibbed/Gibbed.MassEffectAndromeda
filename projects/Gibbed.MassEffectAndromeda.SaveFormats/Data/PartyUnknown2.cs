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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class PartyUnknown2
    {
        private uint _Version;
        private readonly List<uint> _Unknown1;
        private readonly List<PartyProgressionData> _ProgressionData;
        private readonly List<PartyProgressionSnapshot> _ProgressionSnapshots;
        private readonly List<uint> _Unknown2;
        private readonly List<uint> _Unknown3;
        private readonly uint[] _Unknown4;
        private readonly List<PartyUnknown3> _Unknown5;
        private uint _Unknown6;
        private readonly List<uint> _Unknown7;
        private Transform _Unknown8;

        public PartyUnknown2()
        {
            this._Unknown1 = new List<uint>();
            this._ProgressionData = new List<PartyProgressionData>();
            this._ProgressionSnapshots = new List<PartyProgressionSnapshot>();
            this._Unknown2 = new List<uint>();
            this._Unknown3 = new List<uint>();
            this._Unknown4 = new uint[4];
            this._Unknown5 = new List<PartyUnknown3>();
            this._Unknown7 = new List<uint>();
        }

        internal void Read(IBitReader reader)
        {
            this._Version = reader.ReadUInt32();
            this._Unknown1.Clear();
            this._ProgressionData.Clear();
            this._ProgressionSnapshots.Clear();
            this._Unknown2.Clear();
            this._Unknown3.Clear();
            this._Unknown5.Clear();
            this._Unknown7.Clear();
            if (this._Version >= 1)
            {
                var unknown1Count = reader.ReadUInt16();
                for (int i = 0; i < unknown1Count; i++)
                {
                    reader.PushFrameLength(24);
                    this._Unknown1.Add(reader.ReadUInt32());
                    if (this._Version < 2)
                    {
                        throw new NotImplementedException();
                    }
                    reader.PopFrameLength();
                }

                var progressionDataCount = reader.ReadUInt16();
                for (int i = 0; i < progressionDataCount; i++)
                {
                    var progressionData = new PartyProgressionData();
                    progressionData.Read(reader, this._Version);
                    this._ProgressionData.Add(progressionData);
                }

                if (this._Version >= 7)
                {
                    var progressionSnapshotCount = reader.ReadUInt16();
                    for (int i = 0; i < progressionSnapshotCount; i++)
                    {
                        var progressionSnapshot = new PartyProgressionSnapshot();
                        progressionSnapshot.Read(reader, this._Version);
                        this._ProgressionSnapshots.Add(progressionSnapshot);
                    }
                }

                if (this._Version < 4)
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

                if (this._Version >= 10)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        this._Unknown4[i] = reader.ReadUInt32();
                    }
                }

                var unknown5Count = reader.ReadUInt16();
                for (int i = 0; i < unknown5Count; i++)
                {
                    var unknown5 = new PartyUnknown3();
                    unknown5.Read(reader, this._Version);
                    this._Unknown5.Add(unknown5);
                }

                if (this._Version >= 3)
                {
                    this._Unknown6 = reader.ReadUInt32();
                }

                if (this._Version >= 8)
                {
                    var unknown7Count = reader.ReadUInt16();
                    for (int i = 0; i < unknown7Count; i++)
                    {
                        this._Unknown7.Add(reader.ReadUInt32());
                    }
                }

                if (this._Version >= 12)
                {
                    this._Unknown8 = reader.ReadTransform();
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt32(this._Version);
            if (this._Version >= 1)
            {
                writer.WriteUInt16((ushort)this._Unknown1.Count);
                foreach (var unknown1 in this._Unknown1)
                {
                    writer.PushFrameLength(24);
                    writer.WriteUInt32(unknown1);
                    writer.PopFrameLength();
                }

                writer.WriteUInt16((ushort)this._ProgressionData.Count);
                foreach (var progressionData in this._ProgressionData)
                {
                    progressionData.Write(writer, this._Version);
                }

                if (this._Version >= 7)
                {
                    writer.WriteUInt16((ushort)this._ProgressionSnapshots.Count);
                    foreach (var progressionSnapshot in this._ProgressionSnapshots)
                    {
                        progressionSnapshot.Write(writer, this._Version);
                    }
                }

                if (this._Version < 4)
                {
                    writer.WriteUInt16((ushort)this._Unknown2.Count);
                    foreach (var unknown2 in this._Unknown2)
                    {
                        writer.WriteUInt32(unknown2);
                    }
                    writer.WriteUInt16((ushort)this._Unknown3.Count);
                    foreach (var unknown3 in this._Unknown3)
                    {
                        writer.WriteUInt32(unknown3);
                    }
                }

                if (this._Version >= 10)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        writer.WriteUInt32(this._Unknown4[i]);
                    }
                }

                writer.WriteUInt16((ushort)this._Unknown5.Count);
                foreach (var unknown5 in this._Unknown5)
                {
                    unknown5.Write(writer, this._Version);
                }

                if (this._Version >= 3)
                {
                    writer.WriteUInt32(this._Unknown6);
                }

                if (this._Version >= 8)
                {
                    writer.WriteUInt16((ushort)this._Unknown7.Count);
                    foreach (var unknown7 in this._Unknown7)
                    {
                        writer.WriteUInt32(unknown7);
                    }
                }

                if (this._Version >= 12)
                {
                    writer.WriteTransform(this._Unknown8);
                }
            }
        }
    }
}
