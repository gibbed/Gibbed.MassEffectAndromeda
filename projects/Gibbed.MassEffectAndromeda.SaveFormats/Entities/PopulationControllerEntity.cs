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
using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats.Entities
{
    [Entity(0x0DA8094B)]
    [Entity(0x1BBC522C)]
    [Entity(0x3858176A)]
    [Entity(0x4228A837)]
    [Entity(0x701D3D37)]
    [Entity(0xC09BBFE9)]
    public class PopulationControllerEntity : Entity
    {
        internal override void Read0(IBitReader reader)
        {
            base.Read0(reader);
            throw new NotImplementedException();
        }

        internal override void Read1(IBitReader reader)
        {
            base.Read1(reader);

            var count = reader.ReadUInt16();
            for (uint i = 0; i < count; i++)
            {
                var unknown0 = reader.ReadUInt32();
            }
        }
    }
}
