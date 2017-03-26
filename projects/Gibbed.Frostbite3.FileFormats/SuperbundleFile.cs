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

namespace Gibbed.Frostbite3.FileFormats
{
    public class SuperbundleFile
    {
        public static SuperbundleFile Read(string path)
        {
            using (var input = File.OpenRead(path))
            {
                return LayerHelper.ReadObject(input, s => Read(s));
            }
        }

        public static SuperbundleFile Read(Stream input)
        {
            return new DbObject.Serializer().Deserialize<SuperbundleFile>(input);
        }

        [DbObject.Property("bundles")]
        public List<BundleEntry> Bundles { get; set; }

        public class BundleEntry
        {
            [DbObject.Property("path")]
            public string Path { get; set; }

            [DbObject.Property("magicSalt")]
            public int MagicSalt { get; set; }

            [DbObject.Property("ebx")]
            public List<EbxEntry> Ebx { get; set; }

            [DbObject.Property("dbx")]
            public List<DbxEntry> Dbx { get; set; }

            [DbObject.Property("res")]
            public List<ResourceEntry> Resources { get; set; }

            [DbObject.Property("chunks")]
            public List<ChunkEntry> Chunks { get; set; }

            [DbObject.Property("chunkMeta")]
            public List<ChunkMetaEntry> ChunkMeta { get; set; }

            [DbObject.Property("alignMembers")]
            public bool AlignMembers { get; set; }

            [DbObject.Property("ridSupport")]
            public bool ResourceIdSupport { get; set; }

            [DbObject.Property("storeCompressedSizes")]
            public bool StoreCompressedSizes { get; set; }

            [DbObject.Property("chunkBundleSize")]
            public long ChunkBundleSize { get; set; }

            [DbObject.Property("resBundleSize")]
            public long ResourceBundleSize { get; set; }

            [DbObject.Property("ebxBundleSize")]
            public long EbxBundleSize { get; set; }

            [DbObject.Property("dbxBundleSize")]
            public long DbxBundleSize { get; set; }

            [DbObject.Property("totalSize")]
            public long TotalSize { get; set; }

            [DbObject.Property("dbxTotalSize")]
            public long DbxTotalSize { get; set; }

            [DbObject.Property("bmm")]
            public List<int> Bmm { get; set; }
        }

        public interface IDataEntry
        {
            SHA1 SHA1 { get; }
            long Size { get; }
            long OriginalSize { get; }
            byte[] InlineData { get; }
        }

        public class EbxEntry : IDataEntry
        {
            [DbObject.Property("name")]
            public string Name { get; set; }

            [DbObject.Property("sha1")]
            public SHA1 SHA1 { get; set; }

            [DbObject.Property("size")]
            public long Size { get; set; }

            [DbObject.Property("originalSize")]
            public long OriginalSize { get; set; }

            [DbObject.Property("idata")]
            public byte[] InlineData { get; set; }
        }

        public class DbxEntry : IDataEntry
        {
            [DbObject.Property("name")]
            public string Name { get; set; }

            [DbObject.Property("sha1")]
            public SHA1 SHA1 { get; set; }

            [DbObject.Property("size")]
            public long Size { get; set; }

            [DbObject.Property("originalSize")]
            public long OriginalSize { get; set; }

            [DbObject.Property("idata")]
            public byte[] InlineData { get; set; }
        }

        public class ResourceEntry : IDataEntry
        {
            [DbObject.Property("name")]
            public string Name { get; set; }

            [DbObject.Property("sha1")]
            public SHA1 SHA1 { get; set; }

            [DbObject.Property("size")]
            public long Size { get; set; }

            [DbObject.Property("originalSize")]
            public long OriginalSize { get; set; }

            [DbObject.Property("resType")]
            public int ResourceType { get; set; }

            [DbObject.Property("resMeta")]
            public byte[] ResourceMeta { get; set; }

            [DbObject.Property("resRid")]
            public long ResourceId { get; set; }

            [DbObject.Property("idata")]
            public byte[] InlineData { get; set; }
        }

        public class ChunkEntry
        {
            [DbObject.Property("id")]
            public Guid Id { get; set; }

            [DbObject.Property("sha1")]
            public SHA1 SHA1 { get; set; }

            [DbObject.Property("size")]
            public int Size { get; set; }

            [DbObject.Property("rangeStart")]
            public int RangeStart { get; set; }

            [DbObject.Property("rangeEnd")]
            public int RangeEnd { get; set; }

            [DbObject.Property("bundledSize")]
            public int BundledSize { get; set; }

            [DbObject.Property("logicalOffset")]
            public int LogicalOffset { get; set; }

            [DbObject.Property("logicalSize")]
            public int LogicalSize { get; set; }

            [DbObject.Property("idata")]
            public byte[] InlineData { get; set; }

            public override string ToString()
            {
                return this.Id.ToString();
            }
        }

        public class ChunkMetaEntry
        {
            [DbObject.Property("h32")]
            public int H32 { get; set; }

            [DbObject.Property("meta")]
            public Dictionary<string, object> Meta { get; set; }
        }
    }
}
