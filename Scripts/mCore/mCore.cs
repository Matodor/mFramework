using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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

            const BindingFlags flags = BindingFlags.NonPublic |
                                       BindingFlags.Instance |
                                       BindingFlags.Public;

            var cachedType = new CachedType(type, flags, flags);
            _cachedTypes.Add(type, cachedType);
            return cachedType;
        }

        public static void WriteStruct<T>(this Stream fs, T structure)
        {
            var buffer = new byte[Marshal.SizeOf(structure)];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), true);
            handle.Free();

            fs.Write(buffer, 0, buffer.Length);
        }

        public static object ReadStruct(this byte[] buffer, Type type, int offset = 0)
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var value = Marshal.PtrToStructure(handle.AddrOfPinnedObject() + offset, type);

            handle.Free();
            return value;
        }

        public static T ReadStruct<T>(this byte[] buffer, int offset = 0)
        {
            return (T) ReadStruct(buffer, typeof(T), offset);
        }

        public static object ReadStruct(this Stream fs, Type type)
        {
            var buffer = new byte[Marshal.SizeOf(type)];
            fs.Read(buffer, 0, buffer.Length);

            return ReadStruct(buffer, type);
        }

        public static T ReadStruct<T>(this Stream fs)
        {
            return (T) ReadStruct(fs, typeof(T));
        }
    }
}
