using System.Reflection;
using System.Reflection.Emit;
using mFramework.Core.Interfaces;

namespace mFramework.Core
{
    public delegate object CachedFieldGetter(object target);

    public delegate void CachedFieldSetter(object target, object value);

    public class CachedField : ICachedMemberInfo
    {
        public MemberInfo MemberInfo => FieldInfo;

        public readonly FieldInfo FieldInfo;
        private readonly CachedFieldGetter _getter;
        private readonly CachedFieldSetter _setter;

        public CachedField(FieldInfo info)
        {
            FieldInfo = info;

            if (!info.IsLiteral || info.IsInitOnly)
            {
                _getter = Getter(info);
                _setter = Setter(info);
            }
            
        }

        public void SetValue(object target, object value)
        {
            _setter(target, value);
        }

        public object GetValue(object target)
        {
            return _getter(target);
        }

        public static CachedFieldGetter Getter(FieldInfo info)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var method = new DynamicMethod("Get" + info.Name, typeof(object),
                new[] {typeof(object)}, info.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, info.DeclaringType); // Cast to source type
            gen.Emit(OpCodes.Ldfld, info);

            if (info.FieldType.IsValueType)
                gen.Emit(OpCodes.Box, info.FieldType);

            gen.Emit(OpCodes.Ret);

            return (CachedFieldGetter) method.CreateDelegate(typeof(CachedFieldGetter));
        }

        public static CachedFieldSetter Setter(FieldInfo info)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var method = new DynamicMethod("Set" + info.Name, null,
                new[] {typeof(object), typeof(object)}, info.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0); // Load target to stack
            gen.Emit(OpCodes.Castclass, info.DeclaringType); // Cast target to source type
            gen.Emit(OpCodes.Ldarg_1); // Load value to stack

            gen.Emit(info.FieldType.IsValueType
                ? OpCodes.Unbox_Any
                : OpCodes.Castclass, info.FieldType);

            gen.Emit(OpCodes.Stfld, info); // Set the value to the input field
            gen.Emit(OpCodes.Ret);

            return (CachedFieldSetter) method.CreateDelegate(typeof(CachedFieldSetter));
        }
    }
}