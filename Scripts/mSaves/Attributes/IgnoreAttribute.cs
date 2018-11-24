using System;

namespace mFramework.Saves
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        
    }
}