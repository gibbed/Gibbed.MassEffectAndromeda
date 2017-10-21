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
    public abstract class Agent
    {
        #region Fields
        private readonly ushort _Version;
        private ushort _ReadVersion;
        #endregion

        protected Agent(ushort version)
        {
            this._Version = version;
        }

        #region Properties
        internal abstract string AgentName { get; }

        protected ushort ReadVersion
        {
            get { return this._ReadVersion; }
        }
        #endregion

        internal virtual void Read0(IBitReader reader, byte index)
        {
            this._ReadVersion = reader.ReadUInt16();
            if (this._ReadVersion > this._Version)
            {
                throw new SaveFormatException("unsupported version");
            }
        }

        internal virtual void Write0(IBitWriter writer, byte index)
        {
            writer.WriteUInt16(this._Version);
        }

        internal virtual void Read1(IBitReader reader, ushort arg1)
        {
        }

        internal virtual void Write1(IBitWriter writer, ushort arg1)
        {
        }

        internal virtual void Read2(IBitReader reader)
        {
        }

        internal virtual void Write2(IBitWriter writer)
        {
        }

        internal virtual void Read3(IBitReader reader, ushort arg1)
        {
        }

        internal virtual void Write3(IBitWriter writer, ushort arg1)
        {
        }

        internal virtual void Read4(IBitReader reader)
        {
        }

        internal virtual void Write4(IBitWriter writer)
        {
        }

        internal virtual void Read5(IBitReader reader)
        {
        }

        internal virtual void Write5(IBitWriter writer)
        {
        }
    }
}
