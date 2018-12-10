using UnityEngine;

namespace mFramework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public abstract class UIView : UIObject
    {
#if UNITY_EDITOR
        #region EDITOR
        public abstract string SavePath { get; set; }
        public abstract string Namespace { get; set; }
        #endregion
#endif

        protected UIView()
        {
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void InitializeObject(
            UIObject obj,
            string goName,
            HideFlags goHideFlags,
            Vector3 localPos, 
            Vector3 localScale,
            Vector3 localEulerAngles,
            Vector2 anchorMax, Vector2 anchorMin,
            Vector2 offsetMax, Vector2 offsetMin,
            Vector2 pivot,
            Vector2 sizeDelta,
            Vector3 anchoredPosition3D)
        {
            var rectTransform = obj.RectTransform;
            obj.name = goName;
            obj.gameObject.hideFlags = goHideFlags;

            rectTransform.localScale = localScale;
            rectTransform.localPosition = localPos;
            rectTransform.localEulerAngles = localEulerAngles;
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchorMin = anchorMin;
            rectTransform.offsetMax = offsetMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.pivot = pivot;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition3D = anchoredPosition3D;
            rectTransform.ForceUpdateRectTransforms();
        }
    }
}