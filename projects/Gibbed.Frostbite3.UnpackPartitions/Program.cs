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
using Gibbed.Frostbite3.Unbundling;
using Gibbed.Frostbite3.VfsFormats;
using Gibbed.IO;
using NDesk.Options;

namespace Gibbed.Frostbite3.UnpackPartitions
{
    public class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool verbose = false;
            bool showHelp = false;

            var options = new OptionSet()
            {
                { "v|verbose", "be verbose", v => verbose = v != null },
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

            var superbundleName = Path.ChangeExtension(paths.Superbundle.Substring(paths.Data.Length + 1), null);
            superbundleName = Helpers.FilterName(superbundleName).ToLowerInvariant();

            var layout = LayoutFile.Read(Path.Combine(paths.Data, "layout.toc"));

            var chunkLookup = new ChunkLookup(layout, paths.Data);
            var chunkLoader = new ChunkLoader(chunkLookup);

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
                if (bundleInfo.Ebx == null)
                {
                    continue;
                }

                foreach (var ebxInfo in bundleInfo.Ebx)
                {
                    using (var data = new MemoryStream())
                    {
                        try
                        {
                            chunkLoader.Load(ebxInfo, data);
                            data.Position = 0;
                        }
                        catch (ChunkCryptoKeyMissingException e)
                        {
                            Console.WriteLine("Cannot decrypt '{0}' without crypto key '{1}'.", ebxInfo.Name, e.KeyId);
                            continue;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception while loading '{0}':", ebxInfo.Name);
                            Console.WriteLine(e);
                            continue;
                        }

                        var outputName = Helpers.FilterPath(ebxInfo.Name);
                        var outputPath = Path.Combine(paths.Output, outputName + ".dummy");
                        var outputParentPath = Path.GetDirectoryName(outputPath);
                        if (string.IsNullOrEmpty(outputParentPath) == false)
                        {
                            Directory.CreateDirectory(outputParentPath);
                        }

                        if (verbose == true)
                        {
                            Console.WriteLine("{0}", resourceInfo.Name);
                        }

                        bool wasConverted = false;
                        outputPath = Path.Combine(paths.Output, outputName + ".entity");
                        wasConverted = ConvertEntity(data, outputPath, chunkLookup, chunkLoader);

                        if (wasConverted == false)
                        {
                            outputPath = Path.Combine(paths.Output, outputName + ".ebx");
                            using (var output = File.Create(outputPath))
                            {
                                output.WriteFromStream(data, data.Length);
                            }
                        }
                    }
                }
            }
        }

        private static bool ConvertEntity(MemoryStream data,
                                          string outputPath,
                                          ChunkLookup chunkLookup,
                                          ChunkLoader chunkLoader)
        {
            // TODO(gibbed): implement me.
            return false;
        }
    }
}
