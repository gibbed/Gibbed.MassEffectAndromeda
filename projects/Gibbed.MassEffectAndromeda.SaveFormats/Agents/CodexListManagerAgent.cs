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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    // ServerCodexListManagerEntity
    [Agent(_AgentName)]
    public class CodexListManagerAgent : Agent
    {
        private const string _AgentName = "CodexListManagerSaveGameAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly Data.CodexListUnknown0 _Unknown1;
        #endregion

        public CodexListManagerAgent()
        {
            this._Unknown1 = new Data.CodexListUnknown0();
        }

        #region Properties
        public Data.CodexListUnknown0 Unknown1
        {
            get { return this._Unknown1; }
        }
        #endregion

        internal override void Read4(IBitReader reader)
        {
            base.Read4(reader);
            var unknown1Length = reader.PushFrameLength(24);
            if (unknown1Length > 0)
            {
                this._Unknown1.Read(reader);
            }
            reader.PopFrameLength();
        }

        internal override void Write4(IBitWriter writer)
        {
            base.Write4(writer);
            writer.PushFrameLength(24);
            this._Unknown1.Write(writer);
            writer.PopFrameLength();
        }
    }
}
