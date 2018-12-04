namespace mFramework.Storage
{
    public interface IKeyValueStorage
    {
        bool SetValue(string key, string value);
        bool SetValue(string key, int value);
        bool SetValue(string key, float value);

        bool GetValue(string key, out string value);
        bool GetValue(string key, out int value);
        bool GetValue(string key, out float value);

        bool ContainsKey(string key);
        bool DeleteKey(string key);
    }
}