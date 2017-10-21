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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Gibbed.MassEffectAndromeda.FileFormats
{
    public class BitReader : IBitReader
    {
        private readonly Stack<BitFrame> _Frames;

        private byte[] _Buffer;
        private BitArray _ReadBits;
        private int _Length;
        private int _Position;
        private int _BasePosition;

        private void Initialize(byte[] buffer, int offset, int length)
        {
            this._Frames.Clear();
            this._ReadBits = new BitArray(length * 8);

            this._Buffer = new byte[length];
            this._Length = length * 8;
            this._Position = 0;
            Array.Copy(buffer, offset, this._Buffer, 0, length);
        }

        public BitReader()
        {
            this._Frames = new Stack<BitFrame>();
        }

        public BitReader(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this._Frames = new Stack<BitFrame>();
            this.Initialize(buffer, 0, buffer.Length);
        }

        public BitReader(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (length < 0 || offset + length < 0 || offset + length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this._Frames = new Stack<BitFrame>();
            this.Initialize(buffer, offset, length);
        }

        public BitReader(byte[] buffer, int basePosition)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this._Frames = new Stack<BitFrame>();
            this.Initialize(buffer, 0, buffer.Length);
            this._BasePosition = basePosition;
        }

        public BitReader(byte[] buffer, int offset, int length, int basePosition)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (length < 0 || offset + length < 0 || offset + length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            this._Frames = new Stack<BitFrame>();
            this.Initialize(buffer, offset, length);
            this._BasePosition = basePosition;
        }

        public int Length
        {
            get { return this._Length; }
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

        private void FlagRead(int count)
        {
            for (int i = this._Position; count > 0; count--, i++)
            {
                if (this._ReadBits[i] == true)
                {
                    throw new FormatException("double read");
                }

                this._ReadBits[i] = true;
            }
        }

        private static string DumpPosition(int position, int padding)
        {
            var s = position.ToString(CultureInfo.InvariantCulture);
            s = s.PadLeft(padding, ' ');
            return s;
        }

        public string DumpRead()
        {
            var padding = (this._Length + 1).ToString(CultureInfo.InvariantCulture).Length;

            var sb = new StringBuilder();
            for (int i = 0; i < this._Length; i++)
            {
                if ((i % 64) == 0)
                {
                    if (i > 0)
                    {
                        sb.AppendLine();
                    }

                    sb.AppendFormat("{0} ", DumpPosition(i, padding));
                }
                else if (i > 0 && (i % 8) == 0)
                {
                    sb.Append(" ");
                }

                sb.Append(this._ReadBits[i] == true ? "1" : "o");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public byte[] DumpReadBitmap()
        {
            var bytes = new byte[this._ReadBits.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = this._ReadBits[i] == true
                               ? (byte)0xFF
                               : (this._Buffer[i >> 3] & (1 << (i % 8))) != 0
                                     ? (byte)1
                                     : (byte)0;
            }
            return bytes;
        }

        public bool HasUnreadBits()
        {
            return this._ReadBits.Cast<bool>().Take(this._ReadBits.Length - 8).Any(b => b == false);
        }

        public void AlignToNextByte()
        {
            this._Position = (this._Position + 7) & ~7;
        }

        private sbyte ReadBits8(int bitCount)
        {
            if (bitCount < 0 || bitCount > 8)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            var value = this.ReadUBits8(bitCount);
            if ((value & 1u << (bitCount - 1)) != 0)
            {
                value |= (byte)(0xFFu << bitCount);
            }
            return (sbyte)value;
        }

        private byte ReadUBits8(int bitCount)
        {
            if (bitCount < 0 || bitCount > 8)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            byte result = 0;
            int shift = 0;
            while (bitCount > 0)
            {
                if (this._Position >= this._Length)
                {
                    throw new EndOfStreamException();
                }

                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                this.FlagRead(left);

                var mask = (byte)((1 << left) - 1);
                var value = this._Buffer[this._Position >> 3];
                value >>= offset;

                result |= (byte)((mask & value) << shift);

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
            return result;
        }

        private short ReadBits16(int bitCount)
        {
            if (bitCount < 0 || bitCount > 16)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            var value = this.ReadUBits16(bitCount);
            if ((value & 1u << (bitCount - 1)) != 0)
            {
                value |= (ushort)(0xFFFFu << bitCount);
            }
            return (short)value;
        }

        private ushort ReadUBits16(int bitCount)
        {
            if (bitCount < 0 || bitCount > 16)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            ushort result = 0;
            int shift = 0;
            while (bitCount > 0)
            {
                if (this._Position >= this._Length)
                {
                    throw new EndOfStreamException();
                }

                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                this.FlagRead(left);

                var mask = (byte)((1 << left) - 1);
                var value = this._Buffer[this._Position >> 3];
                value >>= offset;

                result |= (ushort)((mask & value) << shift);

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
            return result;
        }

        private int ReadBits32(int bitCount)
        {
            if (bitCount < 0 || bitCount > 32)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            var value = this.ReadUBits32(bitCount);
            if (bitCount < 32 && (value & 1u << (bitCount - 1)) != 0)
            {
                value |= 0xFFFFFFFFu << bitCount;
            }
            return (int)value;
        }

        private uint ReadUBits32(int bitCount)
        {
            if (bitCount < 0 || bitCount > 32)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            uint result = 0;
            int shift = 0;
            while (bitCount > 0)
            {
                if (this._Position >= this._Length)
                {
                    throw new EndOfStreamException();
                }

                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                this.FlagRead(left);

                var mask = (byte)((1 << left) - 1);
                var value = this._Buffer[this._Position >> 3];
                value >>= offset;

                result |= (uint)(mask & value) << shift;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }

            return result;
        }

        private long ReadBits64(int bitCount)
        {
            var value = this.ReadUBits64(bitCount);
            if ((value & 1ul << (bitCount - 1)) != 0)
            {
                value |= 0xFFFFFFFFFFFFFFFFul << bitCount;
            }
            return (long)value;
        }

        private ulong ReadUBits64(int bitCount)
        {
            if (bitCount < 0 || bitCount > 64)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            if (bitCount <= 32)
            {
                return this.ReadUBits32(bitCount);
            }

            ulong result = 0;
            int shift = 0;
            while (bitCount > 0)
            {
                if (this._Position >= this._Length)
                {
                    throw new EndOfStreamException();
                }

                int offset = this._Position % 8;
                int left = Math.Min(8 - offset, bitCount);

                this.FlagRead(left);

                var mask = (byte)((1 << left) - 1);
                var value = this._Buffer[this._Position >> 3];
                value >>= offset;

                result |= (ulong)(mask & value) << shift;

                bitCount -= left;
                shift += left;

                this._Position += left;
            }
            return result;
        }

        public bool ReadBoolean()
        {
            if (this._Position + 1 > this._Length)
            {
                throw new EndOfStreamException();
            }

            this.FlagRead(1);
            int offset = this._Position % 8;
            var result = (this._Buffer[this._Position >> 3] & (1 << offset)) != 0;
            this._Position++;
            return result;
        }

        public sbyte ReadInt8()
        {
            return this.ReadBits8(8);
        }

        public sbyte ReadInt8(int bitCount)
        {
            return this.ReadBits8(bitCount);
        }

        public byte ReadUInt8()
        {
            return this.ReadUBits8(8);
        }

        public byte ReadUInt8(int bitCount)
        {
            return this.ReadUBits8(bitCount);
        }

        public short ReadInt16()
        {
            return this.ReadBits16(16);
        }

        public short ReadInt16(int bitCount)
        {
            return this.ReadBits16(bitCount);
        }

        public ushort ReadUInt16()
        {
            return this.ReadUBits16(16);
        }

        public ushort ReadUInt16(int bitCount)
        {
            return this.ReadUBits16(bitCount);
        }

        public int ReadInt32()
        {
            return this.ReadBits32(32);
        }

        public int ReadInt32(int bitCount)
        {
            return this.ReadBits32(bitCount);
        }

        public uint ReadUInt32()
        {
            return this.ReadUBits32(32);
        }

        public uint ReadUInt32(int bitCount)
        {
            return this.ReadUBits32(bitCount);
        }

        public long ReadInt64()
        {
            return this.ReadBits64(64);
        }

        public long ReadInt64(int bitCount)
        {
            return this.ReadBits64(bitCount);
        }

        public ulong ReadUInt64()
        {
            return this.ReadUBits64(64);
        }

        public ulong ReadUInt64(int bitCount)
        {
            return this.ReadUBits64(bitCount);
        }

        public float ReadFloat32()
        {
            var rawValue = this.ReadBytes(4);
            return BitConverter.ToSingle(rawValue, 0);
        }

        public Guid ReadGuid()
        {
            var bytes = this.ReadBytes(16);
            return new Guid(bytes);
        }

        public byte[] ReadBits(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return new byte[0];
            }

            var totalByteCount = (length + 7) / 8;
            var byteCount = length / 8;
            var bytes = new byte[totalByteCount];
            var shift = this._Position % 8;

            if (shift == 0)
            {
                // we can cheat and use a straight array copy
                Array.Copy(this._Buffer, this._Position >> 3, bytes, 0, byteCount);
                this.FlagRead(8 * byteCount);
                this.SkipBits(8 * byteCount);
                length -= 8 * byteCount;
                if (length > 0)
                {
                    bytes[byteCount] = this.ReadUBits8(length);
                }
            }
            else
            {
                for (int i = 0; i < byteCount; i++, length -= 8)
                {
                    bytes[i] = this.ReadUBits8(8);
                }
                if (length > 0)
                {
                    bytes[byteCount] = this.ReadUBits8(length);
                }
            }

            return bytes;
        }

        public void ReadBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            this.ReadBytes(buffer, 0, buffer.Length);
        }

        public void ReadBytes(byte[] buffer, int offset, int count)
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
                buffer[o] = this.ReadUBits8(8);
            }
        }

        public byte[] ReadBytes(int length)
        {
            var bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = this.ReadUBits8(8);
            }
            return bytes;
        }

        public string ReadString()
        {
            var length = this.ReadUBits16(16);
            if (length == 0)
            {
                return null;
            }
            var data = this.ReadBytes(length);
            var zeroIndex = Array.IndexOf<byte>(data, 0);
            if (zeroIndex >= 0)
            {
                return Encoding.UTF8.GetString(data, 0, zeroIndex);
            }
            return Encoding.UTF8.GetString(data);
        }

        public object ReadFunkyValue()
        {
            var unknown0 = this.ReadBoolean();
            if (unknown0 == false)
            {
                var unknown1 = this.ReadUInt32();
                var unknown2 = this.ReadUInt32();
                var unknown3 = this.ReadBoolean();
                if (unknown3 == false)
                {
                    var unknown4 = this.ReadBytes(16);
                }
            }
            return null;
        }

        public Vector3 ReadVector3()
        {
            var instance = new Vector3();
            instance.X = this.ReadFloat32();
            instance.Y = this.ReadFloat32();
            instance.Z = this.ReadFloat32();
            return instance;
        }

        public Vector4 ReadVector4()
        {
            var instance = new Vector4();
            instance.X = this.ReadFloat32();
            instance.Y = this.ReadFloat32();
            instance.Z = this.ReadFloat32();
            instance.W = this.ReadFloat32();
            return instance;
        }

        public Transform ReadTransform()
        {
            var instance = new Transform();
            instance.Right = this.ReadVector3();
            instance.Up = this.ReadVector3();
            instance.Forward = this.ReadVector3();
            instance.Translate = this.ReadVector3();
            return instance;
        }

        public void SkipBits(int bitCount)
        {
            if (bitCount < 0)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            if (this._Position + bitCount >= this._Length)
            {
                throw new EndOfStreamException();
            }

            this._Position += bitCount;
        }

        public void SkipBoolean()
        {
            this.SkipBits(1);
        }

        public void SkipUInt32(int bitCount)
        {
            if (bitCount < 0 || bitCount > 32)
            {
                throw new ArgumentOutOfRangeException("bitCount");
            }

            this.SkipBits(bitCount);
        }

        private uint PushFrame(int bitCount, BitFrameType type)
        {
            var value = this.ReadUInt32(bitCount);
            this._Frames.Push(new BitFrame()
            {
                Position = this._Position,
                Value = value,
                Type = type,
            });
            return value;
        }

        public uint PushFrameLength(int bitCount)
        {
            return this.PushFrame(bitCount, BitFrameType.Length);
        }

        public uint PushFramePosition(int bitCount)
        {
            return this.PushFrame(bitCount, BitFrameType.Position);
        }

        public void PopFrameLength()
        {
            if (this._Frames.Count == 0)
            {
                throw new BitReaderException("tried to pop length frame when there was none");
            }

            var frame = this._Frames.Pop();

            if (frame.Type != BitFrameType.Length)
            {
                throw new BitReaderException("invalid type when reading length frame");
            }

            var endPosition = frame.Position + (int)frame.Value;
            if (endPosition != this._Position)
            {
                if (this._Position < endPosition)
                {
                    throw new BitReaderException("invalid position when reading length frame (underrun of " +
                                                 (endPosition - this._Position) +
                                                 " bits)");
                }

                throw new BitReaderException("invalid position when reading length frame (overrun of " +
                                             (this._Position - endPosition) +
                                             " bits)");
            }
        }

        public void PopFramePosition()
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

            if ((int)frame.Value != this._BasePosition + this._Position)
            {
                throw new BitReaderException("invalid position when reading position frame");
            }
        }
    }
}
