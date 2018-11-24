using System;
using System.Collections.Generic;
using mFramework.Storage;

namespace mFramework.Saves
{
    public delegate void SaveableValueChanged<in T>(T saveableValue);

    public abstract class SaveableValue
    {
        public abstract bool Save(IKeyValueStorage storage, string key);
        public abstract bool Load(IKeyValueStorage storage, string key);
    }

    public abstract class SaveableValue<T> : SaveableValue
    {
        public event SaveableValueChanged<T> ValueChanged;

        public T Value
        {
            get { return ProtectedValue; }
            set
            {
                if (Comparer<T>.Default.Compare(value, ProtectedValue) != 0)
                    ValueChanged?.Invoke(value);
                ProtectedValue = value;
            }
        }

        protected T ProtectedValue;
                
        public static implicit operator T(SaveableValue<T> saveable)
        {
            return saveable.ProtectedValue;
        }
    }
}