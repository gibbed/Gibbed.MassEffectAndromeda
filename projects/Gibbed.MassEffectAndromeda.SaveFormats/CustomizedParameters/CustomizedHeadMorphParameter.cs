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
using Gibbed.MassEffectAndromeda.FileFormats;
using Newtonsoft.Json;

namespace Gibbed.MassEffectAndromeda.SaveFormats.CustomizedParameters
{
    [JsonObject(MemberSerialization.OptIn)]
    [CustomizedParameter(_ComponentName)]
    public class CustomizedHeadMorphParameter : CustomizedParameter
    {
        private const string _ComponentName = "CustomizedHeadMorphDesc";

        public override string ComponentName
        {
            get { return _ComponentName; }
        }

        #region Fields
        private Guid _Unknown1;
        private Guid _Unknown2;
        private readonly List<SliderData> _SliderValues;
        private uint _HeadTextureId;
        private byte _Unknown4;
        private readonly List<uint> _Unknown5;
        private readonly List<uint> _Unknown6;
        private readonly List<Vector3> _Unknown7;
        private readonly List<byte[]> _Bones;
        #endregion

        [JsonObject(MemberSerialization.OptIn)]
        public struct SliderData
        {
            [JsonProperty("guid")]
            public Guid Guid { get; set; }

            [JsonProperty("value")]
            public float Value { get; set; }
        }

        public CustomizedHeadMorphParameter()
        {
            this._SliderValues = new List<SliderData>();
            this._Unknown5 = new List<uint>();
            this._Unknown6 = new List<uint>();
            this._Unknown7 = new List<Vector3>();
            this._Bones = new List<byte[]>();
        }

        #region Properties
        [JsonProperty("unknown1")]
        public Guid Unknown1
        {
            get { return this._Unknown1; }
            set { this._Unknown1 = value; }
        }

        [JsonProperty("unknown2")]
        public Guid Unknown2
        {
            get { return this._Unknown2; }
            set { this._Unknown2 = value; }
        }

        [JsonProperty("slider_values")]
        public List<SliderData> SliderValues
        {
            get { return this._SliderValues; }
        }

        [JsonProperty("head_texture_id")]
        public uint HeadTextureId
        {
            get { return this._HeadTextureId; }
            set { this._HeadTextureId = value; }
        }

        [JsonProperty("unknown4")]
        public byte Unknown4
        {
            get { return this._Unknown4; }
            set { this._Unknown4 = value; }
        }

        [JsonProperty("unknown5")]
        public List<uint> Unknown5
        {
            get { return this._Unknown5; }
        }

        [JsonProperty("unknown6")]
        public List<uint> Unknown6
        {
            get { return this._Unknown6; }
        }

        [JsonProperty("unknown7")]
        public List<Vector3> Unknown7
        {
            get { return this._Unknown7; }
        }

        [JsonProperty("bones")]
        public List<byte[]> Bones
        {
            get { return this._Bones; }
        }
        #endregion

        public override void Read(IBitReader reader, ushort version)
        {
            base.Read(reader, version);

            this._Unknown1 = reader.ReadGuid();
            this._Unknown2 = reader.ReadGuid();

            var sliderCount = reader.ReadUInt32();
            var sliderGuids = new Guid[sliderCount];
            for (uint i = 0; i < sliderCount; i++)
            {
                sliderGuids[i] = reader.ReadGuid();
            }
            var sliderValues = new float[sliderCount];
            for (uint i = 0; i < sliderCount; i++)
            {
                sliderValues[i] = reader.ReadFloat32();
            }
            this._SliderValues.Clear();
            for (uint i = 0; i < sliderCount; i++)
            {
                var instance = new SliderData();
                instance.Guid = sliderGuids[i];
                instance.Value = sliderValues[i];
                this._SliderValues.Add(instance);
            }

            this._Unknown4 = reader.ReadUInt8();
            this._HeadTextureId = version < 4 ? 0 : reader.ReadUInt32();

            var unknown5Count = reader.ReadUInt32();
            this._Unknown5.Clear();
            for (uint i = 0; i < unknown5Count; i++)
            {
                this._Unknown5.Add(reader.ReadUInt32());
            }

            var unknown6Count = reader.ReadUInt32();
            this._Unknown6.Clear();
            for (uint i = 0; i < unknown6Count; i++)
            {
                this._Unknown6.Add(reader.ReadUInt32());
            }

            var unknown7Count = reader.ReadUInt32();
            this._Unknown7.Clear();
            for (uint i = 0; i < unknown7Count; i++)
            {
                this._Unknown7.Add(reader.ReadVector3());
            }

            var boneCount = reader.ReadUInt32();
            this._Bones.Clear();
            for (uint i = 0; i < boneCount; i++)
            {
                this._Bones.Add(reader.ReadBytes(12));
            }
        }

        public override void Write(IBitWriter writer, ushort version)
        {
            base.Write(writer, version);

            writer.WriteGuid(this._Unknown1);
            writer.WriteGuid(this._Unknown2);

            writer.WriteUInt32((uint)this._SliderValues.Count);
            foreach (var instance in this._SliderValues)
            {
                writer.WriteGuid(instance.Guid);
            }
            foreach (var instance in this._SliderValues)
            {
                writer.WriteFloat32(instance.Value);
            }
            
            writer.WriteUInt8(this._Unknown4);
            writer.WriteUInt32(this._HeadTextureId);

            writer.WriteUInt32((uint)this._Unknown5.Count);
            foreach (var value in this._Unknown5)
            {
                writer.WriteUInt32(value);
            }

            writer.WriteUInt32((uint)this._Unknown6.Count);
            foreach (var value in this._Unknown6)
            {
                writer.WriteUInt32(value);
            }

            writer.WriteUInt32((uint)this._Unknown7.Count);
            foreach (var value in this._Unknown7)
            {
                writer.WriteVector3(value);
            }

            writer.WriteUInt32((uint)this._Bones.Count);
            foreach (var instance in this._Bones)
            {
                writer.WriteBytes(instance, 0, 12);
            }
        }
    }
}
