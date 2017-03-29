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
using Gibbed.Frostbite3.VfsFormats;
using NDesk.Options;

namespace Gibbed.Frostbite3.UnpackInitFS
{
    public class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool showHelp = false;

            var options = new OptionSet()
            {
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
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_initfs [output_dir]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var inputPath = extras[0];
            var outputBasePath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, null) + "_unpack";

            inputPath = Path.GetFullPath(inputPath);
            outputBasePath = Path.GetFullPath(outputBasePath);

            var initFileSystem = InitFileSystemFile.Read(inputPath);
            foreach (var entry in initFileSystem.Select(e => e.File))
            {
                var fsName = FilterName(entry.FileSystemName ?? "__INITFS");
                var outputName = FilterName(entry.Name);

                var outputPath = Path.Combine(outputBasePath, fsName, outputName);
                var outputParentPath = Path.GetDirectoryName(outputPath);
                if (string.IsNullOrEmpty(outputParentPath) == false)
                {
                    Directory.CreateDirectory(outputParentPath);
                }
                File.WriteAllBytes(outputPath, entry.Payload);
            }
        }

        private static string FilterName(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            path = path.Replace(Path.AltDirectorySeparatorChar, '/');
            return path;
        }
    }
}
