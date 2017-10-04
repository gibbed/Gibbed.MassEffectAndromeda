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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    // GalaxyDataManager
    [Agent(_AgentName)]
    public class GalaxyDataManagerAgent : Agent
    {
        private const string _AgentName = "GalaxyDataStateManagerSaveAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private Guid _Unknown1;
        private Guid _Unknown2;
        private readonly Data.GalaxyDataUnknown0 _Unknown3;
        private readonly Data.GalaxyDataUnknown0 _Unknown4;
        #endregion

        public GalaxyDataManagerAgent()
        {
            this._Unknown3 = new Data.GalaxyDataUnknown0();
            this._Unknown4 = new Data.GalaxyDataUnknown0();
        }

        #region Properties
        public Guid Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        public Guid Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        public Data.GalaxyDataUnknown0 Unknown3
        {
            get { return this._Unknown3; }
        }

        public Data.GalaxyDataUnknown0 Unknown4
        {
            get { return this._Unknown4; }
        }
        #endregion

        internal override void Read2(IBitReader reader)
        {
            base.Read2(reader);

            reader.PushFrameLength(24);

            if (this.Version >= 2)
            {
                reader.PushFrameLength(24);
                this._Unknown1 = reader.ReadGuid();
                this._Unknown2 = reader.ReadGuid();
                reader.PopFrameLength();
            }

            this._Unknown3.Read(reader, this.Version);
            this._Unknown4.Read(reader, this.Version);

            reader.PopFrameLength();
        }

        internal override void Write2(IBitWriter writer)
        {
            base.Write2(writer);

            writer.PushFrameLength(24);

            if (this.Version >= 2)
            {
                writer.PushFrameLength(24);
                writer.WriteGuid(this._Unknown1);
                writer.WriteGuid(this._Unknown2);
                writer.PopFrameLength();
            }

            this._Unknown3.Write(writer, this.Version);
            this._Unknown4.Write(writer, this.Version);

            writer.PopFrameLength();
        }
    }
}
