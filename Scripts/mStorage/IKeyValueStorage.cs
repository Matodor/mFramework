namespace mFramework.Storage
{
    public interface IKeyValueStorage
    {
        void SetValue(string key, string value);
        void SetValue(string key, int value);
        void SetValue(string key, float value);
        void SetValue(string key, bool value);

        bool GetValue(string key, out string value);
        bool GetValue(string key, out int value);
        bool GetValue(string key, out float value);
        bool GetValue(string key, out bool value);
    }
}