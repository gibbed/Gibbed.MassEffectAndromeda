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

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using Gibbed.MassEffectAndromeda.SaveFormats;
using Agents = Gibbed.MassEffectAndromeda.SaveFormats.Agents;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Core
{
    [Export(typeof(SquadViewModel))]
    internal class SquadViewModel : PropertyChangedBase
    {
        #region Imports
        private ShellViewModel _Shell;
        private SaveLoad _SaveLoad;

        [Import(typeof(ShellViewModel))]
        public ShellViewModel Shell
        {
            get { return this._Shell; }
            set
            {
                this._Shell = value;
                this.NotifyOfPropertyChange(() => this.Shell);
            }
        }

        [Import(typeof(SaveLoad))]
        public SaveLoad SaveLoad
        {
            get { return this._SaveLoad; }
            set
            {
                this._SaveLoad = value;
                this.NotifyOfPropertyChange(() => this.SaveLoad);
            }
        }
        #endregion

        #region Fields
        private readonly ObservableCollection<Squad.MemberViewModel> _Members;
        #endregion

        #region Properties
        public ObservableCollection<Squad.MemberViewModel> Members
        {
            get { return this._Members; }
        }
        #endregion

        [ImportingConstructor]
        public SquadViewModel()
        {
            this._Members = new ObservableCollection<Squad.MemberViewModel>();
        }

        public void ImportData(SaveData saveData)
        {
            this._Members.Clear();

            var inventory = this.Shell.Inventory;
            inventory.Items.Clear();
            inventory.RawItems.Clear();

            var agent = saveData.GetAgent<Agents.PartyManagerAgent>();
            if (agent == null)
            {
                return;
            }

            inventory.NextItemId = agent.NextItemId;

            for (int i = 0; i < agent.Squad.MemberSnapshots.Count; i++)
            {
                var snapshot = agent.Squad.MemberSnapshots[i];
                if (snapshot.CharacterId == -1)
                {
                    continue;
                }

                var rawData = agent.Squad.MemberRawData[i];
                if (rawData.DataBytes == null)
                {
                    continue;
                }

                var characterId = snapshot.CharacterId.ToString(CultureInfo.InvariantCulture);
                GameInfo.PartyMemberDefinition definition;
                if (GameInfo.InfoManager.PartyMembers.TryGetValue(characterId, out definition) == false)
                {
                    continue;
                }

                SaveFormats.Data.PartyMember data;
                var bitReader = new FileFormats.BitReader(rawData.DataBytes);
                try
                {
                    data = new SaveFormats.Data.PartyMember(definition.ExcludePresets, definition.ExcludeProfiles);
                    data.Read(bitReader);
                }
                catch (System.Exception)
                {
                    throw;
                }

                var viewModel = new Squad.MemberViewModel(snapshot.CharacterId == 0
                                                              ? inventory
                                                              : null,
                                                          i,
                                                          definition);
                viewModel.ImportData(snapshot, data);
                this._Members.Add(viewModel);
            }
        }

        public void ExportData(SaveData saveData)
        {
            var agent = saveData.GetAgent<Agents.PartyManagerAgent>();
            if (agent == null)
            {
                return;
            }

            var inventory = this.Shell.Inventory;
            agent.NextItemId = inventory.NextItemId;

            foreach (var viewModel in this._Members)
            {
                var i = viewModel.Index;

                SaveFormats.Data.PartyMemberSnapshot snapshot;
                SaveFormats.Data.PartyMember data;
                viewModel.ExportData(out snapshot, out data);

                var bitWriter = new FileFormats.BitWriter(0x1000);
                data.Write(bitWriter);

                var oldRawData = agent.Squad.MemberRawData[i];
                var rawData = new SaveFormats.Data.PartyMemberRawData();
                rawData.Unknown1 = oldRawData.Unknown1;
                rawData.Unknown2 = oldRawData.Unknown2;
                rawData.DataBytes = bitWriter.GetBytes();

                agent.Squad.MemberSnapshots[i] = snapshot;
                agent.Squad.MemberRawData[i] = rawData;
            }
        }
    }
}
