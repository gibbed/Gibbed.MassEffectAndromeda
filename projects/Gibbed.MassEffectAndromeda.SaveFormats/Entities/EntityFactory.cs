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

namespace Gibbed.MassEffectAndromeda.SaveFormats.Entities
{
    internal class EntityFactory
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly object _Lock;
        private static Dictionary<uint, Type> _Lookup;
        // ReSharper restore StaticFieldInGenericType

        static EntityFactory()
        {
            _Lock = new object();
            _Lookup = null;
        }

        private static Dictionary<uint, Type> BuildLookup()
        {
            var lookup = new Dictionary<uint, Type>();
            var assembly = typeof(EntityFactory).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                foreach (EntityAttribute attribute in type.GetCustomAttributes(typeof(EntityAttribute), false))
                {
                    lookup.Add(attribute.Hash, type);
                }
            }
            return lookup;
        }

        private static bool Create(uint hash, out Entity instance)
        {
            lock (_Lock)
            {
                if (_Lookup == null)
                {
                    _Lookup = BuildLookup();
                }
            }

            Type type;
            if (_Lookup.TryGetValue(hash, out type) == false)
            {
                instance = default(Entity);
                return false;
            }

            instance = (Entity)Activator.CreateInstance(type);
            return true;
        }

        public static Entity Create(uint hash)
        {
            Entity instance;
            if (Create(hash, out instance) == false)
            {
                throw new ArgumentOutOfRangeException(
                    "hash",
                    "unknown entity for hash '" + hash.ToString("X8") + "'");
            }
            return instance;
        }
    }
}
