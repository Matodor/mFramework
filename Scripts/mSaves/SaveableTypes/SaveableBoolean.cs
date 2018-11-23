using System;

namespace mFramework.Saves
{
    public sealed class SaveableBoolean : SaveableValue<bool>
    {
        public override bool Save()
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }
        
        public override string ToString()
        {
            return ProtectedValue ? "true" : "false";
        }
    }
}