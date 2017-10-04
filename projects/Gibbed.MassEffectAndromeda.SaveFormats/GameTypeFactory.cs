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
using DJB = Gibbed.Frostbite3.Common.Hashing.DJB;

namespace Gibbed.MassEffectAndromeda.SaveFormats
{
    internal abstract class GameTypeFactory<T, A>
        where A : Attribute, IGameTypeAttribute
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly object _Lock;
        private static Dictionary<uint, Type> _Lookup;
        // ReSharper restore StaticFieldInGenericType

        static GameTypeFactory()
        {
            _Lock = new object();
            _Lookup = null;
        }

        public static uint GetNameHash(string name)
        {
            return DJB.Compute(name);
        }

        private static Dictionary<uint, Type> BuildLookup()
        {
            var lookup = new Dictionary<uint, Type>();
            var assembly = typeof(GameTypeFactory<,>).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetSingleAttribute<A>();
                if (attribute == null)
                {
                    continue;
                }

                if (typeof(T).IsAssignableFrom(type) == false)
                {
                    throw new InvalidOperationException(string.Format("'{0}' has has {1} but isn't a {2}",
                                                                      type.Name,
                                                                      typeof(A).Name,
                                                                      typeof(T).Name));
                    continue;
                }

                var hash = GetNameHash(attribute.Name);
                lookup.Add(hash, type);
            }
            return lookup;
        }

        private static bool Create(uint nameHash, out T instance)
        {
            lock (_Lock)
            {
                if (_Lookup == null)
                {
                    _Lookup = BuildLookup();
                }
            }

            Type type;
            if (_Lookup.TryGetValue(nameHash, out type) == false)
            {
                instance = default(T);
                return false;
            }

            instance = (T)Activator.CreateInstance(type);
            return true;
        }

        public static T Create(uint nameHash)
        {
            T instance;
            if (Create(nameHash, out instance) == false)
            {
                throw new ArgumentOutOfRangeException(
                    "nameHash",
                    "unknown agent for hash '" + nameHash.ToString("X8") + "'");
            }
            return instance;
        }

        public static T Create(string name)
        {
            T instance;
            var nameHash = GetNameHash(name);
            if (Create(nameHash, out instance) == false)
            {
                throw new ArgumentOutOfRangeException(
                    "name",
                    "unknown agent for name '" + name + "'");
            }
            return instance;
        }
    }
}
