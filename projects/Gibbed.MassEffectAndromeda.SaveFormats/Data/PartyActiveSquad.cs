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
using System.ComponentModel;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PartyActiveSquad : INotifyPropertyChanged
    {
        #region Fields
        private readonly List<PartyActiveSquadMember> _Members;
        #endregion

        public PartyActiveSquad()
        {
            this._Members = new List<PartyActiveSquadMember>();
        }

        #region Properties
        [JsonProperty("members")]
        public List<PartyActiveSquadMember> Members
        {
            get { return this._Members; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            var version = reader.ReadUInt32();
            if (version > 4)
            {
                throw new SaveFormatException("unsupported version");
            }

            this._Members.Clear();
            if (version >= 2)
            {
                var memberCount = reader.ReadUInt16();
                for (int i = 0; i < memberCount; i++)
                {
                    var member = new PartyActiveSquadMember();
                    member.Read(reader);
                    this._Members.Add(member);
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt32(4);
            writer.WriteUInt16((ushort)this._Members.Count);
            foreach (var member in this._Members)
            {
                member.Write(writer);
            }
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
