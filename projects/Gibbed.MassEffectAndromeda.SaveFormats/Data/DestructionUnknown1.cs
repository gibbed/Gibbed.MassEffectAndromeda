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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Data
{
    public class DestructionUnknown1
    {
        #region Fields
        private readonly List<DestructionUnknown2> _Unknown1;
        #endregion

        public DestructionUnknown1()
        {
            this._Unknown1 = new List<DestructionUnknown2>();
        }

        #region Properties
        public List<DestructionUnknown2> Unknown1
        {
            get { return this._Unknown1; }
        }
        #endregion

        internal void Read(IBitReader reader)
        {
            this._Unknown1.Clear();
            var unknown1Count = reader.ReadUInt16();
            for (int i = 0; i < unknown1Count; i++)
            {
                var unknown1 = new DestructionUnknown2();
                unknown1.Read(reader);
                this._Unknown1.Add(unknown1);
            }
        }

        internal void Write(IBitWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
