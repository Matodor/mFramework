using System;

namespace mFramework.Saves
{
    public class SaveableUInt : SaveableValue<uint>
    {
        public override string ToString()
        {
            return Value.ToString("N");
        }

        public static byte[] Serialize(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public override byte[] Serialize()
        {
            return Serialize(Value);
        }

        public static uint Deserialize(int startIndex, byte[] array)
        {
            return BitConverter.ToUInt32(array, startIndex);
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex + sizeof(uint) > array.Length)
                return false;

            Value = Deserialize(startIndex, array);
            return true;
        }
    }
}