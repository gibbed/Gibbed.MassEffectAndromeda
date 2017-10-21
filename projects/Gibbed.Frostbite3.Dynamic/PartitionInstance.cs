#define DEBUG_READING

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
using System.Dynamic;
using System.Linq;
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;

namespace Gibbed.Frostbite3.Dynamic
{
    public class PartitionInstance
    {
        private readonly PartitionReader _Reader;
        private readonly Guid _Guid;
        private readonly long _DataOffset;
        private readonly PartitionFlattenedType _Type;
        private readonly Dictionary<string, object> _FieldCache;

        internal PartitionInstance(PartitionReader reader, Guid guid, long dataOffset, PartitionFlattenedType type)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this._Reader = reader;
            this._Guid = guid;
            this._DataOffset = dataOffset;
            this._Type = type;
            this._FieldCache = new Dictionary<string, object>();
        }

        public Guid Guid
        {
            get { return this._Guid; }
        }

        internal PartitionFlattenedType Type
        {
            get { return this._Type; }
        }

        internal PartitionObject ToObject()
        {
            return new PartitionObject(this);
        }

        public IEnumerable<string> GetDynamicMemberNames()
        {
            return this._Type.Fields.Select(fd => fd.Name);
        }

        public bool HasMember(string name)
        {
            var nameHash = DJB.Compute(name);
            var fieldIndex = Array.FindIndex(this._Type.Fields, fd => fd.NameHash == nameHash);
            return fieldIndex >= 0;
        }

        public bool TryGetMember(string name, out object result)
        {
            if (this._FieldCache.ContainsKey(name) == true)
            {
                result = this._FieldCache[name];
                return true;
            }

            var nameHash = DJB.Compute(name);
            var fieldIndex = Array.FindIndex(this._Type.Fields, fd => fd.NameHash == nameHash);
            if (fieldIndex < 0)
            {
                result = null;
                return false;
            }

            var field = this._Type.Fields[fieldIndex];

#if DEBUG_READING
            try
#endif
            {
                result = this._Reader.ReadField(this._DataOffset, field);
            }
#if DEBUG_READING
            catch (Exception e)
            {
                result = e;
                return true;
            }
#endif

            this._FieldCache.Add(name, result);
            return true;
        }

        public override string ToString()
        {
            return "instance of `" + this._Type + "`";
        }
    }
}
