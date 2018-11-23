using System;

namespace mFramework.Core
{
    public class CachedType
    {
        public readonly Type Type;
        public readonly CachedFields CachedFields;
        public readonly CachedProperties CachedProperties;

        public CachedType(Type type)
        {
            Type = type;
            CachedFields = new CachedFields(type);
            CachedProperties = new CachedProperties(type);
        }
    }
}