using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace mFramework.Core
{
    public class CachedFields : IEnumerable<CachedField>
    {
        private readonly IEnumerable<CachedField> _fields;

        public CachedFields(Type type)
        {
            var fields = type.GetFields(
                BindingFlags.NonPublic | 
                BindingFlags.Instance | 
                BindingFlags.Public
            );

            var array = new CachedField[fields.Length];
            for (var i = 0; i < fields.Length; i++)
            {
                //Debug.Log($"FieldType={fields[i].FieldType}\n" +
                //          $"MemberType={fields[i].MemberType}\n" +
                //          $"DeclaringType={fields[i].DeclaringType}\n" +
                //          $"ReflectedType={fields[i].ReflectedType}");

                array[i] = new CachedField(fields[i]);
            }

            _fields = array;
        }

        public IEnumerator<CachedField> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}