using System;
using System.Collections.Generic;
using System.Reflection;
using mFramework.Core.Interfaces;

namespace mFramework.Core
{
    public class CachedType
    {
        public readonly Type Type;
        public readonly CachedFields CachedFields;
        public readonly CachedProperties CachedProperties;

        private static readonly Dictionary<Type, CachedType> _cachedTypes;
        private const BindingFlags DefaultFlags = BindingFlags.NonPublic |
                                                  BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.Static;

        static CachedType()
        {
            _cachedTypes = new Dictionary<Type, CachedType>();
        }

        public CachedType(Type type, 
            BindingFlags propertiesBindingFlags,
            BindingFlags fieldsBindingFlags)
        {
            Type = type;
            CachedFields = new CachedFields(type, fieldsBindingFlags);
            CachedProperties = new CachedProperties(type, propertiesBindingFlags);
        }

        public ICachedMemberInfo GetFieldOrProperty(string name)
        {
            foreach (var cachedField in CachedFields)
            {
                if (cachedField.FieldInfo.Name == name)
                    return cachedField;
            }

            foreach (var cachedProperty in CachedProperties)
            {
                if (cachedProperty.PropertyInfo.Name == name)
                    return cachedProperty;
            }

            return null;
        }

        public static CachedType Get(Type type, 
            BindingFlags fieldsFlags = DefaultFlags,
            BindingFlags propsFlags = DefaultFlags)
        {
            if (_cachedTypes.ContainsKey(type))
                return _cachedTypes[type];

            var cachedType = new CachedType(type, propsFlags, fieldsFlags);
            _cachedTypes.Add(type, cachedType);
            return cachedType;
        }
    }
}