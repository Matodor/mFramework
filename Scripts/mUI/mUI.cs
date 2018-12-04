using UnityEngine;

namespace mFramework.UI
{
    public static class mUI
    {
        public static T Create<T>(UIObject parent = null) where T : UIObject
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}