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
using DbObject = Gibbed.Frostbite3.Common.DbObject;

namespace Gibbed.Frostbite3.VfsFormats.Superbundle
{
    public class BundleInfo
    {
        [DbObject.Property("path")]
        public string Path { get; set; }

        [DbObject.Property("magicSalt")]
        public int MagicSalt { get; set; }

        [DbObject.Property("ebx")]
        public List<EbxInfo> Ebx { get; set; }

        [DbObject.Property("dbx")]
        public List<DbxInfo> Dbx { get; set; }

        [DbObject.Property("res")]
        public List<ResourceInfo> Resources { get; set; }

        [DbObject.Property("chunks")]
        public List<ChunkInfo> Chunks { get; set; }

        [DbObject.Property("chunkMeta")]
        public List<ChunkMetadata> ChunkMeta { get; set; }

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
}
