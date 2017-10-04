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
using System.IO;
using System.Text;

namespace Gibbed.MassEffectAndromeda.FileFormats
{
    public class BitWriter : IBitWriter
    {
        private readonly Stack<BitFrame> _Frames;

        private byte[] _Buffer;
        private int _Position;
        private readonly int _BasePosition;

        public BitWriter(int initialCount, int basePosition)
        {
            this._Frames = new Stack<BitFrame>();
            this._Buffer = new byte[initialCount];
            this._Position = 0;
            this._BasePosition = basePosition;
        }

        public BitWriter(int initialCount)
            : this(initialCount, 0)
        {
        }

        public int Position
        {
            get { return this._Position; }
            set
            {
                if (this._Frames.Count > 0)
                {
                    throw new BitReaderException("cannot set position while in a frame");
                }
                this._Position = value;
            }
        }

        public int BytePosition
        {
            get { return this._Position >> 3; }
        }

        public int BitOffset
        {
            get { return this._Position & 7; }
        }

        public int FrameCount
        {
            get { return this._Frames.Count; }
        }

        private void Resize(int byteCount)
        {
            var currentByteCount = this._Buffer.Length;
            while (currentByteCount < byteCount)
            {
                currentByteCount <<= 1;
            }
            Array.Resize(ref this._Buffer, currentByteCount);
        }

        private void AllocateBits(int length)
        {
            var desiredLength = this._Position + length;
            var byteCount = (desiredLength + 7) / 8;
            if (byteCount <= this._Buffer.Length)
            {
                return;
            }
            this.Resize(byteCount);
        }

        public byte[] GetBytes()
        {
            if (this.FrameCount > 0)
            {
                throw new InvalidOperationException("cannot resolve bytes with active frames");
            }

            var buffer = new byte[(this._Position + 7) >> 3];
            Array.Copy(this._Buffer, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public void AlignToNextByte()
        {
            this._Position = (this._Position + 7) & ~7;
        }

        private void WriteBits8(sbyte value, int bitCount)
        {
            throw new NotImplementedException();
        }

        private void WriteUBits8(byte value, int bitCount)
        {
            if (bitCount < 0 || bitCount > 8)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            this.AllocateBits(bitCount);

            int shift = 0;
            while (bitCount > 0)
            {
                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                var mask = (byte)((1 << left) - 1);
                var part = (byte)(mask & (value >> shift));
                part <<= offset;
                this._Buffer[this._Position >> 3] |= part;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
        }

        private void WriteBits16(short value, int bitCount)
        {
            throw new NotImplementedException();
        }

        private void WriteUBits16(ushort value, int bitCount)
        {
            if (bitCount < 0 || bitCount > 16)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            this.AllocateBits(bitCount);

            int shift = 0;
            while (bitCount > 0)
            {
                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                var mask = (byte)((1 << left) - 1);
                var part = (byte)(mask & (value >> shift));
                part <<= offset;
                this._Buffer[this._Position >> 3] |= part;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
        }

        private void WriteBits32(int value, int bitCount)
        {
            this.WriteUBits32((uint)value, bitCount);
        }

        private void WriteUBits32(uint value, int bitCount)
        {
            if (bitCount < 0 || bitCount > 32)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            this.AllocateBits(bitCount);

            int shift = 0;
            while (bitCount > 0)
            {
                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                var mask = (byte)((1 << left) - 1);
                var part = (byte)(mask & (value >> shift));
                part <<= offset;
                this._Buffer[this._Position >> 3] |= part;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
        }

        private void WriteBits64(long value, int bitCount)
        {
            throw new NotImplementedException();
        }

        private void WriteUBits64(ulong value, int bitCount)
        {
            if (bitCount < 0 || bitCount > 64)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            this.AllocateBits(bitCount);

            int shift = 0;
            while (bitCount > 0)
            {
                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                var mask = (byte)((1 << left) - 1);
                var part = (byte)(mask & (value >> shift));
                part <<= offset;
                this._Buffer[this._Position >> 3] |= part;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
        }

        public void WriteBoolean(bool value)
        {
            this.AllocateBits(1);
            int offset = this._Position % 8;
            if (value == true)
            {
                this._Buffer[this._Position >> 3] |= (byte)(1u << offset);
            }
            else
            {
                this._Buffer[this._Position >> 3] &= (byte)~(byte)(1u << offset);
            }
            this._Position++;
        }

        public void WriteInt8(sbyte value)
        {
            this.WriteBits8(value, 8);
        }

        public void WriteInt8(sbyte value, int bitCount)
        {
            this.WriteBits8(value, bitCount);
        }

        public void WriteUInt8(byte value)
        {
            this.WriteUBits8(value, 8);
        }

        public void WriteUInt8(byte value, int bitCount)
        {
            this.WriteUBits8(value, bitCount);
        }

        public void WriteInt16(short value)
        {
            this.WriteBits16(value, 16);
        }

        public void WriteInt16(short value, int bitCount)
        {
            this.WriteBits16(value, bitCount);
        }

        public void WriteUInt16(ushort value)
        {
            this.WriteUBits16(value, 16);
        }

        public void WriteUInt16(ushort value, int bitCount)
        {
            this.WriteUBits16(value, bitCount);
        }

        public void WriteInt32(int value)
        {
            this.WriteBits32(value, 32);
        }

        public void WriteInt32(int value, int bitCount)
        {
            this.WriteBits32(value, bitCount);
        }

        public void WriteUInt32(uint value)
        {
            this.WriteUBits32(value, 32);
        }

        public void WriteUInt32(uint value, int bitCount)
        {
            this.WriteUBits32(value, bitCount);
        }

        public void WriteInt64(long value)
        {
            this.WriteBits64(value, 64);
        }

        public void WriteInt64(long value, int bitCount)
        {
            this.WriteBits64(value, bitCount);
        }

        public void WriteUInt64(ulong value)
        {
            this.WriteUBits64(value, 64);
        }

        public void WriteUInt64(ulong value, int bitCount)
        {
            this.WriteUBits64(value, bitCount);
        }

        public void WriteFloat32(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.WriteBytes(bytes);
        }

        public void WriteGuid(Guid value)
        {
            var bytes = value.ToByteArray();
            this.WriteBytes(bytes);
        }

        public void WriteBits(byte[] value, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return;
            }

            this.AllocateBits(length);

            var byteCount = length / 8;
            var shift = this._Position % 8;

            if (shift == 0)
            {
                // we can cheat and use a straight array copy
                Array.Copy(value, 0, this._Buffer, this._Position >> 3, byteCount);
                this.SkipBits(8 * byteCount);
                length -= 8 * byteCount;
                if (length > 0)
                {
                    this.WriteUBits8(value[byteCount], length);
                }
            }
            else
            {
                for (int i = 0; i < byteCount; i++, length -= 8)
                {
                    this.WriteUBits8(value[i], 8);
                }
                if (length > 0)
                {
                    this.WriteUBits8(value[byteCount], length);
                }
            }
        }

        public void WriteBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this.WriteBytes(buffer, 0, buffer.Length);
        }

        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0 || offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (int i = 0, o = offset; i < count; i++, o++)
            {
                this.WriteUBits8(buffer[o], 8);
            }
        }

        public void WriteString(string value)
        {
            if (value == null)
            {
                this.WriteUBits16(0, 16);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            this.WriteUBits16((ushort)bytes.Length, 16);
            this.WriteBytes(bytes);
        }

        public void WriteFunkyValue(object value)
        {
            throw new NotImplementedException();
        }

        public void WriteVector3(Vector3 value)
        {
            this.WriteFloat32(value.X);
            this.WriteFloat32(value.Y);
            this.WriteFloat32(value.Z);
        }

        public void WriteVector4(Vector4 value)
        {
            this.WriteFloat32(value.X);
            this.WriteFloat32(value.Y);
            this.WriteFloat32(value.Z);
            this.WriteFloat32(value.W);
        }

        public void WriteTransform(Transform value)
        {
            this.WriteVector3(value.Right);
            this.WriteVector3(value.Up);
            this.WriteVector3(value.Forward);
            this.WriteVector3(value.Translate);
        }

        public void SkipBits(int bitCount)
        {
            if (bitCount < 0)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            if (this._Position + bitCount >= this._Buffer.Length * 8)
            {
                throw new EndOfStreamException();
            }

            this._Position += bitCount;
        }

        public void SkipBoolean()
        {
            throw new NotImplementedException();
        }

        public void SkipUInt32(int bitCount)
        {
            throw new NotImplementedException();
        }

        private void PushFrame(int bitCount, BitFrameType type)
        {
            var position = this._Position;
            this.WriteUInt32(0, bitCount);
            this._Frames.Push(new BitFrame()
            {
                Position = position,
                Value = (uint)bitCount,
                Type = type,
            });
        }

        public void PushFrameLength(int bitCount)
        {
            this.PushFrame(bitCount, BitFrameType.Length);
        }

        public void PushFramePosition(int bitCount)
        {
            this.PushFrame(bitCount, BitFrameType.Position);
        }

        public uint PopFrameLength()
        {
            if (this._Frames.Count == 0)
            {
                throw new BitReaderException("tried to pop length frame when there was none");
            }

            var frame = this._Frames.Pop();

            if (frame.Type != BitFrameType.Length)
            {
                throw new BitReaderException("invalid type when writing length frame");
            }

            var endPosition = this._Position;
            var length = (uint)(endPosition - (frame.Position + frame.Value));

            this._Position = frame.Position;
            this.WriteUInt32(length, (int)frame.Value);
            this._Position = endPosition;
            
            return length;
        }

        public uint PopFramePosition()
        {
            if (this._Frames.Count == 0)
            {
                throw new BitReaderException("tried to pop position frame when there was none");
            }

            var frame = this._Frames.Pop();

            if (frame.Type != BitFrameType.Position)
            {
                throw new BitReaderException("invalid type when reading position frame");
            }

            var position = this._Position;

            this._Position = frame.Position;
            this.WriteUInt32((uint)(this._BasePosition + position), (int)frame.Value);
            this._Position = position;

            return (uint)position;
        }
    }
}
