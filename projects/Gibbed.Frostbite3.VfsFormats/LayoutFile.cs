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

namespace Gibbed.Frostbite3.VfsFormats
{
    public class LayoutFile : DbObjectFile<LayoutFile>
    {
        [DbObject.Property("superBundles")]
        public List<Layout.SuperbundleInfo> Superbundles { get; set; }

        [DbObject.Property("fs")]
        public List<string> FileSystems { get; set; }

        [DbObject.Property("head")]
        public int Head { get; set; }

        [DbObject.Property("installManifest")]
        public Layout.InstallManifest InstallManifest { get; set; }
    }
}
