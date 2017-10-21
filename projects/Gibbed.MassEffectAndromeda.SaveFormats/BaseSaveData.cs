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

using Gibbed.MassEffectAndromeda.FileFormats;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    public class BaseSaveData
    {
        public const ulong Signature = 0x0A45564153004246; // 'FB\0SAVE\n'

        public virtual void Read(IBitReader reader)
        {
            var magic = reader.ReadUInt64();
            if (magic != Signature)
            {
                throw new SaveFormatException("invalid save data signature");
            }

            var hasExtraValue = reader.ReadBoolean();
            if (hasExtraValue == true)
            {
                reader.SkipUInt32(27);
            }
        }

        public virtual void Write(IBitWriter writer)
        {
            writer.WriteUInt64(Signature);
            writer.WriteBoolean(false);
        }
    }
}
