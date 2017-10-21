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
    public class PartyMemberVehicleHealthData
    {
        #region Fields
        private float _CurrentValue;
        private float _MaximumValue;
        private bool _Unknown;
        #endregion

        #region Properties
        [JsonProperty("current_value")]
        public float CurrentValue
        {
            get { return this._CurrentValue; }
            set { this._CurrentValue = value; }
        }

        [JsonProperty("maximum_value")]
        public float MaximumValue
        {
            get { return this._MaximumValue; }
            set { this._MaximumValue = value; }
        }

        [JsonProperty("unknown")]
        public bool Unknown
        {
            get { return this._Unknown; }
            set { this._Unknown = value; }
        }
        #endregion

        public void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._CurrentValue = reader.ReadFloat32();
            this._MaximumValue = reader.ReadFloat32();
            this._Unknown = reader.ReadBoolean();
            reader.PopFrameLength();
        }

        public void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteFloat32(this._CurrentValue);
            writer.WriteFloat32(this._MaximumValue);
            writer.WriteBoolean(this._Unknown);
            writer.PopFrameLength();
        }
    }
}
