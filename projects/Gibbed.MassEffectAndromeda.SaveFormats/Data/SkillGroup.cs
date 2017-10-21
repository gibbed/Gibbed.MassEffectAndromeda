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
    public class SkillGroup
    {
        #region Fields
        private readonly List<SkillLine> _SkillLines;
        #endregion

        public SkillGroup()
        {
            this._SkillLines = new List<SkillLine>();
        }

        #region Properties
        [JsonProperty("skill_lines")]
        public List<SkillLine> SkillLines
        {
            get { return this._SkillLines; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._SkillLines.Clear();
            var skillLineCount = reader.ReadUInt16();
            for (int i = 0; i < skillLineCount; i++)
            {
                var skillLine = new SkillLine();
                skillLine.Read(reader);
                this._SkillLines.Add(skillLine);
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._SkillLines.Count);
            foreach (var unknown1 in this._SkillLines)
            {
                unknown1.Write(writer);
            }
        }
    }
}
