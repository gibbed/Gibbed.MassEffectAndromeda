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

namespace Gibbed.Frostbite3.Dynamic
{
    internal class PartitionObject : DynamicObject
    {
        private static readonly string[] _ExtraDynamicMemberNames;

        static PartitionObject()
        {
            _ExtraDynamicMemberNames = new[] { "__INSTANCE", "__GUID", "__TYPE" };
        }

        private readonly PartitionInstance _Instance;

        public PartitionObject(PartitionInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this._Instance = instance;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _ExtraDynamicMemberNames.Concat(this._Instance.GetDynamicMemberNames());
        }

        public bool HasMember(string name)
        {
            return this._Instance.HasMember(name);
        }

        // TODO(gibbed): hack, remove this eventually
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == "HasMember")
            {
                result = this.HasMember((string)args[0]);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "__INSTANCE")
            {
                result = this._Instance;
                return true;
            }

            if (binder.Name == "__GUID")
            {
                result = this._Instance.Guid;
                return true;
            }

            if (binder.Name == "__TYPE")
            {
                result = this._Instance.Type.Name;
                return true;
            }

            return this._Instance.TryGetMember(binder, out result);
        }

        public override string ToString()
        {
            return "^" + this._Instance;
        }
    }
}
