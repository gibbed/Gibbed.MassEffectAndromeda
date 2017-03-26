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
using Gibbed.Frostbite3.FileFormats;
using NDesk.Options;

namespace Gibbed.Frostbite3.UnpackResources
{
    public class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool convertTextures = false;
            bool showHelp = false;

            var options = new OptionSet()
            {
                { "convert-textures", "convert textures", v => convertTextures = v != null },
                { "h|help", "show this message and exit", v => showHelp = v != null },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_[sb|toc] [output_dir]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string bundlePath, superbundlePath, layoutPath, outputBasePath;

            if (Path.GetExtension(extras[0]) == ".sb")
            {
                superbundlePath = Path.GetFullPath(extras[0]);
                bundlePath = Path.ChangeExtension(superbundlePath, ".toc");
                layoutPath = Helpers.FindLayoutPath(superbundlePath);
                outputBasePath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(superbundlePath, null) + "_unpack";
            }
            else
            {
                bundlePath = Path.GetFullPath(extras[0]);
                superbundlePath = Path.ChangeExtension(bundlePath, ".sb");
                layoutPath = Helpers.FindLayoutPath(bundlePath);
                outputBasePath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(bundlePath, null) + "_unpack";
            }

            if (string.IsNullOrEmpty(layoutPath) == true)
            {
                Console.WriteLine("Could not find layout file.");
                return;
            }

            var dataPath = Path.GetDirectoryName(layoutPath) ?? "";

            var bundle = TableOfContentsFile.Read(bundlePath);
            var superbundle = SuperbundleFile.Read(superbundlePath);

            var extensionsById = ResourceTypes.GetExtensions();

            if (bundle.IsCas == false)
            {
                throw new NotImplementedException();
            }
            else
            {
                var commonBundlePaths = Directory.GetFiles(dataPath, "chunks*.toc", SearchOption.AllDirectories);
                var commonBundles = new List<TableOfContentsFile>();
                foreach (var commonBundlePath in commonBundlePaths)
                {
                    var commonBundle = TableOfContentsFile.Read(commonBundlePath);
                    commonBundles.Add(commonBundle);
                }

                var superbundleName = Path.ChangeExtension(superbundlePath.Substring(dataPath.Length + 1), null);
                superbundleName = Helpers.FilterName(superbundleName).ToLowerInvariant();

                var layout = LayoutFile.Read(layoutPath);
                var installChunks = GetSuperbundleInstallChunks(layout, superbundleName);
                var catalogLookup = new CatalogLookup(dataPath);

                foreach (var installChunk in installChunks)
                {
                    if (catalogLookup.Add(installChunk.InstallBundle) == false)
                    {
                        Console.WriteLine("Failed to load catalog for '{0}'.", installChunk.Name);
                    }
                }

                foreach (var bundleInfo in superbundle.Bundles)
                {
                    if (bundleInfo.Resources == null)
                    {
                        continue;
                    }

                    foreach (var resourceInfo in bundleInfo.Resources)
                    {
                        var entry = catalogLookup.GetEntry(resourceInfo);
                        if (entry == null)
                        {
                            Console.WriteLine("Could not find catalog entry for '{0}'.", resourceInfo.Name);
                            continue;
                        }

                        if (entry.CompressedSize != resourceInfo.Size)
                        {
                            throw new FormatException();
                        }

                        var outputName = Helpers.FilterPath(resourceInfo.Name);
                        var outputPath = Path.Combine(outputBasePath, outputName + ".dummy");
                        var outputParentPath = Path.GetDirectoryName(outputPath);
                        if (string.IsNullOrEmpty(outputParentPath) == false)
                        {
                            Directory.CreateDirectory(outputParentPath);
                        }

                        Console.WriteLine("{0}", resourceInfo.Name);

                        bool wasConverted = false;
                        if (convertTextures == true && resourceInfo.ResourceType == ResourceTypes.Texture)
                        {
                            wasConverted = UnpackTexture(bundleInfo,
                                                         resourceInfo,
                                                         entry,
                                                         outputPath,
                                                         catalogLookup,
                                                         commonBundles);
                        }

                        if (wasConverted == false)
                        {
                            string extension;
                            if (extensionsById.TryGetValue(resourceInfo.ResourceType, out extension) == true)
                            {
                                extension = "." + extension;
                            }
                            else
                            {
                                extension = ".#" + resourceInfo.ResourceType.ToString("X8");
                            }

                            outputPath = Path.Combine(outputBasePath, outputName + extension);
                            using (var output = File.Create(outputPath))
                            {
                                Extraction.Extract(resourceInfo, entry, output);
                            }
                        }
                    }
                }
            }
        }

        private static bool UnpackTexture(SuperbundleFile.BundleEntry bundleInfo,
                                          SuperbundleFile.ResourceEntry resourceInfo,
                                          ICatalogEntryInfo entry,
                                          string outputPath,
                                          CatalogLookup catalogLookup,
                                          List<TableOfContentsFile> commonBundles)
        {
            TextureHeader textureHeader;
            using (var temp = new MemoryStream())
            {
                Extraction.Extract(resourceInfo, entry, temp);
                temp.Position = 0;
                textureHeader = TextureHeader.Read(temp);
                if (temp.Position != temp.Length)
                {
                    throw new FormatException();
                }
            }

            if (textureHeader.Type != TextureType._2d)
            {
                return false;
            }

            if (textureHeader.Unknown00 != 0 ||
                textureHeader.Unknown04 != 0 ||
                textureHeader.Unknown10 != 0 ||
                textureHeader.Unknown14 != 0 ||
                textureHeader.Unknown1C != 1)
            {
                throw new FormatException();
            }

            SHA1 chunkSHA1;
            if (GetChunkSHA1(bundleInfo, commonBundles, textureHeader.ChunkId, out chunkSHA1) == false)
            {
                throw new InvalidOperationException();
            }

            var dataEntry = catalogLookup.GetEntry(chunkSHA1, textureHeader.ChunkSize);
            byte[] dataBytes;
            using (var temp = new MemoryStream())
            {
                Extraction.Extract(dataEntry, textureHeader.ChunkSize, temp);
                temp.Position = 0;
                dataBytes = temp.GetBuffer();
            }

            DDSUtils.WriteFile(textureHeader, dataBytes, outputPath + ".dds");
            return true;
        }

        private static bool GetChunkSHA1(SuperbundleFile.BundleEntry bundleInfo,
                                         List<TableOfContentsFile> commonBundles,
                                         Guid chunkId,
                                         out SHA1 chunkSHA1)
        {
            if (bundleInfo.Chunks != null)
            {
                var chunkInfo = bundleInfo.Chunks.FirstOrDefault(ci => ci.Id == chunkId);
                if (chunkInfo != null)
                {
                    chunkSHA1 = chunkInfo.SHA1;
                    return true;
                }
            }

            var commonChunkInfo = commonBundles.SelectMany(cb => cb.Chunks)
                                               .FirstOrDefault(ci => ci.Id == chunkId);
            if (commonChunkInfo != null)
            {
                chunkSHA1 = commonChunkInfo.SHA1;
                return true;
            }

            chunkSHA1 = default(SHA1);
            return false;
        }

        private static List<LayoutFile.InstallChunkInfo> GetSuperbundleInstallChunks(LayoutFile layout, string name)
        {
            var rootChunk = layout.InstallManifest.InstallChunks.FirstOrDefault(
                ici => ici.Superbundles.Select(n => n.ToLowerInvariant()).Contains(name) == true);
            if (rootChunk == default(LayoutFile.InstallChunkInfo))
            {
                return null;
            }

            var chunks = new Dictionary<Guid, LayoutFile.InstallChunkInfo>();
            chunks.Add(rootChunk.Id, rootChunk);

            var queue = new Queue<Guid>();

            foreach (var id in rootChunk.RequiredChunks)
            {
                queue.Enqueue(id);
            }

            while (queue.Count > 0)
            {
                var id = queue.Dequeue();

                if (chunks.ContainsKey(id) == true)
                {
                    continue;
                }

                var chunk = layout.InstallManifest.InstallChunks.SingleOrDefault(ici => ici.Id == id);
                if (chunk == default(LayoutFile.InstallChunkInfo))
                {
                    throw new InvalidOperationException();
                }

                chunks.Add(id, chunk);

                foreach (var childId in chunk.RequiredChunks)
                {
                    queue.Enqueue(childId);
                }
            }

            return chunks.Values.ToList();
        }
    }
}
