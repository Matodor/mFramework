using System;

namespace mFramework.Saves
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public const int MaxKeyLength = 10;
        public readonly string Key;

        public KeyAttribute(string key)
        {
            if (key.Length > MaxKeyLength)
                throw new Exception($"Max key length is {MaxKeyLength} ({key})");
            Key = key;
        }
    }
}