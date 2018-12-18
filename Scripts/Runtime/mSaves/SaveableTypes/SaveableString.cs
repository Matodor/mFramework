using System.Text;

namespace mFramework.Saves
{
    public class SaveableString : SaveableValue<string>
    {
        public override string ToString()
        {
            return Value;
        }

        public override byte[] Serialize()
        {
            return Serialize(Value);
        }

        public static byte[] Serialize(string value)
        {
            return value == null 
                ? null 
                : Encoding.UTF8.GetBytes(value);
        }

        public static string Deserialize(byte[] array, int startIndex, int count)
        {
            return Encoding.UTF8.GetString(array, startIndex, count);
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex >= array.Length)
                return false;

            Value = Deserialize(array, startIndex, array.Length - startIndex);
            return true;
        }
    }
}