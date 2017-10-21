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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    [JsonObject(MemberSerialization.OptIn)]
    [Agent(_AgentName)]
    public class VendorInventoryAgent : Agent
    {
        private const string _AgentName = "ServerVendorInventorySaveGameAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private readonly List<KeyValuePair<uint, byte[]>> _Unknown;
        #endregion

        public VendorInventoryAgent()
            : base(2)
        {
            this._Unknown = new List<KeyValuePair<uint, byte[]>>();
        }

        #region Properties
        [JsonProperty("unknown")]
        public List<KeyValuePair<uint, byte[]>> Unknown
        {
            get { return this._Unknown; }
        }
        #endregion

        internal override void Read4(IBitReader reader)
        {
            base.Read4(reader);
            var unknown1Count = reader.ReadUInt16();
            this._Unknown.Clear();
            for (int i = 0; i < unknown1Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown1Key = reader.ReadUInt32();
                var unknown1ValueLength = reader.ReadUInt32();
                var unknown1Value = reader.ReadBytes((int)unknown1ValueLength);
                reader.PopFrameLength();
                this._Unknown.Add(new KeyValuePair<uint, byte[]>(unknown1Key, unknown1Value));
            }
        }

        internal override void Write4(IBitWriter writer)
        {
            base.Write4(writer);
            writer.WriteUInt16((ushort)this._Unknown.Count);
            foreach (var kv in this._Unknown)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(kv.Key);
                writer.WriteUInt32((uint)kv.Value.Length);
                writer.WriteBytes(kv.Value);
                writer.PopFrameLength();
            }
        }
    }
}
