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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.IO;
using Newtonsoft.Json;
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    public class SaveMetaHeader
    {
        private static readonly Dictionary<uint, string> _KeyLookup;

        static SaveMetaHeader()
        {
            _KeyLookup = GetKeyLookup();
        }

        public const ulong Signature = 0x5245444145484246; // 'FBHEADER'

        #region Fields
        private readonly List<KeyValuePair<uint, string>> _Values;
        #endregion

        public SaveMetaHeader()
        {
            this._Values = new List<KeyValuePair<uint, string>>();
        }

        #region Properties
        [JsonProperty("values")]
        public List<KeyValuePair<uint, string>> Values
        {
            get { return this._Values; }
        }
        #endregion

        public void Serialize(Stream output, Endian endian)
        {
            output.WriteValueU64(Signature, endian);
            output.WriteValueU16(1, endian); // version
            output.WriteValueS32(this._Values.Count, endian);
            foreach (var kv in this._Values)
            {
                output.WriteValueU32(kv.Key, endian);
                var valueBytes = Encoding.UTF8.GetBytes(kv.Value);
                output.WriteValueU16((ushort)valueBytes.Length, endian);
                output.WriteBytes(valueBytes);
            }
        }

        public void Deserialize(Stream input, Endian endian)
        {
            var magic = input.ReadValueU64(endian);
            if (magic != Signature)
            {
                throw new SaveFormatException("invalid save metadata signature");
            }

            var version = input.ReadValueU16(endian);
            if (version != 1)
            {
                throw new SaveFormatException("unsupported save metadata version");
            }

            var count = input.ReadValueU32(endian);
            var values = new List<KeyValuePair<uint, string>>(); 
            for (uint i = 0; i < count; i++)
            {
                var nameHash = input.ReadValueU32(endian);
                var valueLength = input.ReadValueU16(endian);
                var value = input.ReadString(valueLength, true, Encoding.UTF8);
                values.Add(new KeyValuePair<uint, string>(nameHash, value));
            }

            this._Values.Clear();
            this._Values.AddRange(values);
        }

        public static bool GetKey(uint nameHash, out string name)
        {
            return _KeyLookup.TryGetValue(nameHash, out name);
        }

        private static Dictionary<uint, string> GetKeyLookup()
        {
            return new List<string>()
            {
                "AreaNameStringId",
                "AreaThumbnailTextureId",
                "GameVersion",
                "RequiredDLC",
                "RequiredInstallGroup",
                "ProfileName",
                "ProfileUniqueName",
                "ProfileId",
                "LevelID",
                "PlayerLevel",
                "GameCompleted",
                "TrialMode",
                "CompletionPercentage",
                "DateTime",
                "LevelTitleID",
                "LevelFloorID",
                "LevelRegionID",
                "TotalPlaytime",
                "NameOverrideStringId",
            }.ToDictionary(DJB.Compute, n => n);
        }
    }
}
