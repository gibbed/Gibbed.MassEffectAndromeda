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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct BundleHeapInfo
    {
        [JsonProperty("unknown1")]
        public string Unknown1 { get; set; }
        
        [JsonProperty("unknown2")]
        public uint Unknown2 { get; set; }
        
        [JsonProperty("unknown3")]
        public uint Unknown3 { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BundleHeapType Type { get; set; }

        [JsonProperty("unknown4")]
        public uint Unknown4 { get; set; }

        [JsonProperty("unknown5")]
        public bool Unknown5 { get; set; }

        [JsonProperty("unknown6")]
        public byte Unknown6 { get; set; }

        [JsonProperty("unknown7")]
        public byte Unknown7 { get; set; }

        [JsonProperty("unknown8")]
        public uint Unknown8 { get; set; }
    }
}
