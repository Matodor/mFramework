using UnityEngine;

namespace mFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UICamera))]
    public sealed class BaseView : UIView
    {
#if UNITY_EDITOR
        public override string SavePath { get; set; }
        public override string Namespace { get; set; }
#endif

        public UICamera UICamera { get; private set; }

        private BaseView()
        {
        }

        protected override void Awake()
        {
            base.Awake();

            gameObject.name = "[mUI] BaseView";
            gameObject.hideFlags =
                HideFlags.DontSaveInBuild |
                HideFlags.DontSaveInEditor | 
                HideFlags.NotEditable;

            UICamera = GetComponent<UICamera>();
        }

        protected override void Start()
        {
            base.Start();
            RectTransform.sizeDelta = UICamera.DeltaSize;
        }

        protected override void Reset()
        {
            base.Reset();
            RectTransform.sizeDelta = UICamera.DeltaSize;
        }

#if UNITY_EDITOR
        protected override void Update()
        {
            base.Update();
            RectTransform.sizeDelta = UICamera.DeltaSize;
        }
#endif

        // ReSharper disable once UnusedMember.Local
        private void OnApplicationQuit()
        {
            DestroyImmediate(gameObject);
        }
    }
}
