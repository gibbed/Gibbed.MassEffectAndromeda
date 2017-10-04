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

namespace Gibbed.MassEffectAndromeda.SaveFormats.CustomizedParameters
{
    public abstract class CustomizedParameter : ICustomizedParameter
    {
        public abstract string ComponentName { get; }

        #region Fields
        private uint _ParameterId;
        #endregion

        #region Properties
        public uint ParameterId
        {
            get { return this._ParameterId; }
            set { this._ParameterId = value; }
        }
        #endregion

        public virtual void Read(IBitReader reader, ushort version)
        {
            this._ParameterId = reader.ReadUInt32();
        }

        public virtual void Write(IBitWriter writer, ushort version)
        {
            writer.WriteUInt32(this._ParameterId);
        }
    }
}
