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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Profile
    {
        #region Fields
        private int _Level;
        private bool _IsNew;
        #endregion

        #region Properties
        [JsonProperty("level")]
        public int Level
        {
            get { return this._Level; }
            set { this._Level = value; }
        }

        [JsonProperty("is_new")]
        public bool IsNew
        {
            get { return this._IsNew; }
            set { this._IsNew = value; }
        }
        #endregion

        internal void Read(IBitReader reader, uint version)
        {
            this._Level = reader.ReadInt32();
            this._IsNew = version >= 5 && reader.ReadBoolean();
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteInt32(this._Level);
            writer.WriteBoolean(this._IsNew);
        }
    }
}
