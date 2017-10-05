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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Agents
{
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
        private uint _Unknown2;
        private bool _Unknown3;
        private readonly Data.PartyUnknown0 _Unknown4;
        private readonly Data.PartyUnknown2 _Unknown5;
        #endregion

        public PartyManagerAgent()
        {
            this._Unknown4 = new Data.PartyUnknown0();
            this._Unknown5 = new Data.PartyUnknown2();
        }

        #region Properties
        public uint Unknown1
        {
            get { return this._Unknown1; }
            set
            {
                this._Unknown1 = value;
                this.NotifyPropertyChanged("Unknown1");
            }
        }

        public uint Unknown2
        {
            get { return this._Unknown2; }
            set
            {
                this._Unknown2 = value;
                this.NotifyPropertyChanged("Unknown2");
            }
        }

        public bool Unknown3
        {
            get { return this._Unknown3; }
            set
            {
                this._Unknown3 = value;
                this.NotifyPropertyChanged("Unknown3");
            }
        }

        public Data.PartyUnknown0 Unknown4
        {
            get { return this._Unknown4; }
        }

        public Data.PartyUnknown2 Unknown5
        {
            get { return this._Unknown5; }
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
                this._Unknown2 = reader.ReadUInt32();
            }
            this._Unknown3 = arg1 < 3 || reader.ReadBoolean();
            if (this._Unknown3 == true)
            {
                this._Unknown4.Read(reader);
            }
            reader.PopFrameLength();
        }

        internal override void Write3(IBitWriter writer, ushort arg1)
        {
            base.Write3(writer, arg1);
            writer.PushFrameLength(24);
            if (arg1 >= 2)
            {
                writer.WriteUInt32(this._Unknown2);
            }
            if (arg1 >= 3)
            {
                writer.WriteBoolean(this._Unknown3);
            }
            if (arg1 < 3 || this._Unknown3 == true)
            {
                this._Unknown4.Write(writer);
            }
            writer.PopFrameLength();
        }

        internal override void Read4(IBitReader reader)
        {
            base.Read4(reader);
            var length = reader.PushFrameLength(24);
            if (length > 0)
            {
                this._Unknown5.Read(reader);
            }
            reader.PopFrameLength();
        }

        internal override void Write4(IBitWriter writer)
        {
            base.Write4(writer);
            writer.PushFrameLength(24);
            this._Unknown5.Write(writer);
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
