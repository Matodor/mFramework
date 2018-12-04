﻿using System.Reflection;
using System.Reflection.Emit;

namespace mFramework.Core
{
    public delegate object CachedPropertyGetter(object target);
    public delegate void CachedPropertySetter(object target, object value);

    public class CachedProperty
    {
        public readonly PropertyInfo PropertyInfo;
        private readonly CachedPropertyGetter _getter;
        private readonly CachedPropertySetter _setter;

        public CachedProperty(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            if (propertyInfo.CanRead)
                _getter = Getter(propertyInfo);
            if (propertyInfo.CanWrite)
                _setter = Setter(propertyInfo);
        }

        public void SetValue(object target, object value)
        {
            _setter(target, value);
        }

        public object GetValue(object target)
        {
            return _getter(target);
        }

        public static CachedPropertyGetter Getter(PropertyInfo info)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var method = new DynamicMethod("Get" + info.Name, typeof(object),
                new[] {typeof(object)}, info.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, info.DeclaringType);
            gen.Emit(OpCodes.Callvirt, info.GetGetMethod(true));

            if (info.PropertyType.IsValueType)
                gen.Emit(OpCodes.Box, info.PropertyType);

            gen.Emit(OpCodes.Ret);

            return (CachedPropertyGetter) method.CreateDelegate(typeof(CachedPropertyGetter));
        }

        public static CachedPropertySetter Setter(PropertyInfo info)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var method = new DynamicMethod("Set" + info.Name, null,
                new[] {typeof(object), typeof(object)}, info.DeclaringType, true);
            var gen = method.GetILGenerator();
            
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, info.DeclaringType);
            gen.Emit(OpCodes.Ldarg_1);

            gen.Emit(info.PropertyType.IsValueType 
                ? OpCodes.Unbox_Any 
                : OpCodes.Castclass, info.PropertyType);

            gen.Emit(OpCodes.Callvirt, info.GetSetMethod(true));
            gen.Emit(OpCodes.Ret);

            return (CachedPropertySetter) method.CreateDelegate(typeof(CachedPropertySetter));
        }
    }
}