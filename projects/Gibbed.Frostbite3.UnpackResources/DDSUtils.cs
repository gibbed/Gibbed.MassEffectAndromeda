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
using System.IO;
using System.Text;
using Gibbed.Frostbite3.ResourceFormats;
using Gibbed.IO;

namespace Gibbed.Frostbite3.UnpackResources
{
    public static class DDSUtils
    {
        public static void WriteFile(TextureHeader textureHeader, byte[] data, Stream output)
        {
            const Endian endian = Endian.Little;
            output.WriteString("DDS ", Encoding.ASCII);

            switch (textureHeader.Format)
            {
                case TextureFormat.BC1_UNORM:
                case TextureFormat.BC1_SRGB:
                case TextureFormat.BC2_UNORM:
                case TextureFormat.BC2_SRGB:
                case TextureFormat.BC3_UNORM:
                case TextureFormat.BC3_SRGB:
                case TextureFormat.BC4_UNORM:
                case TextureFormat.BC5_UNORM:
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.FourCC = DDSHelpers.GetFourCC(textureHeader.Format);
                    header.PixelFormat.Flags = (PixelFormatFlags)4;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.BC6U_FLOAT:
                case TextureFormat.BC7_UNORM:
                case TextureFormat.BC7_SRGB:
                case TextureFormat.R9G9B9E5_FLOAT: // TODO(gibbed): probably not right
                {
                    Header header;
                    ExtendedHeader extendedHeader;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.FourCC = 0x30315844;
                    header.PixelFormat.Flags = (PixelFormatFlags)4;
                    SetupExtendedHeader(out extendedHeader, textureHeader);
                    header.Write(output, endian);
                    extendedHeader.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.R8_UNORM:
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.RedBitMask = 0x000000FF;
                    header.PixelFormat.GreenBitMask = 0x00000000;
                    header.PixelFormat.BlueBitMask = 0x00000000;
                    header.PixelFormat.AlphaBitMask = 0x00000000;
                    header.PixelFormat.RGBBitCount = 8;
                    header.PitchOrLinearSize = textureHeader.Width;
                    header.PixelFormat.Flags = PixelFormatFlags.Luminance;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.R8G8B8A8_UNORM:
                case (TextureFormat)20: // R8G8B8A8_SRGB
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.RedBitMask = 0x000000FF;
                    header.PixelFormat.GreenBitMask = 0x0000FF00;
                    header.PixelFormat.BlueBitMask = 0x00FF0000;
                    header.PixelFormat.AlphaBitMask = 0xFF000000;
                    header.PixelFormat.RGBBitCount = 32;
                    header.PitchOrLinearSize = (uint)textureHeader.Width * 4;
                    header.PixelFormat.Flags = PixelFormatFlags.RGBA;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.R10G10B10A2_UNORM:
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.RedBitMask = 0x000003FF;
                    header.PixelFormat.GreenBitMask = 0x000FFC00;
                    header.PixelFormat.BlueBitMask = 0x3FF00000;
                    header.PixelFormat.AlphaBitMask = 0xC0000000;
                    header.PixelFormat.RGBBitCount = 32;
                    header.PitchOrLinearSize = (uint)textureHeader.Width * 4;
                    header.PixelFormat.Flags = PixelFormatFlags.RGBA;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.R32G32B32A32_FLOAT:
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.FourCC = 0x00000074;
                    header.PitchOrLinearSize = (uint)textureHeader.Width * 16;
                    header.PixelFormat.Flags = (PixelFormatFlags)4;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                case TextureFormat.R16G16B16A16_FLOAT:
                {
                    Header header;
                    SetupHeader(out header, textureHeader);
                    header.PixelFormat.FourCC = 0x00000071;
                    header.PitchOrLinearSize = (uint)textureHeader.Width * 8;
                    header.PixelFormat.Flags = (PixelFormatFlags)4;
                    header.Write(output, endian);
                    output.WriteBytes(data);
                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static void SetupHeader(out Header header, TextureHeader textureHeader)
        {
            header = Header.GetDefault();
            header.Size = Header.GetSize();
            header.Width = textureHeader.Width;
            header.Height = textureHeader.Height;
            header.MipMapCount = textureHeader.MipCount;
            header.Flags = textureHeader.MipCount > 1 ? (HeaderFlags)0x2100F : (HeaderFlags)0xA1007;
            //header.PitchOrLinearSize = ...
            header.PixelFormat.Size = PixelFormat.GetSize();
            header.SurfaceFlags = 0x401008;
        }

        private static void SetupExtendedHeader(out ExtendedHeader header, TextureHeader textureHeader)
        {
            header.Format = DDSHelpers.GetDXGIFormat(textureHeader.Format);
            header.Dimension = 3;
            header.MiscFlags = 0;
            header.ArraySize = 1;
            header.MiscFlags2 = 0;
        }

        [Flags]
        private enum HeaderFlags : uint
        {
            Texture = 0x00001007, // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
            Mipmap = 0x00020000, // DDSD_MIPMAPCOUNT
            Volume = 0x00800000, // DDSD_DEPTH
            Pitch = 0x00000008, // DDSD_PITCH
            LinerSize = 0x00080000, // DDSD_LINEARSIZE
        }

        private struct Header
        {
            public static Header GetDefault()
            {
                return new Header()
                {
                    Reserved1 = new byte[11 * 4],
                    PixelFormat = PixelFormat.GetDefault(),
                    Reserved2 = new byte[3 * 4],
                };
            }

            public static uint GetSize()
            {
                return (18 * 4) + PixelFormat.GetSize() + (5 * 4);
            }

            public uint Size;
            public HeaderFlags Flags;
            public int Height;
            public int Width;
            public uint PitchOrLinearSize;
            public uint Depth;
            public uint MipMapCount;
            public byte[] Reserved1;
            public PixelFormat PixelFormat;
            public uint SurfaceFlags;
            public uint CubemapFlags;
            public byte[] Reserved2;

            public void Write(Stream output, Endian endian)
            {
                output.WriteValueU32(this.Size, endian);
                output.WriteValueU32((uint)this.Flags, endian);
                output.WriteValueS32(this.Height, endian);
                output.WriteValueS32(this.Width, endian);
                output.WriteValueU32(this.PitchOrLinearSize, endian);
                output.WriteValueU32(this.Depth, endian);
                output.WriteValueU32(this.MipMapCount, endian);
                output.Write(this.Reserved1, 0, this.Reserved1.Length);
                this.PixelFormat.Write(output, endian);
                output.WriteValueU32(this.SurfaceFlags, endian);
                output.WriteValueU32(this.CubemapFlags, endian);
                output.Write(this.Reserved2, 0, this.Reserved2.Length);
            }
        }

        private struct ExtendedHeader
        {
            public static ExtendedHeader GetDefault()
            {
                return new ExtendedHeader();
            }

            public uint Format;
            public uint Dimension;
            public uint MiscFlags;
            public uint ArraySize;
            public uint MiscFlags2;

            public void Write(Stream output, Endian endian)
            {
                output.WriteValueU32(this.Format, endian);
                output.WriteValueU32(this.Dimension, endian);
                output.WriteValueU32(this.MiscFlags, endian);
                output.WriteValueU32(this.ArraySize, endian);
                output.WriteValueU32(this.MiscFlags2, endian);
            }
        }

        [Flags]
        private enum PixelFormatFlags : uint
        {
            // ReSharper disable UnusedMember.Local
            // ReSharper disable InconsistentNaming
            FourCC = 0x00000004,
            // ReSharper restore InconsistentNaming
            RGB = 0x00000040,
            RGBA = 0x00000041,
            Luminance = 0x00020000,
            // ReSharper restore UnusedMember.Local
        }

        private struct PixelFormat
        {
            public static PixelFormat GetDefault()
            {
                return new PixelFormat();
            }

            public static uint GetSize()
            {
                return 8 * 4;
            }

            public uint Size;
            public PixelFormatFlags Flags;
            public uint FourCC;
            public uint RGBBitCount;
            public uint RedBitMask;
            public uint GreenBitMask;
            public uint BlueBitMask;
            public uint AlphaBitMask;

            public void Write(Stream output, Endian endian)
            {
                output.WriteValueU32(this.Size, endian);
                output.WriteValueU32((uint)this.Flags, endian);
                output.WriteValueU32(this.FourCC, endian);
                output.WriteValueU32(this.RGBBitCount, endian);
                output.WriteValueU32(this.RedBitMask, endian);
                output.WriteValueU32(this.GreenBitMask, endian);
                output.WriteValueU32(this.BlueBitMask, endian);
                output.WriteValueU32(this.AlphaBitMask, endian);
            }
        }
    }
}
