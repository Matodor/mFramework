using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using mFramework.Core;
using mFramework.Storage;
using UnityEngine;

namespace mFramework.Saves
{
    public abstract class Saveable
    {
        public readonly string SaveKey;
        public IKeyValueStorage Storage { get; set; } = mStorage.Instance;

        private static readonly Dictionary<Type, Func<SaveableValue>> _saveableValueTypes;

        static Saveable()
        {
            _saveableValueTypes = new Dictionary<Type, Func<SaveableValue>>();
        }

        protected Saveable(string saveKey)
        {
            if (saveKey.Length > KeyAttribute.MaxKeyLength)
                throw new Exception($"Max key length is {KeyAttribute.MaxKeyLength} ({saveKey})");

            SaveKey = saveKey;
            var cachedType = mCore.GetCachedType(GetType());
            var keys = new List<string>();

            foreach (var cachedProperty in cachedType.CachedProperties)
            {
                if (cachedProperty.PropertyInfo.PropertyType.BaseType == null ||
                    cachedProperty.PropertyInfo.PropertyType.IsPrimitive ||
                    cachedProperty.PropertyInfo.PropertyType.IsClass == false ||
                    cachedProperty.PropertyInfo.PropertyType.IsValueType)
                    continue;

                if (cachedProperty.PropertyInfo.PropertyType.IsArray)
                {
                    Debug.LogWarning($"[mSaves] Property '{cachedProperty.PropertyInfo.Name}' in '{cachedType.Type.Name}' is array, skipped");
                    continue;
                }

                if (!cachedProperty.PropertyInfo.PropertyType.BaseType.IsGenericType)
                    continue;

                var enumerable = cachedProperty.PropertyInfo.GetCustomAttributes();
                var attributes = enumerable as Attribute[] ?? enumerable.ToArray();

                if (!_saveableValueTypes.ContainsKey(cachedProperty.PropertyInfo.PropertyType))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var genericDefenition = cachedProperty.PropertyInfo.PropertyType.BaseType
                        .GetGenericTypeDefinition();

                    if (genericDefenition == typeof(SaveableValue<>))
                    {
                        var constructor = cachedProperty.PropertyInfo.PropertyType.GetConstructor(
                            BindingFlags.Instance |
                            BindingFlags.Public, null, Type.EmptyTypes, null);

                        if (constructor != null)
                        {
                            var expression = Expression.New(constructor);
                            var func = (Func<SaveableValue>) Expression
                                .Lambda(expression)
                                .Compile();
                            _saveableValueTypes.Add(cachedProperty.PropertyInfo.PropertyType, func);
                        }
                        else
                            throw new Exception("SaveableValue must have public constructor without parameters");
                    }
                    else
                        continue;
                }

                if (attributes.OfType<IgnoreAttribute>().Any())
                {
                    Debug.Log($"[mSaves] Property '{cachedProperty.PropertyInfo.Name}' in '{cachedType.Type.Name}' marked as Ignore attribute");
                    continue;
                }

                var keyAttribute = attributes.OfType<KeyAttribute>().FirstOrDefault();
                if (keyAttribute == null)
                    throw new Exception($"[mSaves] Property '{cachedProperty.PropertyInfo.Name}' in '{cachedType.Type.Name}' not have a Key attribute");

                var saveableValue = cachedProperty.GetValue(this) as SaveableValue ??
                    _saveableValueTypes[cachedProperty.PropertyInfo.PropertyType]();

                saveableValue.SaveKey = keyAttribute.Key;

                if (cachedProperty.PropertyInfo.CanWrite)
                    cachedProperty.SetValue(this, saveableValue);

                Debug.Log(string.Join("\n", new[]
                {
                    $"cachedProperty Name={cachedProperty.PropertyInfo.Name}",
                    $"Value={cachedProperty.GetValue(this)} ({cachedProperty.GetValue(this)?.GetType().Name ?? "null"})",
                    $"Type={cachedProperty.PropertyInfo.PropertyType.Name}",
                    $"BaseType ={cachedProperty.PropertyInfo.PropertyType.BaseType}",
                    $"HasElementType ={cachedProperty.PropertyInfo.PropertyType.HasElementType}",
                    $"attributes = {attributes.Select(a => a.GetType().Name).Aggregate((a, b) => a + "," + b)}",
                }));
            }
        }

        public void Save()
        {
            BeforeSave();

            var cachedType = mCore.GetCachedType(GetType());
            foreach (var cachedProperty in cachedType.CachedProperties)
            {
                if (!IsSaveableValue(cachedProperty.PropertyInfo))
                    continue;

                var saveableValue = (SaveableValue) cachedProperty.GetValue(this);
                saveableValue.Save(Storage);
            }

            AfterSave();
        }

        public void Load()
        {
            BeforeLoad();

            var cachedType = mCore.GetCachedType(GetType());
            foreach (var cachedProperty in cachedType.CachedProperties)
            {
                if (!IsSaveableValue(cachedProperty.PropertyInfo))
                    continue;

                var saveableValue = (SaveableValue) cachedProperty.GetValue(this);
                saveableValue.Load(Storage);
            }

            AfterLoad();
        }

        public static bool IsSaveableValue(PropertyInfo info)
        {
            return _saveableValueTypes.ContainsKey(info.PropertyType);
        }

        public virtual void BeforeSave()
        {
        }

        public virtual void BeforeLoad()
        {
        }

        public virtual void AfterSave()
        {
        }

        public virtual void AfterLoad()
        {
        }

        //public virtual void OnReset() { }
        //public virtual void OnReload() { }
    }
}