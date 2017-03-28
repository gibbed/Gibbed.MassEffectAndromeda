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
using System.IO;

namespace Gibbed.Frostbite3.UnpackResources
{
    internal struct Paths
    {
        public readonly string Data;
        public readonly string Superbundle;
        public readonly string TableOfContents;
        public readonly string Output;

        private Paths(string dataPath, string superbundlePath, string tableOfContentsPath, string outputPath)
        {
            this.Data = dataPath;
            this.Superbundle = superbundlePath;
            this.TableOfContents = tableOfContentsPath;
            this.Output = outputPath;
        }

        public static bool Discover(string inputPath, string outputPath, out Paths result)
        {
            if (string.IsNullOrEmpty(inputPath) == true)
            {
                throw new ArgumentNullException("inputPath");
            }

            inputPath = Path.GetFullPath(inputPath);
            outputPath = outputPath != null ? Path.GetFullPath(outputPath) : null;

            switch (Path.GetExtension(inputPath).ToLowerInvariant())
            {
                case ".sb":
                {
                    var superbundlePath = inputPath;
                    var tableOfContentsPath = Path.ChangeExtension(superbundlePath, ".toc");
                    var dataPath = FindDataPath(superbundlePath);
                    var outputBasePath = outputPath ?? Path.ChangeExtension(superbundlePath, null) + "_res_unpack";
                    result = new Paths(dataPath, superbundlePath, tableOfContentsPath, outputBasePath);
                    return true;
                }

                case ".toc":
                {
                    var tableOfContentsPath = inputPath;
                    var superbundlePath = Path.ChangeExtension(tableOfContentsPath, ".sb");
                    var dataPath = FindDataPath(superbundlePath);
                    var outputBasePath = outputPath ?? Path.ChangeExtension(tableOfContentsPath, null) + "_res_unpack";
                    result = new Paths(dataPath, superbundlePath, tableOfContentsPath, outputBasePath);
                    return true;
                }
            }

            result = default(Paths);
            return false;
        }

        public static string FindDataPath(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            while (string.IsNullOrEmpty(basePath) == false)
            {
                var layoutPath = Path.Combine(basePath, "layout.toc");
                if (File.Exists(layoutPath) == true)
                {
                    return basePath;
                }
                basePath = Path.GetDirectoryName(basePath);
            }
            return null;
        }
    }
}
