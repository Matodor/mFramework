#if UNITY_EDITOR
using mFramework.UI;
using UnityEditor;

namespace mFramework.Editor.UI
{
    public static class UIComponents
    {
		[MenuItem("GameObject/mUI/Components/UISprite", false, 0)]
        [MenuItem("mFramework/mUI/Components/UISprite")]
		public static UISprite Sprite()
        {
            return Create<UISprite>();
        }

		[MenuItem("GameObject/mUI/Components/UIButton", false, 0)]
        [MenuItem("mFramework/mUI/Components/UIButton")]
        public static UIButton Button()
        {
            return Create<UIButton>();
        }

        private static T Create<T>() where T : UIComponent
        {
            var component = mUI.Create<T>();

            if (Selection.activeGameObject != null)
            {
                GameObjectUtility.SetParentAndAlign(
                    component.gameObject, Selection.activeGameObject);
            }

            Undo.RegisterCreatedObjectUndo(component.gameObject,
                $"Create {component.name}");
            Selection.activeGameObject = component.gameObject;
            return component;
        }
    }
}
#endif