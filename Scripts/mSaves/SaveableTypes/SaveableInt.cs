using System.IO;
using UnityEngine;

namespace mFramework.Saves
{
    public class SaveableInt : SaveableValue<int>
    {
        public override string ToString()
        {
            return Value.ToString();
        }

        public override byte[] Serialize()
        {
            return new[]
            {
                (byte) Value,
                (byte) (Value >> 8),
                (byte) (Value >> 16),
                (byte) (Value >> 24),
            };
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex + 4 > array.Length)
                return false;

            Value = array[startIndex + 0] |
                    array[startIndex + 1] << 8 |
                    array[startIndex + 2] << 16 |
                    array[startIndex + 3] << 24;
            return true;
        }
    }
}