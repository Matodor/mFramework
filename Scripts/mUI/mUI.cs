using UnityEngine;

namespace mFramework.UI
{
    public static class mUI
    {
        public static T Create<T>(UIObject parent = null)
            where T : UIObject
        {
            if (parent == null)
                return new GameObject(typeof(T).Name).AddComponent<T>();

            var o = new GameObject(typeof(T).Name).AddComponent<T>();
            o.transform.SetParent(parent.transform);
            return o;
        }
    }
}