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
    // ServerProgressionComponent
    [JsonObject(MemberSerialization.OptIn)]
    public class ProgressionComponent
    {
        #region Fields
        private int _Level;
        private float _Unknown1;
        private uint _Unknown2;
        private uint _Unknown3;
        private readonly List<Data.SkillGroup> _SkillGroups;
        private int _CurrentProfileId;
        private readonly List<Data.Profile> _Profiles;
        private int _CurrentPresetIndex;
        private readonly List<Data.Preset> _Presets; 
        #endregion

        public ProgressionComponent()
        {
            this._SkillGroups = new List<Data.SkillGroup>();
            this._Profiles = new List<Data.Profile>();
            this._Presets = new List<Data.Preset>();
        }

        #region Properties
        [JsonProperty("level")]
        public int Level
        {
            get { return this._Level; }
            set { this._Level = value; }
        }

        [JsonProperty("unknown1")]
        public float Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public uint Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("unknown3")]
        public uint Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        [JsonProperty("skill_groups")]
        public List<Data.SkillGroup> SkillGroups
        {
            get { return this._SkillGroups; }
        }

        [JsonProperty("current_profile_id")]
        public int CurrentProfileId
        {
            get { return this._CurrentProfileId; }
            set { this._CurrentProfileId = value; }
        }

        [JsonProperty("profiles")]
        public List<Data.Profile> Profiles
        {
            get { return this._Profiles; }
        }

        [JsonProperty("current_preset_index")]
        public int CurrentPresetIndex
        {
            get { return this._CurrentPresetIndex; }
            set { this._CurrentPresetIndex = value; }
        }

        [JsonProperty("presets")]
        public List<Data.Preset> Presets
        {
            get { return this._Presets; }
        }
        #endregion

        public void Read(IBitReader reader, ushort baseVersion, bool excludeProfiles, bool excludePresets)
        {
            this._Level = reader.ReadInt32();
            this._Unknown1 = reader.ReadFloat32();
            this._Unknown2 = reader.ReadUInt32(); // skill points?

            if (baseVersion < 5)
            {
                return;
            }

            var version = reader.ReadUInt32();
            if (version > 6)
            {
                throw new SaveFormatException("unsupported version");
            }

            if (version >= 2 && version <= 3)
            {
                reader.SkipBits(32);
            }

            if (version >= 3)
            {
                this._Unknown3 = reader.ReadUInt32(); // skill points?
            }

            var skillGroupCount = reader.ReadUInt16();
            this._SkillGroups.Clear();
            for (int i = 0; i < skillGroupCount; i++)
            {
                var skillGroup = new Data.SkillGroup();
                skillGroup.Read(reader);
                this._SkillGroups.Add(skillGroup);
            }

            if (excludeProfiles == false)
            {
                this._CurrentProfileId = reader.ReadInt32();
                var profileCount = reader.ReadUInt16();
                this._Profiles.Clear();
                for (int i = 0; i < profileCount; i++)
                {
                    var profile = new Data.Profile();
                    profile.Read(reader, version);
                    this._Profiles.Add(profile);
                }
            }

            if (excludePresets == false)
            {
                this._CurrentPresetIndex = reader.ReadInt32();
                var presetCount = reader.ReadUInt16();
                this._Presets.Clear();
                for (int i = 0; i < presetCount; i++)
                {
                    var preset = new Data.Preset();
                    preset.Read(reader);
                    this._Presets.Add(preset);
                }
            }
        }

        public void Write(IBitWriter writer, bool excludeProfiles, bool excludePresets)
        {
            writer.WriteInt32(this._Level);
            writer.WriteFloat32(this._Unknown1);
            writer.WriteUInt32(this._Unknown2);
            writer.WriteUInt32(6); // version
            writer.WriteUInt32(this._Unknown3);

            writer.WriteUInt16((ushort)this._SkillGroups.Count);
            foreach (var skillGroup in this._SkillGroups)
            {
                skillGroup.Write(writer);
            }

            if (excludeProfiles == false)
            {
                writer.WriteInt32(this._CurrentProfileId);
                writer.WriteUInt16((ushort)this._Profiles.Count);
                foreach (var profile in this._Profiles)
                {
                    profile.Write(writer);
                }
            }

            if (excludePresets == false)
            {
                writer.WriteInt32(this._CurrentPresetIndex);
                writer.WriteUInt16((ushort)this._Presets.Count);
                foreach (var preset in this._Presets)
                {
                    preset.Write(writer);
                }
            }
        }
    }
}
