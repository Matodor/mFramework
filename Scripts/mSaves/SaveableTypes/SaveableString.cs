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
            return Encoding.UTF8.GetBytes(Value);
        }

        public override bool Deserialize(byte[] array, int startIndex)
        {
            if (array == null || array.Length == 0 || startIndex >= array.Length)
                return false;

            Value = Encoding.UTF8.GetString(array, startIndex, array.Length - startIndex);
            return true;
        }
    }
}