using System;
using System.Reflection;
using mFramework.Core.Interfaces;

namespace mFramework.Core
{
    public class CachedType
    {
        public readonly Type Type;
        public readonly CachedFields CachedFields;
        public readonly CachedProperties CachedProperties;

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
    }
}