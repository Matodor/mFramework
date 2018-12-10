using UnityEngine;

namespace mFramework.UI
{
    public sealed class mUI
    {
        public static BaseView BaseView { get; private set; }

        static mUI()
        {
            BaseView = Create<BaseView>();
            Debug.Log("[mUI] Ctor");
        }

        public static T Create<T>(UIObject parent = null)
            where T : UIObject
        {
            if (parent == null)
                parent = BaseView;

            var obj = new GameObject(typeof(T).Name).AddComponent<T>();
            if (parent != null)
                obj.RectTransform.SetParent(parent.transform);
            return obj;
        }
    }
}