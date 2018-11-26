using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace mFramework.Core
{
    public class CachedProperties : IEnumerable<CachedProperty>
    {
        private readonly IEnumerable<CachedProperty> _properties;

        public CachedProperties(Type type)
        {
            var properties = type.GetProperties(
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public
            );

            var array = new CachedProperty[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                //Debug.Log($"PropertyType={properties[i].PropertyType}\n" +
                //          $"Name={properties[i].Name}\n" +
                //          $"MemberType={properties[i].MemberType}\n" +
                //          $"DeclaringType={properties[i].DeclaringType}\n" +
                //          $"ReflectedType={properties[i].ReflectedType}");

                array[i] = new CachedProperty(properties[i]);
            }

            _properties = array;
        }

        public IEnumerator<CachedProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}