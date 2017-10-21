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

using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PartyMemberVehicleData
    {
        #region Fields
        private uint _Unknown1;
        private readonly PartyMemberVehicleHealthData[] _Healths;
        #endregion

        public PartyMemberVehicleData()
        {
            // TODO(gibbed): figure out where 5 comes from
            this._Healths = new[]
            {
                new PartyMemberVehicleHealthData(),
                new PartyMemberVehicleHealthData(),
                new PartyMemberVehicleHealthData(),
                new PartyMemberVehicleHealthData(),
                new PartyMemberVehicleHealthData(),
            };
        }

        #region Properties
        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("healths")]
        public PartyMemberVehicleHealthData[] Healths
        {
            get { return this._Healths; }
        }
        #endregion

        public void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown1 = reader.ReadUInt32();
            for (int i = 0; i < 5; i++)
            {
                this._Healths[i].Read(reader);
            }
            reader.PopFrameLength();
        }

        public void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt32(this._Unknown1);
            for (int i = 0; i < 5; i++)
            {
                this._Healths[i].Write(writer);
            }
            writer.PopFrameLength();
        }
    }
}
