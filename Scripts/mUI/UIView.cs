using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public abstract class UIView : UIObject
    {
        public IReadOnlyDictionary<ulong, UIView> ChildViews => _childViews;
        public IReadOnlyDictionary<ulong, UIComponent> ChildComponents => _childComponents;

        private readonly Dictionary<ulong, UIView> _childViews;
        private readonly Dictionary<ulong, UIComponent> _childComponents;

#if UNITY_EDITOR
        #region EDITOR

        public abstract string GeneratePath { get; set; }
        public abstract string Namespace { get; set; }

        #endregion
#endif

        protected UIView()
        {
            _childViews = new Dictionary<ulong, UIView>();
            _childComponents = new Dictionary<ulong, UIComponent>();
        }

        protected override void Awake()
        { 
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
         
        public void AddComponent(UIComponent child)
        {
            _childComponents.Add(child.GUID, child);
        }

        public void RemoveComponent(UIComponent child)
        {
            _childComponents.Remove(child.GUID);
        }
    }
}