﻿using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [CustomEditor(typeof(UIObject), true)]
    public class UIObjectEditor : UnityEditor.Editor
    {
        private UIObject _object;

        protected virtual void Awake()
        {
            _object = target as UIObject;
        }

        protected virtual void OnEnable()
        {
            _object = target as UIObject;
        }

        protected virtual void OnDisable()
        {
            _object = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LongField("GUID", (long)_object.GUID);
            EditorGUI.EndDisabledGroup();

            _object.IgnoreByViewWriter = EditorGUILayout.Toggle("Ignore by view writer", _object.IgnoreByViewWriter);

            if (GUILayout.Button("test"))
            {
                _object.RectTransform.ForceUpdateRectTransforms();
            }
        }
    }
}