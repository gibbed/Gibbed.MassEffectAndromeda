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
using Gibbed.Frostbite3.Common;
using Gibbed.Frostbite3.ResourceFormats;
using Gibbed.Frostbite3.Unpacking;
using Gibbed.Frostbite3.VfsFormats;
using NDesk.Options;
using AlphaFS = Alphaleonis.Win32.Filesystem;
using Superbundle = Gibbed.Frostbite3.VfsFormats.Superbundle;

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

            Paths paths;
            if (Paths.Discover(extras[0], extras.Count > 1 ? extras[1] : null, out paths) == false)
            {
                Console.WriteLine("Failed to discover data paths.");
                return;
            }

            var extensionsById = ResourceTypes.GetExtensions();

            var superbundleName = Path.ChangeExtension(paths.Superbundle.Substring(paths.Data.Length + 1), null);
            superbundleName = Helpers.FilterName(superbundleName).ToLowerInvariant();

            var layout = LayoutFile.Read(Path.Combine(paths.Data, "layout.toc"));

            var chunkLookup = new ChunkLookup(layout, paths.Data);
            var superbundle = chunkLookup.AddBundle(superbundleName);

            // add common chunk bundles (chunks*.toc/sb)
            foreach (var superbundleInfo in layout.Superbundles.Where(
                sbi => ChunkLookup.IsChunkBundle(sbi.Name) == true))
            {
                if (chunkLookup.AddBundle(superbundleInfo.Name.ToLowerInvariant()) == null)
                {
                    Console.WriteLine("Failed to load catalog for '{0}'.", superbundleInfo.Name);
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
                    var chunkInfo = chunkLookup.GetChunkVariant(resourceInfo);
                    if (chunkInfo == null)
                    {
                        Console.WriteLine("Could not find catalog entry for '{0}'.", resourceInfo.Name);
                        continue;
                    }

                    if (chunkInfo.Size != resourceInfo.Size)
                    {
                        throw new InvalidOperationException();
                    }

                    var outputName = Helpers.FilterPath(resourceInfo.Name);
                    var outputPath = AlphaFS.Path.Combine(paths.Output, outputName + ".dummy");
                    var outputParentPath = AlphaFS.Path.GetDirectoryName(outputPath);
                    if (string.IsNullOrEmpty(outputParentPath) == false)
                    {
                        AlphaFS.Directory.CreateDirectory(outputParentPath);
                    }

                    Console.WriteLine("{0}", resourceInfo.Name);

                    bool wasConverted = false;
                    if (convertTextures == true && resourceInfo.ResourceType == ResourceTypes.Texture)
                    {
                        outputPath = AlphaFS.Path.Combine(paths.Output, outputName + ".dds");
                        wasConverted = ConvertTexture(resourceInfo, chunkInfo, outputPath, chunkLookup);
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

                        outputPath = AlphaFS.Path.Combine(paths.Output, outputName + extension);
                        using (var output = AlphaFS.File.Create(outputPath))
                        {
                            ChunkLoading.Load(resourceInfo, chunkInfo, output);
                        }
                    }
                }
            }
        }

        private static bool ConvertTexture(Superbundle.ResourceInfo resourceInfo,
                                           ChunkLookup.IChunkVariantInfo entry,
                                           string outputPath,
                                           ChunkLookup chunkLookup)
        {
            TextureHeader textureHeader;
            using (var temp = new MemoryStream())
            {
                ChunkLoading.Load(resourceInfo, entry, temp);
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

            if (textureHeader.Unknown10 != 0 ||
                (textureHeader.Flags != TextureFlags.None &&
                 textureHeader.Flags != TextureFlags.Unknown0 &&
                 textureHeader.Flags != (TextureFlags.Unknown0 | TextureFlags.Unknown3) &&
                 textureHeader.Flags != TextureFlags.Unknown5) ||
                textureHeader.Unknown1C != 1)
            {
                throw new FormatException();
            }

            SHA1 chunkSHA1;
            long size;
            if (chunkLookup.GetChunkSHA1(textureHeader.DataChunkId, out chunkSHA1, out size) == false)
            {
                throw new InvalidOperationException();
            }

            var dataChunkInfo = chunkLookup.GetChunkVariant(chunkSHA1, size);
            byte[] dataBytes;
            using (var temp = new MemoryStream())
            {
                ChunkLoading.Load(dataChunkInfo, textureHeader.TotalSize, temp);
                temp.Position = 0;
                dataBytes = temp.GetBuffer();
            }

            using (var output = AlphaFS.File.Create(outputPath))
            {
                DDSUtils.WriteFile(textureHeader, dataBytes, output);
            }

            return true;
        }
    }
}
