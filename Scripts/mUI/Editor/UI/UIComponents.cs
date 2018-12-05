#if UNITY_EDITOR
using mFramework.UI;
using UnityEditor;

namespace mFramework.Editor.UI
{
    public static class UIComponents
    {
        private const string BasePath = "mFramework/mUI/Components/";

        [MenuItem(BasePath + "UIButton")]
        public static UIButton Button()
        {
            return Create<UIButton>();
        }

        private static T Create<T>() where T : UIComponent
        {
            var component = mUI.Component<T>();

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