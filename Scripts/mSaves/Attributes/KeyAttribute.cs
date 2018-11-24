using System;

namespace mFramework.Saves
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(string key)
        {

        }
    }
}