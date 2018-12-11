using System.Reflection;

namespace mFramework.Core.Interfaces
{
    public interface ICachedMemberInfo
    {
        MemberInfo MemberInfo { get; }

        void SetValue(object target, object value);
        object GetValue(object target);
    }
}