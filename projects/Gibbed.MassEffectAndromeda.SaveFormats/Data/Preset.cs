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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Preset
    {
        #region Fields
        private int _ProfileId;
        private readonly List<PresetPower> _Powers;
        #endregion

        public Preset()
        {
            this._Powers = new List<PresetPower>();
        }

        #region Properties
        [JsonProperty("profile_id")]
        public int ProfileId
        {
            get { return this._ProfileId; }
            set { this._ProfileId = value; }
        }

        [JsonProperty("powers")]
        public List<PresetPower> Powers
        {
            get { return this._Powers; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._ProfileId = reader.ReadInt32();
            var powerCount = reader.ReadUInt16();
            this._Powers.Clear();
            for (int i = 0; i < powerCount; i++)
            {
                var power = new PresetPower();
                power.Read(reader);
                this._Powers.Add(power);
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteInt32(this._ProfileId);
            writer.WriteUInt16((ushort)this._Powers.Count);
            foreach (var power in this._Powers)
            {
                power.Write(writer);
            }
            writer.PopFrameLength();
        }
    }
}
