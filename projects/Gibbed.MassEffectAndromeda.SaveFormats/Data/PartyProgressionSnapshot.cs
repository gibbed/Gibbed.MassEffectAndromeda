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
using System.Collections.Generic;
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class PartyProgressionSnapshot
    {
        private int _Level;
        private int _SkillPointCount;
        private int _LowestPuchasableSkillCost;
        private int _SkillPointPerLevel;
        private int _CharacterId;
        private int _MaximumShields;
        private int _MaximumHealth;
        private List<PartyMemberActiveSkill> _ActiveSkills;

        public PartyProgressionSnapshot()
        {
            this._ActiveSkills = new List<PartyMemberActiveSkill>();
        }

        internal void Read(IBitReader reader, uint version)
        {
            this._ActiveSkills.Clear();
            if (version >= 11)
            {
                reader.PushFrameLength(24);
                this._Level = reader.ReadInt32();
                this._SkillPointCount = reader.ReadInt32();
                this._LowestPuchasableSkillCost = reader.ReadInt32();
                this._SkillPointPerLevel = reader.ReadInt32();
                this._CharacterId = reader.ReadInt32();
                this._MaximumShields = reader.ReadInt32();
                this._MaximumHealth = reader.ReadInt32();
                var activeSkillCount = reader.ReadUInt16();
                for (int i = 0; i < activeSkillCount; i++)
                {
                    reader.PushFrameLength(24);
                    PartyMemberActiveSkill activeSkill;
                    activeSkill.LineHash = reader.ReadInt32();
                    activeSkill.LineRank = reader.ReadInt32();
                    activeSkill.GroupHash = reader.ReadInt32();
                    activeSkill.GroupTypeId = reader.ReadInt32();
                    this._ActiveSkills.Add(activeSkill);
                    reader.PopFrameLength();
                }
                reader.PopFrameLength();
            }
            else
            {
                reader.PushFrameLength(24);
                throw new NotImplementedException();
                reader.PopFrameLength();
            }
        }

        internal void Write(IBitWriter writer, uint version)
        {
            if (version >= 11)
            {
                writer.PushFrameLength(24);
                writer.WriteInt32(this._Level);
                writer.WriteInt32(this._SkillPointCount);
                writer.WriteInt32(this._LowestPuchasableSkillCost);
                writer.WriteInt32(this._SkillPointPerLevel);
                writer.WriteInt32(this._CharacterId);
                writer.WriteInt32(this._MaximumShields);
                writer.WriteInt32(this._MaximumHealth);
                writer.WriteUInt16((ushort)this._ActiveSkills.Count);
                foreach (var activeSkill in this._ActiveSkills)
                {
                    writer.PushFrameLength(24);
                    writer.WriteInt32(activeSkill.LineHash);
                    writer.WriteInt32(activeSkill.LineRank);
                    writer.WriteInt32(activeSkill.GroupHash);
                    writer.WriteInt32(activeSkill.GroupTypeId);
                    writer.PopFrameLength();
                }
                writer.PopFrameLength();
            }
            else
            {
                writer.PushFrameLength(24);
                throw new NotImplementedException();
                writer.PopFrameLength();
            }
        }
    }
}
