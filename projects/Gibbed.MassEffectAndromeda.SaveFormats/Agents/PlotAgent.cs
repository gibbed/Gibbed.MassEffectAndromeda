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
    [Agent(_AgentName)]
    public class PlotAgent : Agent
    {
        private const string _AgentName = "PlotSaveGameAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly Data.Plot _Plot;
        #endregion

        public PlotAgent()
        {
            this._Plot = new Data.Plot();
        }

        #region Properties
        public Data.Plot Plot
        {
            get { return this._Plot; }
        }
        #endregion

        internal override void Read2(IBitReader reader)
        {
            base.Read2(reader);
            reader.PushFrameLength(24);
            this._Plot.Read(reader);
            reader.PopFrameLength();
        }

        internal override void Write2(IBitWriter writer)
        {
            base.Write2(writer);
            writer.PushFrameLength(24);
            this._Plot.Write(writer);
            writer.PopFrameLength();
        }
    }
}
