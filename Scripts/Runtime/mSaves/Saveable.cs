// ReSharper disable InlineOutVariableDeclaration
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
        public bool IsModified { get; private set; }

        protected readonly Dictionary<string, CachedProperty> SaveableProperties;

        private static readonly Dictionary<Type, Func<SaveableValue>> _saveableValueTypes;

        static Saveable()
        {
            _saveableValueTypes = new Dictionary<Type, Func<SaveableValue>>();
        }

        protected Saveable(string saveKey)
        {
            if (saveKey.Length > KeyAttribute.MaxKeyLength)
                throw new Exception($"Saveable key max length={KeyAttribute.MaxKeyLength} ({saveKey})");

            IsModified = false;
            SaveableProperties = new Dictionary<string, CachedProperty>();
            SaveKey = saveKey;
            var cachedType = CachedType.Get(GetType());

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

                if (!IsSaveableValue(cachedProperty.PropertyInfo))
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

                if (SaveableProperties.ContainsKey(keyAttribute.Key)) 
                    throw new Exception($"[mSaves] Key {keyAttribute.Key} already exist");

                if (cachedProperty.PropertyInfo.CanWrite)
                {
                    cachedProperty.SetValue(this,
                        cachedProperty.GetValue(this) as SaveableValue ??
                        _saveableValueTypes[cachedProperty.PropertyInfo.PropertyType]()
                    );
                }

                SaveableProperties.Add(keyAttribute.Key, cachedProperty);

                //Debug.Log(string.Join("\n", new[]
                //{
                //    $"cachedProperty Name={cachedProperty.PropertyInfo.Name}",
                //    $"Value={cachedProperty.GetValue(this)} ({cachedProperty.GetValue(this)?.GetType().Name ?? "null"})",
                //    $"Type={cachedProperty.PropertyInfo.PropertyType.Name}",
                //    $"BaseType ={cachedProperty.PropertyInfo.PropertyType.BaseType}",
                //    $"HasElementType ={cachedProperty.PropertyInfo.PropertyType.HasElementType}",
                //    $"attributes = {attributes.Select(a => a.GetType().Name).Aggregate((a, b) => a + "," + b)}",
                //}));
            }
        }

        public virtual void Save()
        {
            BeforeSave();
            
            var list = new List<byte>();
            foreach (var pair in SaveableProperties)
            {
                var saveableValue = (SaveableValue) pair.Value.GetValue(this);
                var data = saveableValue.Serialize();
                if (data == null || data.Length == 0)
                    continue;

                var key = CombineKey(SaveKey, pair.Key);
                mStorage.AddData(key, data);
                list.AddRange(data);
            }

            var crc = Crc32.ComputeChecksum(list.ToArray());
            KeyValueStorage.Instance.SetValue($"_crc_{SaveKey}", crc);
            Debug.Log($"[mSaves] Saved {SaveKey} with gotcrc={crc}");

            AfterSave();
        }

        public virtual void Load()
        {
            BeforeLoad();

            var list = new List<byte>();
            foreach (var pair in SaveableProperties)
            {
                var saveableValue = (SaveableValue) pair.Value.GetValue(this);
                var key = CombineKey(SaveKey, pair.Key);

                byte[] data;
                if (mStorage.GetData(key, out data))
                {
                    saveableValue.Deserialize(data, 0);
                    list.AddRange(data);
                }
            }

            var crc = Crc32.ComputeChecksum(list.ToArray());
            uint wantedCrc;

            if (KeyValueStorage.Instance.GetValue($"_crc_{SaveKey}", out wantedCrc))
            {
                if (crc != wantedCrc)
                {
                    IsModified = true;
                    Debug.LogWarning($"[mSaves] Loaded {SaveKey} with gotcrc={crc} wantedcrc={wantedCrc}");
                }
                else
                    Debug.Log($"[mSaves] Loaded {SaveKey} with gotcrc={crc} wantedcrc={wantedCrc}");
            }
            else
                Debug.Log($"[mSaves] Loaded {SaveKey} with crc={crc}");

            AfterLoad();
        }

        public static string CombineKey(string saveableKey, string saveableValueKey)
        {
            return $"{saveableKey}_{saveableValueKey}";
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

        public virtual void OnReset()
        {
        }

        public virtual void OnReload()
        {
        }
    }
}