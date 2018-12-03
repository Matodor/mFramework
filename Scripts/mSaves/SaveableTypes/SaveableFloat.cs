using System;

namespace mFramework.Saves
{
    public class SaveableFloat : SaveableValue<float>
    {
        public override string ToString()
        {
            return Value.ToString("R");
        }

        public static byte[] Serialize(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public override byte[] Serialize()
        {
            return Serialize(Value);
        }

        public static float Deserialize(int startIndex, byte[] array)
        {
            return BitConverter.ToSingle(array, startIndex);
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