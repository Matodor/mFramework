using System;
using System.Reflection;

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
    }
}