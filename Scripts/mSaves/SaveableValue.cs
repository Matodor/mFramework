using System;
using System.Collections.Generic;

namespace mFramework.Saves
{
    public delegate void SaveableValueChanged<in T>(T saveableValue);

    public abstract class SaveableValue<T>
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

        public abstract bool Save();
        public abstract void Load();
    }
}