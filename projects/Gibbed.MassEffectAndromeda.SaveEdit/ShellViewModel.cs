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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using Gibbed.MassEffectAndromeda.SaveEdit.Core;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveEdit
{
    [Export(typeof(ShellViewModel))]
    internal class ShellViewModel : PropertyChangedBase
    {
        #region Imports
        private GeneralViewModel _General;
        private CustomizationViewModel _Customization;
        private SquadViewModel _Squad;
        private InventoryViewModel _Inventory;
        private PlotViewModel _Plot;
        private AboutViewModel _About;

        [Import(typeof(GeneralViewModel))]
        public GeneralViewModel General
        {
            get { return this._General; }

            set
            {
                this._General = value;
                this.NotifyOfPropertyChange(() => this.General);
            }
        }

        [Import(typeof(CustomizationViewModel))]
        public CustomizationViewModel Customization
        {
            get { return this._Customization; }

            set
            {
                this._Customization = value;
                this.NotifyOfPropertyChange(() => this.Customization);
            }
        }

        [Import(typeof(SquadViewModel))]
        public SquadViewModel Squad
        {
            get { return this._Squad; }

            set
            {
                this._Squad = value;
                this.NotifyOfPropertyChange(() => this.Squad);
            }
        }

        [Import(typeof(InventoryViewModel))]
        public InventoryViewModel Inventory
        {
            get { return this._Inventory; }

            set
            {
                this._Inventory = value;
                this.NotifyOfPropertyChange(() => this.Inventory);
            }
        }

        [Import(typeof(PlotViewModel))]
        public PlotViewModel Plot
        {
            get { return this._Plot; }

            set
            {
                this._Plot = value;
                this.NotifyOfPropertyChange(() => this.Plot);
            }
        }

        [Import(typeof(AboutViewModel))]
        public AboutViewModel About
        {
            get { return this._About; }

            set
            {
                this._About = value;
                this.NotifyOfPropertyChange(() => this.About);
            }
        }
        #endregion

        #region Fields
        private SaveLoad _SaveLoad;
        private SaveFormats.SaveFile _SaveFile;

        private bool _IsGeneralSelected;
        private bool _IsFirstAboutSelection;
        private bool _IsAboutSelected;
        #endregion

        #region Properties
        public SaveFormats.SaveFile SaveFile
        {
            get { return this._SaveFile; }
            private set
            {
                if (this._SaveFile != value)
                {
                    this._SaveFile = value;
                    this.NotifyOfPropertyChange(() => this.SaveFile);
                }
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

        public bool IsGeneralSelected
        {
            get { return this._IsGeneralSelected; }
            set
            {
                if (this._IsGeneralSelected != value)
                {
                    this._IsGeneralSelected = value;
                    this.NotifyOfPropertyChange(() => this.IsGeneralSelected);
                }
            }
        }

        public bool IsAboutSelected
        {
            get { return this._IsAboutSelected; }
            set
            {
                if (this._IsAboutSelected != value)
                {
                    this._IsFirstAboutSelection = false;
                    this._IsAboutSelected = value;
                    this.NotifyOfPropertyChange(() => this.IsAboutSelected);
                }
            }
        }
        #endregion

        [ImportingConstructor]
        public ShellViewModel()
        {
            this._IsAboutSelected = true;
            this._IsFirstAboutSelection = true;
        }

        private void MaybeSwitchToGeneral()
        {
            if (this.IsAboutSelected == true && this._IsFirstAboutSelection == true)
            {
                this.IsGeneralSelected = true;
            }
        }

        public IEnumerable<IResult> NewSave()
        {
            yield return new DelegateResult(
                () => { throw new NotSupportedException(); })
                .Rescue<SaveFormats.SaveFormatException>(
                    x => new MyMessageBox("Failed to create save: " + x.Message, "Error")
                             .WithIcon(MessageBoxImage.Error))
                .Rescue<SaveFormats.SaveCorruptionException>(
                    x => new MyMessageBox("Failed to create save: " + x.Message, "Error")
                             .WithIcon(MessageBoxImage.Error))
                .Rescue(
                    x => new MyMessageBox("An exception was thrown " +
                                          "(press Ctrl+C to copy):\n\n" + x.ToString(),
                                          "Error")
                             .WithIcon(MessageBoxImage.Error));
        }

        public IEnumerable<IResult> ReadSave()
        {
            string fileName = null;
            int filterIndex = -1;

            foreach (var result in this.SaveLoad.OpenFile(fn => fileName = fn, fi => filterIndex = fi))
            {
                yield return result;
            }

            if (fileName == null)
            {
                yield break;
            }

            SaveFormats.SaveFile saveFile = null;

            if (filterIndex == 2)
            {
                var result = MessageBoxResult.No;
                yield return
                    new MyMessageBox(
                        "There is no guarantee an import from a raw save file will work correctly.\n" +
                        "Are you sure you want to proceed?",
                        "Warning")
                        .WithButton(MessageBoxButton.YesNo)
                        .WithDefaultResult(MessageBoxResult.Yes)
                        .WithResultDo(r => result = r)
                        .WithIcon(MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    yield break;
                }
            }

            yield return new DelegateResult(
                () =>
                {
                    if (filterIndex <= 1)
                    {
                        using (var input = File.OpenRead(fileName))
                        {
                            saveFile = SaveFormats.SaveFile.Read(input);
                        }
                    }
                    else if (filterIndex == 2)
                    {
                        using (var input = File.OpenRead(fileName))
                        using (var textReader = new StreamReader(input))
                        using (var reader = new JsonTextReader(textReader))
                        {
                            var serializer = new JsonSerializer();
                            serializer.MissingMemberHandling = MissingMemberHandling.Error;
                            serializer.TypeNameHandling = TypeNameHandling.Auto;
                            saveFile = serializer.Deserialize<SaveFormats.SaveFile>(reader);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("unsupported save type");
                    }

                    try
                    {
                        this.General.ImportData(saveFile.Data);
                        this.Customization.ImportData(saveFile.Data);
                        this.Squad.ImportData(saveFile.Data);
                        this.Plot.ImportData(saveFile.Data);
                        this.SaveFile = saveFile;
                        this.MaybeSwitchToGeneral();
                    }
                    catch (Exception)
                    {
                        this.SaveFile = null;
                        throw;
                    }
                })
                .Rescue<DllNotFoundException>(
                    x => new MyMessageBox("Failed to load save: " + x.Message, "Error")
                             .WithIcon(MessageBoxImage.Error))
                .Rescue<SaveFormats.SaveFormatException>(
                    x => new MyMessageBox("Failed to load save: " + x.Message, "Error")
                             .WithIcon(MessageBoxImage.Error))
                .Rescue<SaveFormats.SaveCorruptionException>(
                    x => new MyMessageBox("Failed to load save: " + x.Message, "Error")
                             .WithIcon(MessageBoxImage.Error))
                .Rescue(
                    x => new MyMessageBox("An exception was thrown " +
                                          "(press Ctrl+C to copy):\n\n" + x.ToString(),
                                          "Error")
                             .WithIcon(MessageBoxImage.Error));
        }

        public IEnumerable<IResult> WriteSave()
        {
            if (this.SaveFile == null)
            {
                yield break;
            }

            string fileName = null;
            int filterIndex = -1;

            foreach (var result in this.SaveLoad.SaveFile(fn => fileName = fn, fi => filterIndex = fi))
            {
                yield return result;
            }

            if (fileName == null)
            {
                yield break;
            }

            var saveFile = this.SaveFile;

            yield return new DelegateResult(
                () =>
                {
                    this.General.ExportData(saveFile.Data);
                    this.Customization.ExportData(saveFile.Data);
                    this.Squad.ExportData(saveFile.Data);
                    this.Plot.ExportData(saveFile.Data);

                    if (filterIndex <= 1)
                    {
                        using (var output = File.Create(fileName))
                        {
                            saveFile.Serialize(output);
                        }
                    }
                    else if (filterIndex == 2)
                    {
                        using (var output = File.Create(fileName))
                        using (var textWriter = new StreamWriter(output))
                        using (var writer = new JsonTextWriter(textWriter))
                        {
                            writer.Formatting = Formatting.Indented;
                            writer.IndentChar = ' ';
                            writer.Indentation = 2;
                            var serializer = new JsonSerializer();
                            serializer.TypeNameHandling = TypeNameHandling.Auto;
                            serializer.Serialize(writer, saveFile);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("unsupported save type");
                    }
                }).Rescue(
                    x => new MyMessageBox("An exception was thrown " +
                                          "(press Ctrl+C to copy):\n\n" + x.ToString(),
                                          "Error")
                             .WithIcon(MessageBoxImage.Error));
        }
    }
}
