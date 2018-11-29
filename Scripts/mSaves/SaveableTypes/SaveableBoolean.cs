﻿namespace mFramework.Saves
{
    public sealed class SaveableBoolean : SaveableValue<bool>
    {
        private const byte ByteFalse = 0;
        private const byte ByteTrue = 1;

        public override string ToString()
        {
            return Value ? "true" : "false";
        }

        public override byte[] Serialize()
        {
            return new [] { Value ? ByteTrue : ByteFalse};
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex >= array.Length)
                return false;

            Value = array[startIndex] == ByteTrue;
            return true;
        }
    }
}