using System.Collections.Generic;

namespace mFramework.Saves
{
    public delegate void SaveableValueChanged<in T>(T saveableValue);

    public abstract class SaveableValue
    {
        public abstract byte[] Serialize();
        public abstract bool Deserialize(byte[] array, int startIndex);
    }

    public abstract class SaveableValue<T> : SaveableValue
    {
        public event SaveableValueChanged<T> ValueChanged;

        public T Value
        {
            get { return _value; }
            set
            {
                if (Comparer<T>.Default.Compare(value, _value) != 0)
                    ValueChanged?.Invoke(value);
                _value = value;
            }
        }

        private T _value;
                
        public static implicit operator T(SaveableValue<T> saveable)
        {
            return saveable._value;
        }
    }
}