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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerMECraftingProgressionComponent
    [JsonObject(MemberSerialization.OptIn)]
    public class CraftingProgressionComponent
    {
        #region Fields
        private readonly List<KeyValuePair<uint, bool>> _Unknown;
        #endregion

        public CraftingProgressionComponent()
        {
            this._Unknown = new List<KeyValuePair<uint,bool>>();
        }

        #region Properties
        [JsonProperty("unknown")]
        public List<KeyValuePair<uint, bool>> Unknown
        {
            get { return this._Unknown; }
        }
        #endregion

        public void Read(IBitReader reader, int version)
        {
            var unknown1Count = reader.ReadUInt16();
            this._Unknown.Clear();
            for (int i = 0; i < unknown1Count; i++)
            {
                reader.PushFrameLength(24);
                var unknown1Key = reader.ReadUInt32();
                var unknown1Value = reader.ReadBoolean();
                this._Unknown.Add(new KeyValuePair<uint, bool>(unknown1Key, unknown1Value));
                reader.PopFrameLength();
            }
        }

        public void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._Unknown.Count);
            foreach (var kv in this._Unknown)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32(kv.Key);
                writer.WriteBoolean(kv.Value);
                writer.PopFrameLength();
            }
        }
    }
}
