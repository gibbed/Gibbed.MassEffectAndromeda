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
using Gibbed.Frostbite3.Unbundling;
using Gibbed.IO;
using NDesk.Options;

namespace Gibbed.Frostbite3.UnpackResources
{
    public class Program
    {
        #region Logger
        // ReSharper disable InconsistentNaming
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // ReSharper restore InconsistentNaming
        #endregion

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        private static void IncreaseLogLevel(string arg, ref int level)
        {
            if (arg == null || level <= 0)
            {
                return;
            }

            level--;
        }

        public static void Main(string[] args)
        {
            bool convertTextures = false;
            bool noPatch = false;
            bool noCatch = false;
            int logLevelOrdinal = 3;
            bool showHelp = false;

            var options = new OptionSet()
            {
                { "t|convert-textures", "convert textures", v => convertTextures = v != null },
                { "no-patch", "don't use patch data", v => noPatch = v != null },
                { "no-catch", "don't catch exceptions when loading data", v => noCatch = v != null },
                { "v|verbose", "increase log level (-v/-vv/-vvv)", v => IncreaseLogLevel(v, ref logLevelOrdinal) },
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

            LogHelper.SetConfiguration(NLog.LogLevel.FromOrdinal(logLevelOrdinal));

            var inputPath = extras[0];
            var outputBasePath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, null) + "_res_unpack";

            string superbundleName;
            var dataBasePath = Discovery.FindBasePath(inputPath, out superbundleName);
            if (string.IsNullOrEmpty(dataBasePath) == true)
            {
                Logger.Error("Failed to discover base game path.");
                return;
            }

            var dataManager = DataManager.Initialize(dataBasePath, noPatch);
            if (dataManager == null)
            {
                Logger.Fatal("Could not initialize superbundle manager.");
                return;
            }

            var extensionsById = ResourceTypes.GetExtensions();

            var superbundle = dataManager.MountSuperbundle(superbundleName);

            foreach (var resourceInfo in superbundle.Bundles
                                                    .Where(bi => bi.Resources != null)
                                                    .SelectMany(bi => bi.Resources)
                                                    .OrderBy(bi => bi.Name))
            {
                using (var data = new MemoryStream())
                {
                    Logger.Info(resourceInfo.Name);

                    if (noCatch == true)
                    {
                        dataManager.LoadData(resourceInfo, data);
                        data.Position = 0;
                    }
                    else
                    {
                        try
                        {
                            dataManager.LoadData(resourceInfo, data);
                            data.Position = 0;
                        }
                        catch (ChunkCryptoKeyMissingException e)
                        {
                            Logger.Warn("Cannot decrypt '{0}' without crypto key '{1}'.",
                                        resourceInfo.Name,
                                        e.KeyId);
                            continue;
                        }
                        catch (Exception e)
                        {
                            Logger.Warn(e, "Exception while loading '{0}':", resourceInfo.Name);
                            continue;
                        }
                    }

                    var outputName = Helpers.FilterPath(resourceInfo.Name);
                    var outputPath = Path.Combine(outputBasePath, outputName + ".dummy");
                    var outputParentPath = Path.GetDirectoryName(outputPath);
                    if (string.IsNullOrEmpty(outputParentPath) == false)
                    {
                        Directory.CreateDirectory(outputParentPath);
                    }

                    bool wasConverted = false;
                    if (convertTextures == true && resourceInfo.ResourceType == ResourceTypes.Texture)
                    {
                        outputPath = Path.Combine(outputBasePath, outputName + ".dds");
                        wasConverted = ConvertTexture(data, outputPath, dataManager);
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
                            data.Position = 0;
                            output.WriteFromStream(data, data.Length);
                        }
                    }
                }
            }

            dataManager.Dispose();
        }

        private static bool ConvertTexture(MemoryStream data,
                                           string outputPath,
                                           DataManager dataManager)
        {
            var textureHeader = TextureHeader.Read(data);

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

            SHA1Hash chunkId;
            long size;
            if (dataManager.GetChunkId(textureHeader.DataChunkId, out chunkId, out size) == false)
            {
                throw new InvalidOperationException();
            }

            byte[] dataBytes;
            using (var temp = new MemoryStream())
            {
                if (dataManager.LoadChunk(chunkId, size, textureHeader.TotalSize, temp) == false)
                {
                    throw new InvalidOperationException();
                }

                temp.Position = 0;
                dataBytes = temp.GetBuffer();
            }

            using (var output = File.Create(outputPath))
            {
                DDSUtils.WriteFile(textureHeader, dataBytes, output);
            }

            return true;
        }
    }
}
