using System;
using System.Collections.Generic;

namespace mFramework.Core
{
    public static class mCore
    {
        private static readonly Dictionary<Type, CachedType> _cachedTypes;

        static mCore()
        {
            _cachedTypes = new Dictionary<Type, CachedType>();
        }

        public static CachedType GetCachedType(Type type)
        {
            if (_cachedTypes.ContainsKey(type))
                return _cachedTypes[type];

            var cachedType = new CachedType(type);
            _cachedTypes.Add(type, cachedType);
            return cachedType;
        }
    }
}
