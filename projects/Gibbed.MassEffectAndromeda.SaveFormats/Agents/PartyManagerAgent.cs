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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
    [JsonObject(MemberSerialization.OptIn)]
    [Agent(_AgentName)]
    public class PartyManagerAgent : Agent, INotifyPropertyChanged
    {
        private const string _AgentName = "MEPartyManagerSaveGameAgent";

        internal override string AgentName
        {
            get { return _AgentName; }
        }

        #region Fields
        private uint _Unknown1;
        private uint _NextItemId;
        private bool _HasActiveSquad;
        private readonly Data.PartyActiveSquad _ActiveSquad;
        private readonly Data.PartySquad _Squad;
        #endregion

        public PartyManagerAgent()
            : base(4)
        {
            this._ActiveSquad = new Data.PartyActiveSquad();
            this._Squad = new Data.PartySquad();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set
            {
                this._Unknown1 = value;
                this.NotifyPropertyChanged("Unknown1");
            }
        }

        [JsonProperty("next_item_id")]
        public uint NextItemId
        {
            get { return this._NextItemId; }
            set
            {
                this._NextItemId = value;
                this.NotifyPropertyChanged("NextItemId");
            }
        }

        [JsonProperty("has_active_squad")]
        public bool HasActiveSquad
        {
            get { return this._HasActiveSquad; }
            set
            {
                this._HasActiveSquad = value;
                this.NotifyPropertyChanged("HasActiveSquad");
            }
        }

        [JsonProperty("active_squad")]
        public Data.PartyActiveSquad ActiveSquad
        {
            get { return this._ActiveSquad; }
        }

        [JsonProperty("squad")]
        public Data.PartySquad Squad
        {
            get { return this._Squad; }
        }
        #endregion

        internal override void Read1(IBitReader reader, ushort arg1)
        {
            base.Read1(reader, arg1);

            if (arg1 < 2)
            {
                this._Unknown1 = reader.ReadUInt32();
            }
        }

        internal override void Write1(IBitWriter writer, ushort arg1)
        {
            base.Write1(writer, arg1);

            if (arg1 < 2)
            {
                writer.WriteUInt32(this._Unknown1);
            }
        }

        internal override void Read3(IBitReader reader, ushort arg1)
        {
            base.Read3(reader, arg1);
            reader.PushFrameLength(24);
            if (arg1 >= 2)
            {
                this._NextItemId = reader.ReadUInt32();
            }
            this._HasActiveSquad = arg1 < 3 || reader.ReadBoolean();
            if (this._HasActiveSquad == true)
            {
                this._ActiveSquad.Read(reader);
            }
            reader.PopFrameLength();
        }

        internal override void Write3(IBitWriter writer, ushort arg1)
        {
            base.Write3(writer, arg1);
            writer.PushFrameLength(24);
            if (arg1 >= 2)
            {
                writer.WriteUInt32(this._NextItemId);
            }
            if (arg1 >= 3)
            {
                writer.WriteBoolean(this._HasActiveSquad);
            }
            if (arg1 < 3 || this._HasActiveSquad == true)
            {
                this._ActiveSquad.Write(writer);
            }
            writer.PopFrameLength();
        }

        internal override void Read4(IBitReader reader)
        {
            base.Read4(reader);
            var length = reader.PushFrameLength(24);
            if (length > 0)
            {
                this._Squad.Read(reader);
            }
            reader.PopFrameLength();
        }

        internal override void Write4(IBitWriter writer)
        {
            base.Write4(writer);
            writer.PushFrameLength(24);
            this._Squad.Write(writer);
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
