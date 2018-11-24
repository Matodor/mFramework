using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using mFramework.Core;
using mFramework.Storage;

namespace mFramework.Saves
{
    public abstract class Saveable
    {
        public readonly string Key;
        public IKeyValueStorage Storage { get; set; } = mStorage.Instance;

        private static readonly Dictionary<Type, Func<object>> _saveableValueTypes;

        static Saveable()
        {
            _saveableValueTypes = new Dictionary<Type, Func<object>>();
        }

        protected Saveable(string key)
        {
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

                if (!_saveableValueTypes.ContainsKey(cachedField.FieldInfo.FieldType))
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
                            _saveableValueTypes.Add(cachedField.FieldInfo.FieldType, func);
                        }
                        else 
                            throw new Exception("SaveableValue must have public constructor without parameters");
                    }
                    else 
                        continue;
                }

                if (cachedField.GetValue(this) == null)
                    cachedField.SetValue(this, _saveableValueTypes[cachedField.FieldInfo.FieldType]());

                //Debug.Log(string.Join("\n", new string[]
                //{
                //    $"Name={cachedField.FieldInfo.Name}",
                //    $"Value={cachedField.GetValue(this)} ({cachedField.GetValue(this)?.GetType().Name ?? "null"})",
                //    $"Type={cachedField.FieldInfo.FieldType.Name}\n",
                //    $"BaseType ={cachedField.FieldInfo.FieldType.BaseType}",
                //}));
            }
        }

        public virtual string FieldKey(string fieldName)
        {
            return fieldName;
        }

        public void Save()
        {
            BeforeSave();

            var cachedType = mCore.GetCachedType(GetType());
            foreach (var cachedField in cachedType.CachedFields)
            {
                if (!IsSaveableField(cachedField.FieldInfo))
                    continue;

                var key = FieldKey(cachedField.FieldInfo.Name);
                var saveableValue = (SaveableValue) cachedField.GetValue(this);
                saveableValue.Save(Storage, key);
            }

            AfterSave();
        }

        public void Load()
        {
            BeforeLoad();

            var cachedType = mCore.GetCachedType(GetType());
            foreach (var cachedField in cachedType.CachedFields)
            {
                if (!IsSaveableField(cachedField.FieldInfo))
                    continue;

                var key = FieldKey(cachedField.FieldInfo.Name);
                var saveableValue = (SaveableValue) cachedField.GetValue(this);
                saveableValue.Load(Storage, key);
            }

            AfterLoad();
        }

        public static bool IsSaveableField(FieldInfo fieldInfo)
        {
            return _saveableValueTypes.ContainsKey(fieldInfo.FieldType) && 
                   fieldInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(IgnoreAttribute)) == null;
        }

        public virtual void BeforeSave() { }
        public virtual void BeforeLoad() { }

        public virtual void AfterSave() { }
        public virtual void AfterLoad() { }

        //public virtual void OnReset() { }
        //public virtual void OnReload() { }
    }
}