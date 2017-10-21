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
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;
using Hashing = Gibbed.Frostbite3.Common.Hashing;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    public class SaveFile
    {
        public const ulong Signature = 0x534B4E5548434246; // 'FBCHUNKS'

        #region Fields
        private Endian _Endian;
        private readonly SaveMetaHeader _MetaHeader;
        private readonly SaveData _Data;
        #endregion

        public SaveFile()
        {
            this._Endian = Endian.Little;
            this._MetaHeader = new SaveMetaHeader();
            this._Data = new SaveData();
        }

        #region Properties
        [JsonProperty("endian")]
        public Endian Endian
        {
            get { return this._Endian; }
            set { this._Endian = value; }
        }

        [JsonProperty("meta_header")]
        public SaveMetaHeader MetaHeader
        {
            get { return this._MetaHeader; }
        }

        [JsonProperty("data")]
        public SaveData Data
        {
            get { return this._Data; }
        }
        #endregion

        public void Serialize(Stream output)
        {
            var endian = this._Endian;

            byte[] metaHeaderBytes;
            using (var data = new MemoryStream())
            {
                this._MetaHeader.Serialize(data, endian);
                data.Flush();
                metaHeaderBytes = data.ToArray();
            }

            byte[] dataBytes;
            {
                var writer = new BitWriter(0x10000);
                this._Data.Write(writer);
                dataBytes = writer.GetBytes();
            }

            output.WriteValueU64(Signature, endian);
            output.WriteValueU16(1, endian); // version

            output.WriteValueS32(4 + metaHeaderBytes.Length, endian);
            output.WriteValueS32(4 + dataBytes.Length, endian);

            var metaHeaderCRC = Hashing.CRC32.Compute(metaHeaderBytes, 0, metaHeaderBytes.Length, 0x12345678);
            output.WriteValueU32(metaHeaderCRC, endian);
            output.WriteBytes(metaHeaderBytes);

            var dataCRC = Hashing.CRC32.Compute(dataBytes, 0, dataBytes.Length, 0x12345678);
            output.WriteValueU32(dataCRC, endian);
            output.WriteBytes(dataBytes);
        }

        public void Deserialize(Stream input)
        {
            var basePosition = input.Position;

            if (basePosition + 18 > input.Length)
            {
                throw new EndOfStreamException("not enough data for save header");
            }

            var magic = input.ReadValueU64(Endian.Little);
            if (magic != Signature && magic.Swap() != Signature)
            {
                throw new SaveFormatException("invalid save file signature");
            }
            var endian = magic == Signature ? Endian.Little : Endian.Big;

            var version = input.ReadValueU16(endian);
            if (version != 1)
            {
                throw new SaveFormatException("unsupported save file version");
            }

            var metaHeaderLength = input.ReadValueU32(endian);
            var dataLength = input.ReadValueU32(endian);

            if (basePosition + 18 + metaHeaderLength + dataLength > input.Length)
            {
                throw new EndOfStreamException("incomplete save file");
            }

            if (metaHeaderLength > int.MaxValue)
            {
                throw new SaveFormatException("metadata length is too large");
            }

            if (dataLength > int.MaxValue)
            {
                throw new SaveFormatException("data length is too large");
            }

            var metaHeaderBytes = input.ReadBytes((int)metaHeaderLength);
            using (var data = new MemoryStream(metaHeaderBytes, false))
            {
                var providedCRC = data.ReadValueU32(endian);
                var computedCRC = Hashing.CRC32.Compute(metaHeaderBytes, 4, metaHeaderBytes.Length - 4, 0x12345678);
                if (providedCRC != computedCRC)
                {
                    throw new SaveCorruptionException("save metadata CRC invalid -- corrupt?");
                }
                this._MetaHeader.Deserialize(data, endian);
            }

            var dataBytes = input.ReadBytes((int)dataLength);
            using (var data = new MemoryStream(dataBytes, false))
            {
                var providedCRC = data.ReadValueU32(endian);
                var computedCRC = Hashing.CRC32.Compute(dataBytes, 4, dataBytes.Length - 4, 0x12345678);
                if (providedCRC != computedCRC)
                {
                    throw new SaveCorruptionException("save data CRC invalid -- corrupt?");
                }
                var reader = new BitReader(dataBytes, 4, dataBytes.Length - 4);
                this._Data.Read(reader);
            }

            this._Endian = endian;
        }

        public static SaveFile Read(Stream input)
        {
            var instance = new SaveFile();
            instance.Deserialize(input);
            return instance;
        }
    }
}
