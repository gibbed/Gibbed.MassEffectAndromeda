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
using DbObject = Gibbed.Frostbite3.Common.DbObject;

namespace Gibbed.Frostbite3.VfsFormats.Layout
{
    public class InstallChunk
    {
        [DbObject.Property("id")]
        public Guid Id { get; set; }

        [DbObject.Property("name")]
        public string Name { get; set; }

        [DbObject.Property("installBundle")]
        public string InstallBundle { get; set; }

        [DbObject.Property("alwaysInstalled")]
        public bool AlwaysInstalled { get; set; }

        [DbObject.Property("mandatoryDLC")]
        public bool MandatoryDLC { get; set; }

        [DbObject.Property("optionalDLC")]
        public bool OptionalDLC { get; set; }

        [DbObject.Property("testDLC")]
        public bool TestDLC { get; set; }

        [DbObject.Property("license")]
        public string License { get; set; }

        [DbObject.Property("estimatedSize")]
        public long EstimatedSize { get; set; }

        [DbObject.Property("maxSizeMB")]
        public int MaxSizeMB { get; set; }

        [DbObject.Property("requiredChunks")]
        public List<Guid> RequiredChunks { get; set; }

        [DbObject.Property("superbundles")]
        public List<string> Superbundles { get; set; }

        [DbObject.Property("language")]
        public string Language { get; set; }

        [DbObject.Property("saveLocked")]
        public bool IsSaveLocked { get; set; }

        [DbObject.Property("friendlyName")]
        public int FriendlyName { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
