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

using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Gibbed.MassEffectAndromeda.SaveEdit.Core
{
    [Export(typeof(GeneralViewModel))]
    internal class GeneralViewModel : PropertyChangedBase
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
        public GeneralViewModel()
        {
        }

        public void ImportData(SaveFormats.SaveData saveData)
        {
        }

        public void ExportData(SaveFormats.SaveData saveData)
        {
        }
    }
}
