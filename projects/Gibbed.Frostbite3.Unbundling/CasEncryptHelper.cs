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

using System.IO;
using System.Linq;
using Gibbed.Frostbite3.VfsFormats;
using YamlDotNet.RepresentationModel;

namespace Gibbed.Frostbite3.Unbundling
{
    public static class CasEncryptHelper
    {
        public static void TryLoad(string path, ChunkLoader chunkLoader)
        {
            if (File.Exists(path) == false)
            {
                return;
            }

            var initFileSystem = InitFileSystemFile.Read(path);
            var entry = initFileSystem.FirstOrDefault(
                e => e.File.FileSystemName == null && e.File.Name == "Scripts/CasEncrypt.yaml");
            if (entry == null)
            {
                return;
            }

            using (var temp = new MemoryStream(entry.File.Payload, false))
            using (var reader = new StreamReader(temp))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
                var start = (YamlSequenceNode)mapping.Children[new YamlScalarNode("start")];
                foreach (var child in start.Children.OfType<YamlMappingNode>())
                {
                    var key = ((YamlScalarNode)child.Children[new YamlScalarNode("key")]).Value;
                    var keyId = ((YamlScalarNode)child.Children[new YamlScalarNode("keyid")]).Value;
                    chunkLoader.AddCryptoKey(keyId, key);
                }
            }
        }
    }
}
