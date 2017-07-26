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
using System.Text;
using Gibbed.Frostbite3.Common;
using Gibbed.Frostbite3.VfsFormats;
using Gibbed.IO;
using NDesk.Options;
using Newtonsoft.Json;
using ValueTag = Gibbed.Frostbite3.Common.DbObject.ValueTag;
using ValueType = Gibbed.Frostbite3.Common.DbObject.ValueType;

namespace Gibbed.Frostbite3.ConvertDbObject
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
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_dbobject [output_json]", GetExecutableName());
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var inputPath = extras[0];
            var outputPath = extras.Count > 1 ? extras[1] : inputPath + ".json";
            using (var textWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.IndentChar = ' ';
                jsonWriter.Indentation = 2;

                using (var input = File.OpenRead(inputPath))
                {
                    // ReSharper disable AccessToDisposedClosure
                    FileLayerHelper.Read(input, s => Convert(s, jsonWriter));
                    // ReSharper restore AccessToDisposedClosure
                }

                jsonWriter.Flush();
                textWriter.Flush();
                File.WriteAllText(outputPath, textWriter.ToString(), Encoding.UTF8);
            }
        }

        private static void Convert(Stream input, JsonWriter writer)
        {
            var tag = new ValueTag(input.ReadValueU8());
            ConvertValue(input, tag, writer);
        }

        private static void ConvertValue(Stream input, ValueTag tag, JsonWriter writer)
        {
            switch (tag.Type)
            {
                case ValueType.Array:
                {
                    ConvertArray(input, tag, writer);
                    break;
                }

                case ValueType.Object:
                {
                    ConvertDictionary(input, tag, writer);
                    break;
                }

                case ValueType.Bool:
                {
                    writer.WriteValue(input.ReadValueU8() != 0);
                    break;
                }

                case ValueType.String:
                {
                    writer.WriteValue(ReadString(input, tag));
                    break;
                }

                case ValueType.Int32:
                {
                    writer.WriteValue(input.ReadValueS32(Endian.Little));
                    break;
                }

                case ValueType.Int64:
                {
                    writer.WriteValue(input.ReadValueS64(Endian.Little));
                    break;
                }

                case ValueType.Float64:
                {
                    writer.WriteValue(input.ReadValueF64(Endian.Little));
                    break;
                }

                case ValueType.Guid:
                {
                    writer.WriteValue(input.ReadValueGuid(Endian.Big));
                    break;
                }

                case ValueType.SHA1:
                {
                    var bytes = input.ReadBytes(20);
                    writer.WriteStartObject();
                    var oldFormatting = writer.Formatting;
                    writer.Formatting = Formatting.None;
                    writer.WritePropertyName("type");
                    writer.WriteValue("sha1");
                    writer.WritePropertyName("value");
                    writer.WriteValue(BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant());
                    writer.WriteEndObject();
                    writer.Formatting = oldFormatting;
                    break;
                }

                case ValueType.Bytes:
                {
                    var length = input.ReadPackedValueUInt32();
                    var basePosition = input.Position;
                    var endPosition = basePosition + length;
                    if (endPosition > input.Length)
                    {
                        throw new EndOfStreamException();
                    }

                    if (length > int.MaxValue)
                    {
                        throw new FormatException();
                    }

                    var bytes = input.ReadBytes((int)length);

                    if (input.Position != endPosition)
                    {
                        throw new FormatException();
                    }

                    writer.WriteStartObject();
                    var oldFormatting = writer.Formatting;
                    writer.Formatting = Formatting.None;
                    writer.WritePropertyName("type");
                    writer.WriteValue("bytes");
                    writer.WritePropertyName("value");
                    writer.WriteValue(BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant());
                    writer.WriteEndObject();
                    writer.Formatting = oldFormatting;
                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static void ConvertArray(Stream input, ValueTag tag, JsonWriter writer)
        {
            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            writer.WriteStartArray();
            var elementTag = new ValueTag(input.ReadValueU8());
            if (elementTag.Type != ValueType.Invalid)
            {
                do
                {
                    ConvertValue(input, elementTag, writer);
                    elementTag = new ValueTag(input.ReadValueU8());
                }
                while (elementTag.Type != ValueType.Invalid);
            }
            writer.WriteEndArray();

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }
        }

        private static void ConvertDictionary(Stream input, ValueTag tag, JsonWriter writer)
        {
            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            writer.WriteStartObject();
            var propertyTag = new ValueTag(input.ReadValueU8());
            while (propertyTag.Type != ValueType.Invalid)
            {
                var propertyName = input.ReadStringZ(Encoding.UTF8);
                writer.WritePropertyName(propertyName);
                ConvertValue(input, propertyTag, writer);
                propertyTag = new ValueTag(input.ReadValueU8());
            }
            writer.WriteEndObject();

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }
        }

        private static string ReadString(Stream input, ValueTag tag)
        {
            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            var value = input.ReadStringZ(Encoding.UTF8);

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }

            return value;
        }
    }
}
