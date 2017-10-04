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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Components
{
    // ServerProgressionComponent
    public class ProgressionComponent
    {
        #region Fields
        private int _Level;
        private float _Unknown1;
        private uint _Unknown2;
        private uint _Unknown3;
        private uint _Unknown4;
        private readonly List<Data.ProgressionUnknown0> _Unknown5;
        private int _Unknown6;
        private readonly List<KeyValuePair<int, bool>> _Unknown7;
        private int _Unknown8;
        private readonly List<Data.ProgressionUnknown2> _Unknown9; 
        #endregion

        public ProgressionComponent()
        {
            this._Unknown5 = new List<Data.ProgressionUnknown0>();
            this._Unknown7 = new List<KeyValuePair<int, bool>>();
            this._Unknown9 = new List<Data.ProgressionUnknown2>();
        }

        #region Properties
        public int Level
        {
            get { return this._Level; }
            set { this._Level = value; }
        }

        public float Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        public uint Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        public uint Unknown3
        {
            get { return this._Unknown3; }
            set { this._Unknown3 = value; }
        }

        public uint Unknown4
        {
            get { return this._Unknown4; }
            set { this._Unknown4 = value; }
        }

        public List<Data.ProgressionUnknown0> Unknown5
        {
            get { return this._Unknown5; }
        }

        public int Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        public List<KeyValuePair<int, bool>> Unknown7
        {
            get { return this._Unknown7; }
        }

        public int Unknown8
        {
            get { return this._Unknown8; }
            set { this._Unknown8 = value; }
        }

        public List<Data.ProgressionUnknown2> Unknown9
        {
            get { return this._Unknown9; }
        }
        #endregion

        public void Read(IBitReader reader, int version, int characterIndex)
        {
            this._Level = reader.ReadInt32();
            this._Unknown1 = reader.ReadFloat32();
            this._Unknown2 = reader.ReadUInt32(); // skill points?

            if (version >= 5)
            {
                this._Unknown3 = reader.ReadUInt32();

                if (this._Unknown3 >= 2 && this._Unknown3 <= 3)
                {
                    reader.SkipBits(32);
                }

                if (this._Unknown3 >= 3)
                {
                    this._Unknown4 = reader.ReadUInt32(); // skill points?
                }

                // skill data
                var unknown5Count = reader.ReadUInt16();
                this._Unknown5.Clear();
                for (int i = 0; i < unknown5Count; i++)
                {
                    var unknown5 = new Data.ProgressionUnknown0();
                    unknown5.Read(reader);
                    this._Unknown5.Add(unknown5);
                }

                if (characterIndex == 0) // ExcludeProfiles
                {
                    this._Unknown6 = reader.ReadInt32();
                    var unknown7Count = reader.ReadUInt16();
                    this._Unknown7.Clear();
                    for (int i = 0; i < unknown7Count; i++)
                    {
                        var unknown7Key = reader.ReadInt32();
                        var unknown7Value = this._Unknown3 >= 5 && reader.ReadBoolean();
                        this._Unknown7.Add(new KeyValuePair<int, bool>(unknown7Key, unknown7Value));
                    }
                }

                if (characterIndex == 0) // ExcludePresets
                {
                    this._Unknown8 = reader.ReadInt32();

                    var unknown9Count = reader.ReadUInt16();
                    this._Unknown9.Clear();
                    for (int i = 0; i < unknown9Count; i++)
                    {
                        var unknown9 = new Data.ProgressionUnknown2();
                        unknown9.Read(reader);
                        this._Unknown9.Add(unknown9);
                    }
                }
            }
        }
    }
}
