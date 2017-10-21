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
    public class SkillLine
    {
        #region Fields
        private int _Unknown;
        private readonly List<int> _SkillRanks;
        #endregion

        public SkillLine()
        {
            this._SkillRanks = new List<int>();
        }

        #region Properties
        [JsonProperty("unknown")]
        public int Unknown
        {
            get { return this._Unknown; }
            set { this._Unknown = value; }
        }

        [JsonProperty("skill_ranks")]
        public List<int> SkillRanks
        {
            get { return this._SkillRanks; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            reader.PushFrameLength(24);
            this._Unknown = reader.ReadInt32();
            var skillRankCount = reader.ReadUInt16();
            this._SkillRanks.Clear();
            for (int k = 0; k < skillRankCount; k++)
            {
                this._SkillRanks.Add(reader.ReadInt32());
            }
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteInt32(this._Unknown);
            writer.WriteUInt16((ushort)this._SkillRanks.Count);
            foreach (var skillRank in this._SkillRanks)
            {
                writer.WriteInt32(skillRank);
            }
            writer.PopFrameLength();
        }
    }
}
