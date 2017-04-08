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
using Gibbed.IO;

namespace Gibbed.Frostbite3.Unbundling
{
    public class Discovery
    {
        public static string FindBasePath(string inputPath, out string superbundleName)
        {
            inputPath = Path.GetFullPath(inputPath);

            var layoutPath = FindLayoutPath(inputPath);
            if (string.IsNullOrEmpty(layoutPath) == true)
            {
                superbundleName = null;
                return null;
            }

            var dataPath = Path.GetDirectoryName(layoutPath);
            if (string.IsNullOrEmpty(dataPath) == true || inputPath.StartsWith(dataPath) == false)
            {
                superbundleName = null;
                return null;
            }

            var basePath = Path.GetDirectoryName(dataPath);
            if (string.IsNullOrEmpty(basePath) == true)
            {
                superbundleName = null;
                return null;
            }

            var superbundlePath = PathHelper.GetRelativePath(dataPath, inputPath);
            if (string.IsNullOrEmpty(superbundlePath) == true)
            {
                superbundleName = null;
                return null;
            }

            superbundleName = Helpers.FilterName(Path.ChangeExtension(superbundlePath, null));
            return basePath;
        }

        private static string FindLayoutPath(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            while (string.IsNullOrEmpty(basePath) == false)
            {
                var layoutPath = Path.Combine(basePath, "layout.toc");
                if (File.Exists(layoutPath) == true)
                {
                    return layoutPath;
                }
                basePath = Path.GetDirectoryName(basePath);
            }
            return null;
        }
    }
}
