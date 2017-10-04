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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class PartyMemberVehicleHealthData
    {
        #region Fields
        private float _CurrentValue;
        private float _MaximumValue;
        private bool _Unknown2;
        #endregion

        #region Properties
        public float CurrentValue
        {
            get { return this._CurrentValue; }
            set { this._CurrentValue = value; }
        }

        public float MaximumValue
        {
            get { return this._MaximumValue; }
            set { this._MaximumValue = value; }
        }

        public bool Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }
        #endregion

        public void Read(IBitReader reader, int version)
        {
            reader.PushFrameLength(24);
            this._CurrentValue = reader.ReadFloat32();
            this._MaximumValue = reader.ReadFloat32();
            this._Unknown2 = reader.ReadBoolean();
            reader.PopFrameLength();
        }
    }
}
