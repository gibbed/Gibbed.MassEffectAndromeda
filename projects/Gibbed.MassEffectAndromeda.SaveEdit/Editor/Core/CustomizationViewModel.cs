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
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using CustomizationAgent = Gibbed.MassEffectAndromeda.SaveFormats.ComponentContainerAgents.CustomizationAgent;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Core
{
    [Export(typeof(CustomizationViewModel))]
    internal class CustomizationViewModel : PropertyChangedBase
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
        #endregion

        #region Properties
        #endregion

        [ImportingConstructor]
        public CustomizationViewModel()
        {
        }

        public void ImportData(SaveFormats.SaveData saveData)
        {
        }

        public void ExportData(SaveFormats.SaveData saveData)
        {
        }

        public IEnumerable<IResult> DoExportSlot1()
        {
            var agent = this.Shell.SaveFile.Data.GetComponentContainerAgent<CustomizationAgent>();
            if (agent == null)
            {
                yield return new MyMessageBox("Customization data appears to be missing.", "Error")
                    .WithIcon(MessageBoxImage.Error);
                yield break;
            }

            CustomizationAgent.ComponentContainer container;
            if (agent.ComponentContainers.TryGetValue(1, out container) == false)
            {
                yield return new MyMessageBox("Slot #1 customization data appears to be missing.", "Error")
                    .WithIcon(MessageBoxImage.Error);
                yield break;
            }

            byte[] dataBytes;
            using (var data = new MemoryStream())
            {
            }
        }
    }
}
