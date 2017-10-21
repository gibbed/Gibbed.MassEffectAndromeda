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
    public class DestructionManagerAgent : Agent
    {
        private const string _AgentName = "DestructionManager";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly Data.DestructionUnknown0 _Unknown1;
        private readonly Data.DestructionUnknown3 _Unknown2;
        #endregion

        public DestructionManagerAgent()
            : base(1)
        {
            this._Unknown1 = new Data.DestructionUnknown0();
            this._Unknown2 = new Data.DestructionUnknown3();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public Data.DestructionUnknown0 Unknown1
        {
            get { return this._Unknown1; }
        }

        [JsonProperty("unknown2")]
        public Data.DestructionUnknown3 Unknown2
        {
            get { return this._Unknown2; }
        }
        #endregion

        internal override void Read5(IBitReader reader)
        {
            base.Read5(reader);
            reader.PushFrameLength(24);
            this._Unknown1.Read(reader);
            reader.PopFrameLength();
            reader.PushFrameLength(24);
            this._Unknown2.Read(reader);
            reader.PopFrameLength();
        }

        internal override void Write5(IBitWriter writer)
        {
            base.Write5(writer);
            writer.PushFrameLength(24);
            this._Unknown1.Write(writer);
            writer.PopFrameLength();
            writer.PushFrameLength(24);
            this._Unknown2.Write(writer);
            writer.PopFrameLength();
        }
    }
}
