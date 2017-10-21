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

using System.ComponentModel;
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PartyMemberRawData : INotifyPropertyChanged
    {
        #region Fields
        private byte[] _DataBytes;
        private bool _Unknown1;
        private bool _Unknown2;
        #endregion

        #region Properties
        [JsonProperty("data_bytes")]
        public byte[] DataBytes
        {
            get { return this._DataBytes; }
            set
            {
                this._DataBytes = value;
                this.NotifyPropertyChanged("DataBytes");
            }
        }

        [JsonProperty("unknown1")]
        public bool Unknown1
        {
            get { return this._Unknown1; }
            set
            {
                this._Unknown1 = value;
                this.NotifyPropertyChanged("Unknown1");
            }
        }

        [JsonProperty("unknown2")]
        public bool Unknown2
        {
            get { return this._Unknown2; }
            set
            {
                this._Unknown2 = value;
                this.NotifyPropertyChanged("Unknown2");
            }
        }
        #endregion

        internal void Read(IBitReader reader, uint version)
        {
            reader.PushFrameLength(24);
            var dataBytesLength = reader.ReadUInt32();
            this._DataBytes = reader.ReadBytes((int)dataBytesLength);
            this._Unknown1 = version >= 6 && reader.ReadBoolean();
            this._Unknown2 = version >= 6 && reader.ReadBoolean();
            reader.PopFrameLength();
        }

        internal void Write(IBitWriter writer)
        {
            writer.PushFrameLength(24);
            writer.WriteUInt32((uint)this._DataBytes.Length);
            writer.WriteBytes(this._DataBytes);
            writer.WriteBoolean(this._Unknown1);
            writer.WriteBoolean(this._Unknown2);
            writer.PopFrameLength();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
