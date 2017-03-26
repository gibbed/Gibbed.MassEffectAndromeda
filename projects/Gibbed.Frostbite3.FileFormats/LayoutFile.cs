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
    public class LayoutFile
    {
        public static LayoutFile Read(string path)
        {
            using (var input = File.OpenRead(path))
            {
                return LayerHelper.ReadObject(input, s => Read(s));
            }
        }

        public static LayoutFile Read(Stream input)
        {
            return new DbObject.Serializer().Deserialize<LayoutFile>(input);
        }

        [DbObject.Property("superBundles")]
        public List<SuperBundleInfo> Superbundles { get; set; }

        [DbObject.Property("fs")]
        public List<string> FileSystems { get; set; }

        [DbObject.Property("head")]
        public int Head { get; set; }

        [DbObject.Property("installManifest")]
        public InstallManifestInfo InstallManifest { get; set; }

        public class SuperBundleInfo
        {
            [DbObject.Property("name")]
            public string Name { get; set; }
        }

        public class InstallManifestInfo
        {
            [DbObject.Property("installChunks")]
            public List<InstallChunkInfo> InstallChunks { get; set; }

            [DbObject.Property("installGroups")]
            public List<InstallGroupInfo> InstallGroups { get; set; }

            [DbObject.Property("installScenarioConfig")]
            public InstallScenarioConfigInfo InstallScenarioConfig { get; set; }

            [DbObject.Property("excludedSuperbundles")]
            public List<object> ExcludedSuperbundles { get; set; }

            [DbObject.Property("enableDuplication")]
            public bool EnableDuplication { get; set; }

            [DbObject.Property("totalSizeAsDelta")]
            public bool TotalSizeAsDelta { get; set; }

            [DbObject.Property("totalSizeMB")]
            public int TotalSize { get; set; }

            [DbObject.Property("enableStoreBmm")]
            public bool EnableStoreBmm { get; set; }
        }

        public class InstallChunkInfo
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

        public class InstallGroupInfo
        {
            [DbObject.Property("name")]
            public string Name { get; set; }

            [DbObject.Property("chunks")]
            public List<Guid> Chunks { get; set; }
        }

        public class InstallScenarioConfigInfo
        {
            [DbObject.Property("defaultScenarioId")]
            public Guid DefaultScenarioId { get; set; }

            [DbObject.Property("installScenarios")]
            public List<InstallScenarioInfo> InstallScenarios { get; set; }
        }

        public class InstallScenarioInfo
        {
            [DbObject.Property("id")]
            public Guid Id { get; set; }

            [DbObject.Property("name")]
            public string Name { get; set; }

            [DbObject.Property("scenarioType")]
            public string ScenarioType { get; set; }

            [DbObject.Property("alwaysInstalled")]
            public bool AlwaysInstalled { get; set; }

            [DbObject.Property("numberOfLaunchPackages")]
            public int NumberOfLaunchPackages { get; set; }

            [DbObject.Property("installPackages")]
            public List<InstallPackageInfo> InstallPackages { get; set; }
        }

        public class InstallPackageInfo
        {
            [DbObject.Property("id")]
            public Guid Id { get; set; }

            [DbObject.Property("name")]
            public string Name { get; set; }
        }
    }
}
