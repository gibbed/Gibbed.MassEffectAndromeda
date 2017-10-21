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
    public class PowerItemData : GearItemData
    {
        #region Fields
        private uint _Unknown5;
        private uint _Unknown6;
        private bool _Unknown7;
        private uint _Unknown8;
        #endregion

        #region Properties
        [JsonProperty("unknown5")]
        public uint Unknown5
        {
            get { return this._Unknown5; }
            set { this._Unknown5 = value; }
        }

        [JsonProperty("unknown6")]
        public uint Unknown6
        {
            get { return this._Unknown6; }
            set { this._Unknown6 = value; }
        }

        [JsonProperty("unknown7")]
        public bool Unknown7
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
            if (version >= 1)
            {
                reader.PushFrameLength(24);
                this._Unknown5 = reader.ReadUInt32();
                this._Unknown6 = reader.ReadUInt32();
                this._Unknown7 = reader.ReadBoolean();
                this._Unknown8 = reader.ReadUInt32();
                reader.PopFrameLength();
            }

            base.Read(reader, version);
        }
    }
}
