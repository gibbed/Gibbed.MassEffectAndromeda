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

namespace Gibbed.MassEffectAndromeda.FileFormats
{
    public interface IBitWriter
    {
        int Position { get; set; }
        int BytePosition { get; }
        int BitOffset { get; }
        int FrameCount { get; }

        void AlignToNextByte();

        void WriteBoolean(bool value);
        void WriteUInt8(byte value);
        void WriteUInt8(byte value, int bitCount);
        void WriteUInt16(ushort value);
        void WriteUInt16(ushort value, int bitCount);
        void WriteInt32(int value);
        void WriteInt32(int value, int bitCount);
        void WriteUInt32(uint value);
        void WriteUInt32(uint value, int bitCount);
        void WriteUInt64(ulong value);
        void WriteUInt64(ulong value, int bitCount);
        void WriteFloat32(float value);
        void WriteGuid(Guid value);
        void WriteBits(byte[] value, int length);
        void WriteBytes(byte[] buffer);
        void WriteBytes(byte[] buffer, int offset, int count);
        void WriteString(string value);
        void WriteFunkyValue(object value);
        void WriteVector3(Vector3 value);
        void WriteVector4(Vector4 value);
        void WriteTransform(Transform value);

        void SkipBits(int bitCount);
        void SkipBoolean();
        void SkipUInt32(int bitCount);

        void PushFrameLength(int bitCount);
        void PushFramePosition(int bitCount);
        uint PopFrameLength();
        uint PopFramePosition();
    }
}
