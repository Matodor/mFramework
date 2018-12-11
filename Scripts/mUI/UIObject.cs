using mFramework.Core.Common;
using mFramework.Core.Extensions;
using mFramework.Core.Interfaces;
using UnityEngine;

namespace mFramework.UI
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    public abstract class UIObject : MonoBehaviour, IInitializeWriter
#else
    public abstract class UIObject : MonoBehaviour
#endif
    {
#if UNITY_EDITOR
        #region EDITOR
        public bool IgnoreByViewWriter { get; set; }
        #endregion
#endif
        public ulong GUID { get; private set; }

        public RectTransform RectTransform { get; private set; }

        #region Events
        public event UIEventHandler<UIObject> Destroyed;
        #endregion

        private static ulong _guid;
        private int _sortingOrder;

        protected virtual void Awake()
        {
            GUID = ++_guid;
            RectTransform = gameObject.GetComponent<RectTransform>();
            RectTransform.sizeDelta = Vector2.one;
        }

#if UNITY_EDITOR
        // ReSharper disable once UnusedMember.Global
        protected virtual void Reset()
        {
        }
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
            //Debug.Log($"OnTransformChildrenChanged {GUID}");
        }

        // ReSharper disable once UnusedMember.Global
        protected virtual void OnTransformParentChanged()
        {
            //Debug.Log($"OnTransformParentChanged {GUID}");

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

#if UNITY_EDITOR
        public virtual void GenerateInitialize(IClassWriter _, string identifier)
        {
            _.Line($"{identifier}.gameObject.layer = {gameObject.layer};");
            _.Line($"{identifier}.gameObject.tag = \"{gameObject.tag}\";");
            _.Line($"{identifier}.gameObject.name = \"{gameObject.name}\";");
            _.Line($"{identifier}.gameObject.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;");

            var components = gameObject.GetComponents<Component>();
            for (var i = 0; i < components.Length; i++)
            {
                var componentWriter = new ComponentWriter(identifier, components[i], _);
            }
            
            _.DirectiveRegion("RectTransform");
            _.Line($"{identifier}.RectTransform.localScale = {RectTransform.localScale.StringCtor()};");
            _.Line($"{identifier}.RectTransform.localPosition = {RectTransform.localPosition.StringCtor()};");
            _.Line($"{identifier}.RectTransform.localRotation = {RectTransform.localRotation.StringCtor()};");
            _.Line($"{identifier}.RectTransform.anchorMax = {RectTransform.anchorMax.StringCtor()};");
            _.Line($"{identifier}.RectTransform.anchorMin = {RectTransform.anchorMin.StringCtor()};");
            _.Line($"{identifier}.RectTransform.offsetMax = {RectTransform.offsetMax.StringCtor()};");
            _.Line($"{identifier}.RectTransform.offsetMin = {RectTransform.offsetMin.StringCtor()};");
            _.Line($"{identifier}.RectTransform.pivot = {RectTransform.pivot.StringCtor()};");
            _.Line($"{identifier}.RectTransform.sizeDelta = {RectTransform.sizeDelta.StringCtor()};");
            _.Line($"{identifier}.RectTransform.anchoredPosition = {RectTransform.anchoredPosition.StringCtor()};");
            _.DirectiveEndRegion();
        }
#endif
    }
}
