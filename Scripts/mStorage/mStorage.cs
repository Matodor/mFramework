using System;

namespace mFramework.Storage
{
    public class mStorage : IKeyValueStorage
    {
        public static IKeyValueStorage Instance { get; } = new mStorage();
        
        public void SetValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, int value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, float value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, bool value)
        {
            
        }

        public bool GetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        public bool GetValue(string key, out int value)
        {
            throw new NotImplementedException();
        }

        public bool GetValue(string key, out float value)
        {
            throw new NotImplementedException();
        }

        public bool GetValue(string key, out bool value)
        {
            throw new NotImplementedException();
        }
    }
}
