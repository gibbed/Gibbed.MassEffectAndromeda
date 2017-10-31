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
    public class NotificationsUnknown5
    {
        #region Fields
        private readonly List<NotificationsUnknown6> _Unknown;
        #endregion

        public NotificationsUnknown5()
        {
            this._Unknown = new List<NotificationsUnknown6>();
        }

        #region Properties
        [JsonProperty("unknown")]
        public List<NotificationsUnknown6> Unknown
        {
            get { return this._Unknown; }
        }
        #endregion

        internal void Read(IBitReader reader, ushort version)
        {
            this._Unknown.Clear();
            if (version < 2)
            {
                var unknownCount = reader.ReadUInt32();
                var types = new NotificationsType[unknownCount];
                for (uint i = 0; i < unknownCount; i++)
                {
                    types[i] = (NotificationsType)reader.ReadUInt32();
                }
                for (uint i = 0; i < unknownCount; i++)
                {
                    var unknown = new NotificationsUnknown6();
                    unknown.Type = types[i];
                    unknown.Unknown1 = reader.ReadUInt8();
                    unknown.Unknown2 = null;
                    this._Unknown.Add(unknown);
                }
            }
            else if (version < 3)
            {
                var unknownCount = reader.ReadUInt32();
                for (uint i = 0; i < unknownCount; i++)
                {
                    var unknown = new NotificationsUnknown6();
                    unknown.Type = (NotificationsType)reader.ReadUInt32();
                    switch (unknown.Type)
                    {
                        case NotificationsType.Unknown0:
                        case NotificationsType.Unknown1:
                        {
                            unknown.Unknown1 = reader.ReadUInt32();
                            break;
                        }

                        case NotificationsType.Unknown2:
                        {
                            unknown.Unknown2 = reader.ReadString();
                            break;
                        }

                        default:
                        {
                            throw new FormatException();
                        }
                    }
                    this._Unknown.Add(unknown);
                }
            }
            else
            {
                var unknownCount = reader.ReadUInt16();
                for (uint i = 0; i < unknownCount; i++)
                {
                    reader.PushFrameLength(24);
                    var unknown = new NotificationsUnknown6();
                    unknown.Type = (NotificationsType)reader.ReadUInt32();
                    switch (unknown.Type)
                    {
                        case NotificationsType.Unknown0:
                        case NotificationsType.Unknown1:
                        {
                            unknown.Unknown1 = reader.ReadUInt32();
                            break;
                        }

                        case NotificationsType.Unknown2:
                        {
                            unknown.Unknown2 = reader.ReadString();
                            break;
                        }

                        default:
                        {
                            throw new FormatException();
                        }
                    }
                    this._Unknown.Add(unknown);
                    reader.PopFrameLength();
                }
            }
        }

        internal void Write(IBitWriter writer)
        {
            writer.WriteUInt16((ushort)this._Unknown.Count);
            foreach (var unknown in this._Unknown)
            {
                writer.PushFrameLength(24);
                writer.WriteUInt32((uint)unknown.Type);
                switch (unknown.Type)
                {
                    case NotificationsType.Unknown0:
                    case NotificationsType.Unknown1:
                    {
                        writer.WriteUInt32(unknown.Unknown1);
                        break;
                    }

                    case NotificationsType.Unknown2:
                    {
                        writer.WriteString(unknown.Unknown2);
                        break;
                    }

                    default:
                    {
                        throw new FormatException();
                    }
                }
                writer.PopFrameLength();
            }
        }
    }
}
