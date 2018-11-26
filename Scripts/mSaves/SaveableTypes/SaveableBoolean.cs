using mFramework.Storage;
using UnityEngine;

namespace mFramework.Saves
{
    public sealed class SaveableBoolean : SaveableValue<bool>
    {
        public override bool Save(IKeyValueStorage storage)
        {
            Debug.Log($"Save {SaveKey}");
            return true;
        }

        public override bool Load(IKeyValueStorage storage)
        {
            Debug.Log($"Load {SaveKey}");
            return true;
        }

        public override string ToString()
        {
            return ProtectedValue ? "true" : "false";
        }
    }
}