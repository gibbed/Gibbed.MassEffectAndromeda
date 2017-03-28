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
using Superbundle = Gibbed.Frostbite3.VfsFormats.Superbundle;

namespace Gibbed.Frostbite3.UnpackResources
{
    internal class ChunkLookup
    {
        public static readonly Regex ChunkBundleRegex;

        static ChunkLookup()
        {
            ChunkBundleRegex = new Regex(@"/chunks\d+$", RegexOptions.Compiled);
        }

        public static bool IsChunkBundle(string name)
        {
            return ChunkBundleRegex.IsMatch(name) == true;
        }

        private readonly object _Lock;
        private readonly LayoutFile _Layout;
        private readonly string _DataPath;
        private readonly Dictionary<string, SuperbundleFile> _Superbundles;
        private readonly Dictionary<string, TableOfContentsFile> _TableOfContents;
        private readonly Dictionary<Guid, InstallChunkInfo> _InstallChunkInfo;
        private readonly Dictionary<string, List<ChunkVariantInfo>> _ChunkInfo;

        public ChunkLookup(LayoutFile layout, string dataPath)
        {
            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            if (dataPath == null)
            {
                throw new ArgumentNullException("dataPath");
            }

            this._Lock = new object();
            this._Layout = layout;
            this._DataPath = dataPath;
            this._Superbundles = new Dictionary<string, SuperbundleFile>();
            this._TableOfContents = new Dictionary<string, TableOfContentsFile>();
            this._InstallChunkInfo = new Dictionary<Guid, InstallChunkInfo>();
            this._ChunkInfo = new Dictionary<string, List<ChunkVariantInfo>>();
        }

        public SuperbundleFile AddBundle(string name)
        {
            if (name == null)
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
                    var installChunks = this._Layout.InstallManifest.InstallChunks.Where(
                        ic => ic.Superbundles != null &&
                              ic.Superbundles.Any(sbi => Helpers.CompareName(name, sbi) == 0) == true);

                    int count = 0;
                    foreach (var installChunk in installChunks)
                    {
                        if (this.AddInstallChunk(installChunk) == false)
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

                var superbundlePath = Path.Combine(this._DataPath, Helpers.FilterPath(name) + ".sb");
                var tableOfContentsPath = Path.Combine(this._DataPath, Helpers.FilterPath(name) + ".toc");
                if (File.Exists(superbundlePath) == false || File.Exists(tableOfContentsPath) == false)
                {
                    return null;
                }

                var tableOfContents = TableOfContentsFile.Read(tableOfContentsPath);
                if (tableOfContents.IsCas == false)
                {
                    throw new NotSupportedException();
                }

                superbundle = SuperbundleFile.Read(superbundlePath);

                this._Superbundles[name] = superbundle;
                this._TableOfContents[name] = tableOfContents;
                return superbundle;
            }
        }

        private bool AddInstallChunk(VfsFormats.Layout.InstallChunk installChunk)
        {
            if (this._InstallChunkInfo.ContainsKey(installChunk.Id) == true)
            {
                return true;
            }

            var basePath = Path.Combine(this._DataPath, Helpers.FilterPath(installChunk.InstallBundle));
            var catalogPath = Path.Combine(basePath, "cas.cat");
            if (File.Exists(catalogPath) == false)
            {
                return false;
            }

            var catalog = CatalogFile.Read(catalogPath);

            if (catalog.Unknown3s.Count > 0)
            {
                Console.WriteLine("'{0}' has unknown3s.", catalogPath);
            }

            var installChunkInfo = new InstallChunkInfo(basePath);
            foreach (var entry in catalog.ChunkEntries)
            {
                var variantInfo = new ChunkVariantInfo(installChunkInfo, entry);

                List<ChunkVariantInfo> chunkVariants;
                if (this._ChunkInfo.TryGetValue(entry.SHA1.Text, out chunkVariants) == false)
                {
                    chunkVariants = this._ChunkInfo[entry.SHA1.Text] = new List<ChunkVariantInfo>();
                }

                chunkVariants.Add(variantInfo);
            }
            this._InstallChunkInfo.Add(installChunk.Id, installChunkInfo);
            return true;
        }

        public bool GetChunkSHA1(Guid id, out SHA1 sha1, out long size)
        {
            foreach (var superbundle in this._Superbundles.Values)
            {
                if (superbundle.Bundles == null)
                {
                    continue;
                }

                foreach (var bundleInfo in superbundle.Bundles)
                {
                    if (bundleInfo.Chunks == null)
                    {
                        continue;
                    }

                    foreach (var chunkInfo in bundleInfo.Chunks)
                    {
                        if (chunkInfo.Id == id)
                        {
                            sha1 = chunkInfo.SHA1;
                            size = chunkInfo.Size;
                            return true;
                        }
                    }
                }
            }

            foreach (var tableOfContents in this._TableOfContents.Values)
            {
                if (tableOfContents.Chunks == null)
                {
                    continue;
                }

                foreach (var chunkInfo in tableOfContents.Chunks)
                {
                    if (chunkInfo.Id == id)
                    {
                        sha1 = chunkInfo.SHA1;
                        size = 0;
                        return true;
                    }
                }
            }

            sha1 = default(SHA1);
            size = 0;
            return false;
        }

        public IChunkVariantInfo GetChunkVariant(Superbundle.ResourceInfo resourceInfo)
        {
            return this.GetChunkVariant(resourceInfo.SHA1, resourceInfo.Size);
        }

        public IChunkVariantInfo GetChunkVariant(SHA1 sha1, long variantSize)
        {
            List<ChunkVariantInfo> variantInfos;
            if (this._ChunkInfo.TryGetValue(sha1.Text, out variantInfos) == false)
            {
                return null;
            }

            foreach (var candidate in variantInfos)
            {
                if ((variantSize == 0 && candidate.TailSize == 0) ||
                    candidate.Size == variantSize)
                {
                    return candidate;
                }
            }

            return null;
        }

        public interface IChunkVariantInfo
        {
            string DataPath { get; }
            uint Offset { get; }
            uint Size { get; }
            uint TailSize { get; }
        }

        private struct ChunkVariantInfo : IChunkVariantInfo
        {
            private InstallChunkInfo _InstallChunkInfo;
            private CatalogFile.ChunkEntry _Entry;

            public ChunkVariantInfo(InstallChunkInfo installChunkInfo, CatalogFile.ChunkEntry entry)
            {
                this._InstallChunkInfo = installChunkInfo;
                this._Entry = entry;
            }

            public string DataPath
            {
                get { return this._InstallChunkInfo.GetDataPath(this._Entry.DataIndex); }
            }

            public uint Offset
            {
                get { return this._Entry.Offset; }
            }

            public uint Size
            {
                get { return this._Entry.Size; }
            }

            public uint TailSize
            {
                get { return this._Entry.TailSize; }
            }
        }

        private class InstallChunkInfo
        {
            private readonly string _BasePath;

            public InstallChunkInfo(string basePath)
            {
                this._BasePath = basePath;
            }

            public string GetDataPath(uint index)
            {
                var name = string.Format("cas_{0:D2}.cas", index);
                return Path.Combine(this._BasePath, name);
            }
        }
    }
}
