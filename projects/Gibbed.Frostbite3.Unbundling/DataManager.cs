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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gibbed.Frostbite3.Common;
using Gibbed.Frostbite3.VfsFormats;
using Gibbed.IO;
using Layout = Gibbed.Frostbite3.VfsFormats.Layout;
using Superbundle = Gibbed.Frostbite3.VfsFormats.Superbundle;

namespace Gibbed.Frostbite3.Unbundling
{
    public class DataManager : IDisposable
    {
        private static readonly Regex _ChunkBundleRegex;

        static DataManager()
        {
            _ChunkBundleRegex = new Regex(@"/chunks\d+$", RegexOptions.Compiled);
        }

        internal static bool IsChunkBundle(string name)
        {
            return _ChunkBundleRegex.IsMatch(name) == true;
        }

        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        private readonly object _Lock;
        private readonly string _BasePath;
        private readonly List<DataSource> _Sources;
        private readonly Dictionary<string, SuperbundleFile> _Superbundles;
        private readonly Dictionary<string, TableOfContentsFile> _TableOfContents;
        private readonly Dictionary<Guid, InstallChunkManager> _InstallChunkManagers;
        private readonly object _CryptoKeyLock;
        private readonly Dictionary<string, byte[]> _CryptoKeys;

        private LayoutFile _PrimaryLayout
        {
            get
            {
                if (this._Sources.Count < 1)
                {
                    throw new InvalidOperationException();
                }
                return this._Sources[0].Layout;
            }
        }

        public IEnumerable<string> Superbundles
        {
            get { return this._PrimaryLayout.Superbundles.Select(sbi => sbi.Name); }
        }

        private DataManager(string basePath)
        {
            if (string.IsNullOrEmpty(basePath) == true)
            {
                throw new ArgumentNullException("basePath");
            }

            this._Lock = new object();
            this._BasePath = basePath;
            this._Sources = new List<DataSource>();
            this._Superbundles = new Dictionary<string, SuperbundleFile>();
            this._TableOfContents = new Dictionary<string, TableOfContentsFile>();
            this._InstallChunkManagers = new Dictionary<Guid, InstallChunkManager>();
            this._CryptoKeyLock = new object();
            this._CryptoKeys = new Dictionary<string, byte[]>();
        }

        ~DataManager()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this._InstallChunkManagers != null)
                {
                    foreach (var installChunkManager in this._InstallChunkManagers.Values)
                    {
                        installChunkManager.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static DataManager Initialize(string basePath, bool noPatch)
        {
            var instance = new DataManager(basePath);

            var dataLayout = instance.AddLayout("Data");
            if (dataLayout == null)
            {
                Logger.Error("Could not add default superbundle layout.");
                return null;
            }

            instance.AddUpdates(dataLayout, "Update");

            if (noPatch == false && instance.AddLayout("Patch") == null)
            {
                Logger.Warn("Could not add patch superbundle layout.");
            }

            instance.ReadFile("initfs_Win32", s => CasEncryptHelper.TryLoad(s, instance));
            instance.MountCommonSuperbundles();
            return instance;
        }

        internal void AddCryptoKey(string keyId, string keyText)
        {
            lock (this._CryptoKeyLock)
            {
                if (this._CryptoKeys.ContainsKey(keyId) == true)
                {
                    throw new InvalidOperationException();
                }

                var keyBytes = Helpers.GetBytesFromHexString(keyText);
                this._CryptoKeys.Add(keyId, keyBytes);
            }
        }

        private bool TryGetCryptoKey(string keyId, out byte[] keyBytes)
        {
            lock (this._CryptoKeyLock)
            {
                return this._CryptoKeys.TryGetValue(keyId, out keyBytes);
            }
        }

        private LayoutFile AddLayout(string name)
        {
            var dataPath = Path.Combine(this._BasePath, name);
            var layoutPath = Path.Combine(dataPath, "layout.toc");
            if (File.Exists(layoutPath) == false)
            {
                Logger.Debug("Could not load superbundle layout for '{0}'.", name);
                return null;
            }

            var layout = LayoutFile.Read(layoutPath);
            var index = this._Sources.FindLastIndex(s => s.Layout.Head > layout.Head);
            this._Sources.Insert(index < 0 ? 0 : (index + 1), new DataSource(layout, dataPath));
            return layout;
        }

        private void AddUpdates(LayoutFile layout, string name)
        {
            var updatesPath = Path.Combine(this._BasePath, name);

            foreach (var updatePath in Directory.GetDirectories(updatesPath))
            {
                var manifestPath = Path.Combine(updatePath, "package.mft");
                if (File.Exists(manifestPath) == false)
                {
                    continue;
                }

                var dataPath = Path.Combine(updatePath, "Data");
                var index = this._Sources.FindLastIndex(s => s.Layout.Head > layout.Head);
                this._Sources.Insert(index < 0 ? 0 : (index + 1), new DataSource(layout, dataPath));
            }
        }

        public void MountCommonSuperbundles()
        {
            // add common chunk bundles (chunks*.toc/sb)
            foreach (var superbundleInfo in this._PrimaryLayout.Superbundles.Where(
                sbi => IsChunkBundle(sbi.Name) == true))
            {
                if (this.MountSuperbundle(superbundleInfo.Name) == null)
                {
                    Logger.Warn("Failed to mount common superbundle '{0}'.", superbundleInfo.Name);
                }
            }
        }

        public SuperbundleFile MountSuperbundle(string name)
        {
            if (string.IsNullOrEmpty("name") == true)
            {
                throw new ArgumentNullException("name");
            }

            name = Helpers.FilterName(name).ToLowerInvariant();

            lock (this._Lock)
            {
                SuperbundleFile superbundle;
                if (this._Superbundles.TryGetValue(name, out superbundle) == true)
                {
                    return superbundle;
                }

                if (IsChunkBundle(name) == false)
                {
                    int count = 0;
                    foreach (var installChunk in this.GetRequiredInstallChunks(name))
                    {
                        if (this.MountInstallChunk(installChunk) == false)
                        {
                            continue;
                        }

                        count++;
                    }

                    if (count <= 0)
                    {
                        return null;
                    }
                }

                var superbundlePath = Helpers.FilterPath(name) + ".sb";
                superbundle = this.ReadFile(
                    superbundlePath,
                    s =>
                    {
                        Logger.Debug("Reading superbundle '{0}'", superbundlePath);
                        return SuperbundleFile.Read(s);
                    });
                if (superbundle == null)
                {
                    Logger.Warn("Failed to read superbundle '{0}'", superbundlePath);
                    return null;
                }

                var tableOfContentsPath = Helpers.FilterPath(name) + ".toc";
                var tableOfContents = this.ReadFile(
                    tableOfContentsPath,
                    s =>
                    {
                        Logger.Debug("Reading TOC '{0}'", tableOfContentsPath);
                        return TableOfContentsFile.Read(s);
                    });
                if (tableOfContents == null)
                {
                    Logger.Warn("Failed to read TOC '{0}'", tableOfContentsPath);
                    return null;
                }
                if (tableOfContents.IsCas == false)
                {
                    throw new NotSupportedException();
                }

                this._Superbundles[name] = superbundle;
                this._TableOfContents[name] = tableOfContents;
                return superbundle;
            }
        }

        private IEnumerable<Layout.InstallChunk> GetRequiredInstallChunks(string name)
        {
            var layout = this._PrimaryLayout;
            var lookup = layout.InstallManifest.InstallChunks.ToDictionary(ic => ic.Id, ic => ic);

            var completed = new List<Guid>();
            var queue = new Queue<Layout.InstallChunk>();

            foreach (var installChunk in layout.InstallManifest.InstallChunks.Where(
                ic => ic.Superbundles != null &&
                      ic.Superbundles.Any(sbi => Helpers.CompareName(name, sbi) == 0) == true))
            {
                yield return installChunk;
                queue.Enqueue(installChunk);
            }

            while (queue.Count > 0)
            {
                var installChunk = queue.Dequeue();
                foreach (var requiredInstallChunkId in installChunk.RequiredChunks.Except(completed))
                {
                    Layout.InstallChunk requiredInstallChunk;
                    if (lookup.TryGetValue(requiredInstallChunkId, out requiredInstallChunk) == false)
                    {
                        continue;
                    }
                    yield return requiredInstallChunk;
                    completed.Add(requiredInstallChunkId);
                    queue.Enqueue(requiredInstallChunk);
                }
            }
        }

        private bool MountInstallChunk(Layout.InstallChunk installChunk)
        {
            if (this._InstallChunkManagers.ContainsKey(installChunk.Id) == true)
            {
                return true;
            }

            var manager = InstallChunkManager.Initialize(this.TryGetCryptoKey, installChunk, this._Sources);
            if (manager == null)
            {
                return false;
            }

            this._InstallChunkManagers.Add(installChunk.Id, manager);
            return true;
        }

        public bool GetChunkId(Guid guid, out SHA1Hash sha1, out long size)
        {
            var superbundleChunkInfo =
                this._Superbundles
                    .Values
                    .Where(sb => sb.Bundles != null)
                    .SelectMany(sb => sb.Bundles)
                    .Where(bi => bi.Chunks != null)
                    .SelectMany(bi => bi.Chunks)
                    .FirstOrDefault(ci => ci.Id == guid);
            if (superbundleChunkInfo != null)
            {
                sha1 = superbundleChunkInfo.SHA1;
                size = superbundleChunkInfo.Size;
                return true;
            }

            var tableOfContentsChunkInfo =
                this._TableOfContents
                    .Values
                    .Where(toc => toc.Chunks != null)
                    .SelectMany(toc => toc.Chunks)
                    .FirstOrDefault(ci => ci.Id == guid);
            if (tableOfContentsChunkInfo != null)
            {
                sha1 = tableOfContentsChunkInfo.SHA1;
                size = 0;
                return true;
            }

            sha1 = default(SHA1Hash);
            size = 0;
            return false;
        }

        public void LoadData(Superbundle.IDataInfo dataInfo, Stream output)
        {
            if (dataInfo.InlineData != null)
            {
                if (dataInfo.Size == dataInfo.OriginalSize)
                {
                    if (dataInfo.InlineData.Length != dataInfo.Size)
                    {
                        throw new InvalidOperationException();
                    }

                    output.WriteBytes(dataInfo.InlineData);
                    return;
                }

                using (var temp = new MemoryStream(dataInfo.InlineData, false))
                {
                    CompressionHelper.Decompress(temp, output, dataInfo.OriginalSize);
                }
                return;
            }

            foreach (var installChunkManager in this._InstallChunkManagers.Values)
            {
                if (installChunkManager.LoadData(dataInfo, output) == true)
                {
                    return;
                }
            }

            throw new DataLoadException(string.Format("could not find chunk '{0}'", dataInfo.SHA1));
        }

        public bool LoadChunk(SHA1Hash id, long variantSize, long originalSize, Stream output)
        {
            foreach (var installChunkManager in this._InstallChunkManagers.Values)
            {
                if (installChunkManager.LoadChunk(id, variantSize, originalSize, output) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReadFile(string path, Action<string> action)
        {
            foreach (var source in this._Sources)
            {
                var localPath = Path.Combine(source.Path, path);
                if (File.Exists(localPath) == false)
                {
                    continue;
                }
                action(localPath);
                break;
            }
        }

        private T ReadFile<T>(string path, Func<string, T> func)
        {
            foreach (var source in this._Sources)
            {
                var localPath = Path.Combine(source.Path, path);
                if (File.Exists(localPath) == false)
                {
                    continue;
                }
                return func(localPath);
            }
            return default(T);
        }
    }
}
