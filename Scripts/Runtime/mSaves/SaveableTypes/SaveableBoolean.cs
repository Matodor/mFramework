using System;

namespace mFramework.Saves
{
    public sealed class SaveableBoolean : SaveableValue<bool>
    {
        public override string ToString()
        {
            return Value ? "true" : "false";
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(Value);
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex >= array.Length)
                return false;

            Value = BitConverter.ToBoolean(array, startIndex);
            return true;
        }
    }
}