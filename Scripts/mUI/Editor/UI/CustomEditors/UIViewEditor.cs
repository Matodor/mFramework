using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : UIObjectEditor
    {
        private UIView _view;
        private bool _show;

        protected override void Awake()
        {
            base.Awake();
            _view = target as UIView;
            _show = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _view = target as UIView;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _view = null;
        } 

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _show = EditorGUILayout.Foldout(_show, "View writer settings");
            if (_show)
            {
                _view.Namespace = EditorGUILayout.TextField(
                    "Namespace", _view.Namespace);
                _view.SavePath = EditorGUILayout.TextField(
                    "Path", _view.SavePath);

                if (GUILayout.Button($"Update '{_view.GetType().Name}' class"))
                {
                    ClassWriter.View(_view);
                }
            }
        }
    }
}