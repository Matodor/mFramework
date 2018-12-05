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

            EditorGUILayout.IntField("Child views", _view.ChildViews.Count, GUIStyle.none);
            EditorGUILayout.IntField("Child components", _view.ChildComponents.Count, GUIStyle.none);

            _show = EditorGUILayout.Foldout(_show, "Generate settings");
            if (_show)
            {
                _view.Namespace = EditorGUILayout.TextField(
                    "Namespace", _view.Namespace);
                _view.GeneratePath = EditorGUILayout.TextField(
                    "Path", _view.GeneratePath);

                if (GUILayout.Button("Save View"))
                {
                    ViewClassGenerator.View(
                        @namespace: _view.Namespace, 
                        className: _view.GetType().Name, 
                        savePath: _view.GeneratePath
                    );
                }
            }
        }
    }
}