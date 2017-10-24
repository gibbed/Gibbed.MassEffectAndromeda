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
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    // GalaxyDataManager
    [JsonObject(MemberSerialization.OptIn)]
    [Agent(_AgentName)]
    public class GalaxyDataManagerAgent : Agent
    {
        private const string _AgentName = "GalaxyDataStateManagerSaveAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private Guid _CurrentSystem;
        private Guid _CurrentDestination;
        private readonly Data.GalaxyDataUnknown0 _Unknown3;
        private readonly Data.GalaxyDataUnknown0 _Unknown4;
        #endregion

        public GalaxyDataManagerAgent()
            : base(2)
        {
            this._Unknown3 = new Data.GalaxyDataUnknown0();
            this._Unknown4 = new Data.GalaxyDataUnknown0();
        }

        #region Properties
        [JsonProperty("current_system")]
        public Guid CurrentSystem
        {
            get { return this._CurrentSystem; }
            set { this._CurrentSystem = value; }
        }

        [JsonProperty("current_destination")]
        public Guid CurrentDestination
        {
            get { return this._CurrentDestination; }
            set { this._CurrentDestination = value; }
        }

        [JsonProperty("unknown3")]
        public Data.GalaxyDataUnknown0 Unknown3
        {
            get { return this._Unknown3; }
        }

        [JsonProperty("unknown4")]
        public Data.GalaxyDataUnknown0 Unknown4
        {
            get { return this._Unknown4; }
        }
        #endregion

        internal override void Read2(IBitReader reader)
        {
            base.Read2(reader);

            reader.PushFrameLength(24);

            if (this.ReadVersion >= 2)
            {
                reader.PushFrameLength(24);
                this._CurrentSystem = reader.ReadGuid();
                this._CurrentDestination = reader.ReadGuid();
                reader.PopFrameLength();
            }

            this._Unknown3.Read(reader, this.ReadVersion);
            this._Unknown4.Read(reader, this.ReadVersion);

            reader.PopFrameLength();
        }

        internal override void Write2(IBitWriter writer)
        {
            base.Write2(writer);

            writer.PushFrameLength(24);
                writer.PushFrameLength(24);
                writer.WriteGuid(this._CurrentSystem);
                writer.WriteGuid(this._CurrentDestination);
                writer.PopFrameLength();
            this._Unknown3.Write(writer);
            this._Unknown4.Write(writer);
            writer.PopFrameLength();
        }
    }
}
