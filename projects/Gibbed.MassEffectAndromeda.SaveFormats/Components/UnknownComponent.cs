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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UnknownComponent
    {
        #region Fields
        private Vector3 _Unknown1;
        private Vector3 _Unknown2;
        private Vector3 _Unknown3;
        private Vector3 _Unknown4;
        private bool _Unknown5;
        private uint _Unknown6;
        private uint _Unknown7;
        private readonly List<Data.PartyMemberVehicleData> _Vehicles;
        #endregion

        public UnknownComponent()
        {
            this._Vehicles = new List<Data.PartyMemberVehicleData>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public Vector3 Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public Vector3 Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("unknown3")]
        public Vector3 Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        [JsonProperty("unknown4")]
        public Vector3 Unknown4
        {
            get { return this._Unknown4; }
            set { this._Unknown4 = value; }
        }

        [JsonProperty("unknown5")]
        public bool Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }

        [JsonProperty("unknown6")]
        public uint Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        [JsonProperty("unknown7")]
        public uint Unknown7
        {
            get { return this._Unknown7; }
            set { this._Unknown7 = value; }
        }

        [JsonProperty("vehicles")]
        public List<Data.PartyMemberVehicleData> Vehicles
        {
            get { return this._Vehicles; }
        }
        #endregion

        public void Read(IBitReader reader, uint baseVersion)
        {
            this._Unknown1 = reader.ReadVector3();
            this._Unknown2 = reader.ReadVector3();
            this._Unknown3 = reader.ReadVector3();
            this._Unknown4 = reader.ReadVector3();
            this._Unknown5 = reader.ReadBoolean();
            if (this._Unknown5 == true)
            {
                this._Unknown6 = reader.ReadUInt32();
                this._Unknown7 = reader.ReadUInt32();
            }
            if (baseVersion > 9)
            {
                var vehicleCount = reader.ReadUInt32();
                this._Vehicles.Clear();
                for (uint i = 0; i < vehicleCount; i++)
                {
                    var vehicle = new Data.PartyMemberVehicleData();
                    vehicle.Read(reader);
                    this._Vehicles.Add(vehicle);
                }
            }
        }

        public void Write(IBitWriter writer)
        {
            writer.WriteVector3(this._Unknown1);
            writer.WriteVector3(this._Unknown2);
            writer.WriteVector3(this._Unknown3);
            writer.WriteVector3(this._Unknown4);
            writer.WriteBoolean(this._Unknown5);
            if (this._Unknown5 == true)
            {
                writer.WriteUInt32(this._Unknown6);
                writer.WriteUInt32(this._Unknown7);
            }
            writer.WriteUInt32((uint)this._Vehicles.Count);
            foreach (var vehicle in this._Vehicles)
            {
                vehicle.Write(writer);
            }
        }
    }
}
