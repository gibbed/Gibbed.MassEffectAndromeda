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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    [Agent(_AgentName)]
    public class WorldStateManagerAgent : Agent
    {
        private const string _AgentName = "WorldStateManager";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly List<KeyValuePair<string, string>> _Unknown1;
        #endregion

        public WorldStateManagerAgent()
        {
            this._Unknown1 = new List<KeyValuePair<string, string>>();
        }

        #region Properties
        public List<KeyValuePair<string, string>> Unknown1
        {
            get { return this._Unknown1; }
        }
        #endregion

        internal override void Read1(IBitReader reader, ushort arg1)
        {
            base.Read1(reader, arg1);
            reader.PushFrameLength(24);
            this._Unknown1.Clear();
            var unknown1Count = reader.ReadUInt16();
            for (int i = 0; i < unknown1Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown1Key = reader.ReadString();
                var unknown1Value = reader.ReadString();
                reader.PopFrameLength();
                this._Unknown1.Add(new KeyValuePair<string, string>(unknown1Key, unknown1Value));
            }
            reader.PopFrameLength();
        }

        internal override void Write1(IBitWriter writer, ushort arg1)
        {
            base.Write1(writer, arg1);
            writer.PushFrameLength(24);
            writer.WriteUInt16((ushort)this._Unknown1.Count);
            foreach (var kv in this._Unknown1)
            {
                writer.PushFrameLength(24);
                writer.WriteString(kv.Key);
                writer.WriteString(kv.Value);
                writer.PopFrameLength();
            }
            writer.PopFrameLength();
        }
    }
}
