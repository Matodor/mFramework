using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace mFramework.UI
{
    [DisallowMultipleComponent]
    public abstract class UIObject : MonoBehaviour
    {
        public ulong GUID { get; private set; }

        public RectTransform RectTransform { get; private set; }

        #region Events
        public event UIEventHandler<UIObject> Destroyed;
        #endregion

        private static ulong _guid;

        protected virtual void Awake()
        {
            GUID = ++_guid;
            RectTransform = gameObject.AddComponent<RectTransform>();
            RectTransform.sizeDelta = Vector2.one;
        }

#if UNITY_EDITOR
        // ReSharper disable once UnusedMember.Global
        protected virtual void Reset() {}
#endif

        // ReSharper disable once UnusedMember.Global
        protected virtual void Start() { }

        // ReSharper disable once UnusedMember.Global
        protected virtual void Update() { }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnEnable() { }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnDisable() { }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        // ReSharper disable once UnusedMember.Global
        public void Destroy()
        {
            Destroy(gameObject);
        }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnTransformChildrenChanged()
        {
            //Debug.Log($"OnTransformChildrenChanged {GetType().Name}");
        }

        // ReSharper disable once UnusedMember.Global
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
