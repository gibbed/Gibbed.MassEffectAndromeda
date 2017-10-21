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
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Items
{
    public class RangedWeaponItemData : GearItemData
    {
        #region Fields
        private ushort _Unknown5;
        private ushort _Unknown6;
        private ushort _Unknown7;
        private uint _Unknown8;
        #endregion

        #region Properties
        [JsonProperty("unknown5")]
        public ushort Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }

        [JsonProperty("unknown6")]
        public ushort Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        [JsonProperty("unknown7")]
        public ushort Unknown7
        {
            get { return this._Unknown7; }
            set { this._Unknown7 = value; }
        }

        [JsonProperty("unknown8")]
        public uint Unknown8
        {
            get { return this._Unknown8; }
            set { this._Unknown8 = value; }
        }
        #endregion

        public override void Read(IBitReader reader, int version)
        {
            this._Unknown5 = reader.ReadUInt16();
            this._Unknown6 = reader.ReadUInt16();
            this._Unknown7 = reader.ReadUInt16();
            this._Unknown8 = version < 8 ? 0 : reader.ReadUInt32();
            base.Read(reader, version);
        }
    }
}
