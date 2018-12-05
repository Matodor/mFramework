using UnityEngine;

namespace mFramework.UI
{
    public static class mUI
    {
        public static T Component<T>(UIView parent = null) 
            where T : UIComponent
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }

        public static T View<T>(UIView parent = null)
            where T : UIView
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }
    }
}