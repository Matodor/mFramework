using System;

namespace mFramework.Saves
{
    public class SaveableInt : SaveableValue<int>
    {
        public override string ToString()
        {
            return Value.ToString("N");
        }

        public static byte[] Serialize(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public override byte[] Serialize()
        {
            return Serialize(Value);
        }

        public static int Deserialize(int startIndex, byte[] array)
        {
            return BitConverter.ToInt32(array, startIndex);
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex + 4 > array.Length)
                return false;

            Value = Deserialize(startIndex, array);
            return true;
        }
    }
}