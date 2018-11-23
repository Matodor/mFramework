using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using mFramework.Core;
using UnityEngine;

namespace mFramework.Saves
{
    public abstract class Saveable
    {
        public readonly string Key;

        private static readonly Dictionary<Type, Func<object>> _saveableTypes;

        static Saveable()
        {
            _saveableTypes = new Dictionary<Type, Func<object>>();
        }

        protected Saveable(string key)
        {
            // TODO backing fields from properties
            Key = key;

            var cachedType = mCore.GetCachedType(GetType());
            foreach (var cachedField in cachedType.CachedFields)
            {
                if (cachedField.FieldInfo.FieldType.IsPrimitive ||
                    cachedField.FieldInfo.FieldType.IsClass == false ||
                    cachedField.FieldInfo.FieldType.IsValueType ||
                    cachedField.FieldInfo.FieldType.BaseType == null)
                    continue;

                if (!cachedField.FieldInfo.FieldType.BaseType.IsGenericType)
                    return;

                if (!_saveableTypes.ContainsKey(cachedField.FieldInfo.FieldType))
                {
                    var genericDefenition = cachedField.FieldInfo.FieldType.BaseType.GetGenericTypeDefinition();
                    if (genericDefenition == typeof(SaveableValue<>))
                    {
                        var constructor = cachedField.FieldInfo.FieldType.GetConstructor(
                            BindingFlags.Instance |
                            BindingFlags.Public, null, Type.EmptyTypes, null);

                        if (constructor != null)
                        {
                            var e = Expression.New(constructor);
                            var func = (Func<object>) Expression.Lambda(e).Compile();
                            _saveableTypes.Add(cachedField.FieldInfo.FieldType, func);
                        }
                        else 
                            throw new Exception("SaveableValue must have public constructor without parameters");
                    }
                    else 
                        continue;
                }

                if (cachedField.GetValue(this) == null)
                    cachedField.SetValue(this, _saveableTypes[cachedField.FieldInfo.FieldType]());

                //Debug.Log(string.Join("\n", new string[]
                //{
                //    $"Name={cachedField.FieldInfo.Name}",
                //    $"Value={cachedField.GetValue(this)} ({cachedField.GetValue(this)?.GetType().Name ?? "null"})",
                //    $"Type={cachedField.FieldInfo.FieldType.Name}\n",
                //    $"BaseType ={cachedField.FieldInfo.FieldType.BaseType}",
                //}));
            }
        }

        public virtual void BeforeSave() { }
        public virtual void BeforeLoad() { }

        public virtual void AfterSave() { }
        public virtual void AfterLoad() { }

        //public virtual void OnReset() { }
        //public virtual void OnReload() { }
    }
}