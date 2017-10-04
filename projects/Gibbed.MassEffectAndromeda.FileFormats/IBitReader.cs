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
    public interface IBitReader
    {
        int Length { get; }
        int Position { get; set; }
        int BytePosition { get; }
        int BitOffset { get; }
        int FrameCount { get; }
        
        string DumpRead();
        byte[] DumpReadBitmap();
        bool HasUnreadBits();

        void AlignToNextByte();

        bool ReadBoolean();
        sbyte ReadInt8();
        sbyte ReadInt8(int bitCount);
        byte ReadUInt8();
        byte ReadUInt8(int bitCount);
        short ReadInt16();
        short ReadInt16(int bitCount);
        ushort ReadUInt16();
        ushort ReadUInt16(int bitCount);
        int ReadInt32();
        int ReadInt32(int bitCount);
        uint ReadUInt32();
        uint ReadUInt32(int bitCount);
        ulong ReadUInt64();
        ulong ReadUInt64(int bitCount);
        float ReadFloat32();
        Guid ReadGuid();
        byte[] ReadBits(int length);
        void ReadBytes(byte[] buffer);
        void ReadBytes(byte[] buffer, int offset, int count);
        byte[] ReadBytes(int length);
        string ReadString();
        object ReadFunkyValue();
        Vector3 ReadVector3();
        Vector4 ReadVector4();
        Transform ReadTransform();

        void SkipBits(int bitCount);
        void SkipBoolean();
        void SkipUInt32(int bitCount);

        uint PushFrameLength(int bitCount);
        uint PushFramePosition(int bitCount);
        void PopFrameLength();
        void PopFramePosition();
    }
}
