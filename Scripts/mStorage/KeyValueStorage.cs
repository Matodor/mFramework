using mFramework.Saves;

namespace mFramework.Storage
{
    public class KeyValueStorage : IKeyValueStorage
    {
        public static readonly KeyValueStorage Instance = new KeyValueStorage();

        private KeyValueStorage()
        {
        }

        public void SetValue(string key, string value)
        {
            mStorage.AddData(key, SaveableString.Serialize(value));
        }

        public void SetValue(string key, int value)
        {
            mStorage.AddData(key, SaveableInt.Serialize(value));
        }

        public void SetValue(string key, float value)
        {
            mStorage.AddData(key, SaveableFloat.Serialize(value));
        }

        public bool GetValue(string key, out string value)
        {
            byte[] data;
            if (mStorage.GetData(key, out data))
            {
                value = SaveableString.Deserialize(data, 0, data.Length);
                return true;
            }

            value = null;
            return false;
        }

        public bool GetValue(string key, out int value)
        {
            byte[] data;
            if (mStorage.GetData(key, out data))
            {
                value = SaveableInt.Deserialize(0, data);
                return true;
            }

            value = default(int);
            return false;
        }

        public bool GetValue(string key, out float value)
        {
            byte[] data;
            if (mStorage.GetData(key, out data))
            {
                value = SaveableFloat.Deserialize(0, data);
                return true;
            }

            value = default(float);
            return false;
        }

        public bool ContainsKey(string key)
        {
            return mStorage.ContainsKey(key);
        }

        public bool DeleteKey(string key)
        {
            return mStorage.RemoveKey(key);
        }
    }
}