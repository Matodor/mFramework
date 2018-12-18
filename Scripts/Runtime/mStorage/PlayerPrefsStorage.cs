using UnityEngine;

namespace mFramework.Storage
{
    public class PlayerPrefsStorage : IKeyValueStorage
    {
        public static readonly PlayerPrefsStorage Instance 
            = new PlayerPrefsStorage();

        private PlayerPrefsStorage()
        {
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public bool SetValue(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            return true;
        }

        public bool SetValue(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            return true;
        }

        public bool SetValue(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            return true;
        }

        public bool GetValue(string key, out string value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetString(key);
                return true;
            }

            value = default(string);
            return false;
        }

        public bool GetValue(string key, out int value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetInt(key);
                return true;
            }

            value = default(int);
            return false;
        }

        public bool GetValue(string key, out float value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key);
                return true;
            }

            value = default(float);
            return false;
        }

        public bool ContainsKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public bool DeleteKey(string key)
        {
            if (ContainsKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                return true;
            }

            return false;
        }
    }
}