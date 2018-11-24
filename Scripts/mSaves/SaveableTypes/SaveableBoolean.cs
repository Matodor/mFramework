using System;
using mFramework.Storage;
using UnityEngine;

namespace mFramework.Saves
{
    public sealed class SaveableBoolean : SaveableValue<bool>
    {
        public override bool Save(IKeyValueStorage storage, string key)
        {
            Debug.Log($"Save {key}");
            return true;
        }

        public override bool Load(IKeyValueStorage storage, string key)
        {
            Debug.Log($"Load {key}");
            return true;
        }

        public override string ToString()
        {
            return ProtectedValue ? "true" : "false";
        }
    }
}