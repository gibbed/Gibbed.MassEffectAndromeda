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
using Gibbed.Frostbite3.Common;
using Gibbed.Frostbite3.VfsFormats;
using Superbundle = Gibbed.Frostbite3.VfsFormats.Superbundle;

namespace Gibbed.Frostbite3.UnpackResources
{
    internal class CatalogLookup
    {
        private readonly string _DataPath;
        private readonly Dictionary<string, CatalogInfo> _CatalogInfo;
        private readonly Dictionary<string, List<EntryInfo>> _EntryInfo;

        public CatalogLookup(string dataPath)
        {
            if (dataPath == null)
            {
                throw new ArgumentNullException("dataPath");
            }

            this._DataPath = dataPath;
            this._CatalogInfo = new Dictionary<string, CatalogInfo>();
            this._EntryInfo = new Dictionary<string, List<EntryInfo>>();
        }

        public bool Add(string path)
        {
            if (this._CatalogInfo.ContainsKey(path) == true)
            {
                return true;
            }

            var basePath = Path.Combine(this._DataPath, Helpers.FilterPath(path));
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

            var catalogInfo = new CatalogInfo(basePath);
            this._CatalogInfo.Add(path, catalogInfo);

            foreach (var entry in catalog.ChunkEntries)
            {
                var info = new EntryInfo(catalogInfo, entry);

                List<EntryInfo> entryInfos;
                if (this._EntryInfo.TryGetValue(entry.SHA1.Text, out entryInfos) == false)
                {
                    entryInfos = this._EntryInfo[entry.SHA1.Text] = new List<EntryInfo>();
                }

                entryInfos.Add(info);
            }

            return true;
        }

        public ICatalogEntryInfo GetEntry(Superbundle.IDataInfo entry)
        {
            List<EntryInfo> infos;
            if (this._EntryInfo.TryGetValue(entry.SHA1.Text, out infos) == false)
            {
                return null;
            }

            foreach (var candidate in infos)
            {
                if (candidate.CompressedSize == entry.Size &&
                    (candidate.UncompressedSize == 0 || candidate.UncompressedSize == entry.OriginalSize))
                {
                    return candidate;
                }
            }

            return null;
        }

        public ICatalogEntryInfo GetEntry(SHA1 sha1, long originalSize)
        {
            List<EntryInfo> infos;
            if (this._EntryInfo.TryGetValue(sha1.Text, out infos) == false)
            {
                return null;
            }

            foreach (var candidate in infos)
            {
                if ((candidate.UncompressedSize == 0 || candidate.UncompressedSize == originalSize))
                {
                    return candidate;
                }
            }

            return null;
        }

        private struct EntryInfo : ICatalogEntryInfo
        {
            private CatalogInfo _Catalog;
            private CatalogFile.ChunkEntry _Entry;

            public EntryInfo(CatalogInfo catalog, CatalogFile.ChunkEntry entry)
            {
                this._Catalog = catalog;
                this._Entry = entry;
            }

            public string DataPath
            {
                get { return this._Catalog.GetDataPath(this._Entry.DataIndex); }
            }

            public uint Offset
            {
                get { return this._Entry.Offset; }
            }

            public uint CompressedSize
            {
                get { return this._Entry.Size; }
            }

            public uint UncompressedSize
            {
                get { return this._Entry.TailSize; }
            }
        }

        private class CatalogInfo
        {
            private readonly string _BasePath;

            public CatalogInfo(string basePath)
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
