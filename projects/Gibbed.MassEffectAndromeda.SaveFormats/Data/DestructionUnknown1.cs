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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DestructionUnknown1
    {
        #region Fields
        private readonly List<DestructionUnknown2> _Unknown;
        #endregion

        public DestructionUnknown1()
        {
            this._Unknown = new List<DestructionUnknown2>();
        }

        #region Properties
        [JsonProperty("unknown")]
        public List<DestructionUnknown2> Unknown
        {
            get { return this._Unknown; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._Unknown.Clear();
            var unknownCount = reader.ReadUInt16();
            for (int i = 0; i < unknownCount; i++)
            {
                var unknown = new DestructionUnknown2();
                unknown.Read(reader);
                this._Unknown.Add(unknown);
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._Unknown.Count);
            foreach (var unknown in this._Unknown)
            {
                unknown.Write(writer);
            }
        }
    }
}
