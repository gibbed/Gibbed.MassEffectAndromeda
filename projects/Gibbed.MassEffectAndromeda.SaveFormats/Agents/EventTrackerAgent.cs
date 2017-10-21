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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    [JsonObject(MemberSerialization.OptIn)]
    [Agent(_AgentName)]
    public class EventTrackerAgent : Agent
    {
        private const string _AgentName = "EventTracker";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private bool _Unknown1;
        private readonly Data.EventTrackerUnknown0 _Unknown2;
        private readonly Data.EventTrackerUnknown1 _Unknown3;
        #endregion

        public EventTrackerAgent()
            : base(3)
        {
            this._Unknown2 = new Data.EventTrackerUnknown0();
            this._Unknown3 = new Data.EventTrackerUnknown1();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public bool Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public Data.EventTrackerUnknown0 Unknown2
        {
            get { return this._Unknown2; }
        }

        [JsonProperty("unknown3")]
        public Data.EventTrackerUnknown1 Unknown3
        {
            get { return this._Unknown3; }
        }
        #endregion

        internal override void Read2(IBitReader reader)
        {
            base.Read2(reader);
            if (this.ReadVersion >= 3)
            {
                this._Unknown1 = reader.ReadBoolean();
                if (this._Unknown1 == true)
                {
                    this._Unknown2.Read(reader);
                    this._Unknown3.Read(reader);
                }
            }
        }

        internal override void Write2(IBitWriter writer)
        {
            base.Write2(writer);
            writer.WriteBoolean(this._Unknown1);
            if (this._Unknown1 == true)
            {
                this._Unknown2.Write(writer);
                this._Unknown3.Write(writer);
            }
        }
    }
}
