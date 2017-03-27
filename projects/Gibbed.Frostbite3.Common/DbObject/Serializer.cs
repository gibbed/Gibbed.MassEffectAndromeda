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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gibbed.IO;

namespace Gibbed.Frostbite3.Common.DbObject
{
    public class Serializer
    {
        private class TypeInfo
        {
            private readonly Dictionary<string, PropertyInfo> _Properties;

            private TypeInfo()
            {
                this._Properties = new Dictionary<string, PropertyInfo>();
            }

            public Dictionary<string, PropertyInfo> Properties
            {
                get { return this._Properties; }
            }

            public static TypeInfo Create(Type type)
            {
                var instance = new TypeInfo();
                foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var propertyAttribute = GetPropertyAttribute(propertyInfo);
                    if (propertyAttribute == null)
                    {
                        continue;
                    }
                    instance._Properties.Add(propertyAttribute.Name, propertyInfo);
                }
                return instance;
            }

            private static PropertyAttribute GetPropertyAttribute(PropertyInfo propertyInfo)
            {
                return (PropertyAttribute)
                       propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
                                   .FirstOrDefault();
            }
        }

        private static class TypeCache
        {
            private static Dictionary<Type, TypeInfo> _Lookup;
            private static readonly object _Lock;

            static TypeCache()
            {
                _Lookup = null;
                _Lock = new object();
            }

            public static TypeInfo Get(Type type)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }

                TypeInfo typeInfo;
                lock (_Lock)
                {
                    if (_Lookup == null)
                    {
                        _Lookup = new Dictionary<Type, TypeInfo>();
                    }

                    if (_Lookup.TryGetValue(type, out typeInfo) == false)
                    {
                        typeInfo = _Lookup[type] = TypeInfo.Create(type);
                    }
                }
                return typeInfo;
            }
        }

        private object ReadArray(Stream input, ValueTag tag, Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (objectType != typeof(object) && objectType.IsArray == false && IsGenericList(objectType) == false)
            {
                throw new ArgumentException("objectType");
            }

            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            Type elementType;
            if (objectType == typeof(object))
            {
                elementType = typeof(object);
            }
            else if (objectType.IsArray == true)
            {
                // Type[]
                elementType = objectType.GetElementType();
            }
            else
            {
                // List<Type>
                elementType = objectType.GetGenericArguments().Single();
            }

            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            var elementTag = new ValueTag(input.ReadValueU8());
            if (elementTag.Type != ValueType.Invalid)
            {
                do
                {
                    var elementValue = this.ReadValue(input, elementTag, elementType);
                    list.Add(elementValue);
                    elementTag = new ValueTag(input.ReadValueU8());
                }
                while (elementTag.Type != ValueType.Invalid);
            }

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }

            if (objectType == typeof(object) || objectType.IsArray != true)
            {
                // just want the list
                return list;
            }

            var array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }
            return array;
        }

        private static bool IsGenericList(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            if (objectType.IsGenericType == false)
            {
                return false;
            }

            var genericType = objectType.GetGenericTypeDefinition();
            if (genericType != typeof(List<>))
            {
                return false;
            }

            return true;
        }

        private Dictionary<string, object> ReadDictionary(Stream input, ValueTag tag, Type objectType)
        {
            if (objectType != typeof(object) && objectType != typeof(Dictionary<string, object>))
            {
                throw new ArgumentException("objectType");
            }

            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            var propertyTag = new ValueTag(input.ReadValueU8());
            var instance = new Dictionary<string, object>();
            while (propertyTag.Type != ValueType.Invalid)
            {
                var propertyName = input.ReadStringZ(Encoding.UTF8);
                var propertyValue = this.ReadValue(input, propertyTag, typeof(object));
                instance.Add(propertyName, propertyValue);
                propertyTag = new ValueTag(input.ReadValueU8());
            }

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }

            return instance;
        }

        private object ReadObject(Stream input, ValueTag tag, Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (objectType == typeof(object) || objectType == typeof(Dictionary<string, object>))
            {
                throw new ArgumentException("objectType");
            }

            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            var typeInfo = TypeCache.Get(objectType);
            var instance = Activator.CreateInstance(objectType);

            var propertyTag = new ValueTag(input.ReadValueU8());
            while (propertyTag.Type != ValueType.Invalid)
            {
                var propertyName = input.ReadStringZ(Encoding.UTF8);
                PropertyInfo propertyInfo;
                if (typeInfo.Properties.TryGetValue(propertyName, out propertyInfo) == false)
                {
                    throw new InvalidOperationException(
                        string.Format("Property '{0}' not found on '{1}'",
                                      propertyName,
                                      objectType.FullName));
                }
                var propertyValue = this.ReadValue(input, propertyTag, propertyInfo.PropertyType);
                propertyInfo.SetValue(instance, propertyValue, null);
                propertyTag = new ValueTag(input.ReadValueU8());
            }

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }

            return instance;
        }

        private object ReadString(Stream input, ValueTag tag, Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (objectType != typeof(object) && objectType != typeof(string))
            {
                throw new ArgumentException("objectType");
            }

            var length = input.ReadPackedValueUInt32();
            var basePosition = input.Position;
            var endPosition = basePosition + length;
            if (endPosition > input.Length)
            {
                throw new EndOfStreamException();
            }

            var value = input.ReadStringZ(Encoding.UTF8);

            if (input.Position != endPosition)
            {
                throw new FormatException();
            }

            return value;
        }

        private object ReadValue(Stream input, ValueTag tag, Type objectType)
        {
            switch (tag.Type)
            {
                case ValueType.Array:
                {
                    return this.ReadArray(input, tag, objectType);
                }

                case ValueType.Object:
                {
                    return objectType == typeof(object) || objectType == typeof(Dictionary<string, object>)
                               ? this.ReadDictionary(input, tag, objectType)
                               : this.ReadObject(input, tag, objectType);
                }

                case ValueType.Bool:
                {
                    return input.ReadValueU8() != 0;
                }

                case ValueType.String:
                {
                    return this.ReadString(input, tag, objectType);
                }

                case ValueType.Int32:
                {
                    return input.ReadValueS32(Endian.Little);
                }

                case ValueType.Int64:
                {
                    return input.ReadValueS64(Endian.Little);
                }

                case ValueType.Guid:
                {
                    return input.ReadValueGuid(Endian.Big);
                }

                case ValueType.SHA1:
                {
                    var bytes = input.ReadBytes(20);
                    return new SHA1(bytes);
                }

                case ValueType.Bytes:
                {
                    var length = input.ReadPackedValueUInt32();
                    var basePosition = input.Position;
                    var endPosition = basePosition + length;
                    if (endPosition > input.Length)
                    {
                        throw new EndOfStreamException();
                    }

                    var value = input.ReadBytes(length);

                    if (input.Position != endPosition)
                    {
                        throw new FormatException();
                    }

                    return value;
                }
            }

            throw new NotSupportedException();
        }

        public object Deserialize(Stream input, Type objectType)
        {
            var tag = new ValueTag(input.ReadValueU8());
            return this.ReadValue(input, tag, objectType);
        }

        public T Deserialize<T>(Stream input)
        {
            return (T)this.Deserialize(input, typeof(T));
        }
    }
}
