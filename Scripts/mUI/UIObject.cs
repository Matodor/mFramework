using UnityEngine;

namespace mFramework.UI
{
    public abstract partial class UIObject : MonoBehaviour
    {
        public UIView Parent { get; private set; }
        public ulong GUID { get; private set; }

        private static ulong _guid;

        protected virtual void Awake()
        {
            GUID = ++_guid;
        }

#if UNITY_EDITOR
        protected virtual void Reset() {}
#endif

        protected virtual void Start() { }
        protected virtual void Update() {}
        protected virtual void OnEnable() {}
        protected virtual void OnDisable() {}
        protected virtual void OnDestroy() {}

        protected virtual void OnTransformChildrenChanged()
        {
            //Debug.Log($"OnTransformChildrenChanged {GetType().Name}");
        }

        protected virtual void OnTransformParentChanged()
        {
            //Debug.Log($"OnTransformParentChanged {GetType().Name}");

            //if (transform.parent != null)
            //{
            //    var parentView = transform.parent.GetComponent<UIObject>() as UIView;
            //    if (Parent != parentView)
            //    {
            //        Parent?.RemoveChild(this);

            //        if (parentView != null)
            //        {
            //            Parent = parentView;
            //            Parent.AddChild(this);
            //        }
            //    }
            //}
            //else
            //{
            //    Parent?.RemoveChild(this);
            //    Parent = null;
            //}
        }
    }
}
