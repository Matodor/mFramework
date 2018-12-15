using System.Reflection;
using mFramework.Core;
using mFramework.Core.Interfaces;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [CustomPropertyDrawer(typeof(UIColor))]
    public class UIColorDrawer : PropertyDrawer
    {
        private bool _foldout;
        private Texture2D _colorPicker;

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _foldout 
                ? EditorGUIUtility.singleLineHeight * 2f + 150 + 15f + (EditorGUIUtility.singleLineHeight + 6f) * 5
                : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            var target = (object) property.serializedObject.targetObject;
            var path = property.propertyPath.Split('.');

            ICachedMemberInfo memberInfo = null;
            object value = null;

            for (var i = 0; i < path.Length; i++)
            {
                var cachedType = mCore.GetCachedType(target.GetType());
                memberInfo = cachedType.GetFieldOrProperty(path[i]);
                value = memberInfo.GetValue(target);

                if (i + 1 < path.Length)
                    target = value;
            }

            var pickedColor = (UIColor) value;
            EditorGUI.DrawRect(rect, (Color) pickedColor);
            _foldout = EditorGUI.Foldout(rect, _foldout, property.displayName);
            
            if (_foldout)
            {
                rect.x += EditorGUIUtility.labelWidth;
                rect.y += EditorGUIUtility.singleLineHeight + 6f;

                var colorType = (ColorType) EditorGUI.EnumPopup(rect, pickedColor.Type);

                if (colorType == ColorType.HSV && pickedColor.Type == ColorType.RGBA)
                    pickedColor = UIColor.ToHSV(pickedColor);
                else if (colorType == ColorType.RGBA && pickedColor.Type == ColorType.HSV)
                    pickedColor = UIColor.ToRGBA(pickedColor);

                var hsv = UIColor.ToHSV(pickedColor);
                const int width = 75;
                const int height = 75;

                if (_colorPicker == null)
                    _colorPicker = new Texture2D(width, height, TextureFormat.ARGB32, false);

                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < width; j++)
                    {
                        var tY = (float) i / height; 
                        var tX = (float) j / width;

                        var color = Color.Lerp(Color.black, Color.white, tY);
                        color = Color.Lerp(color, Color.HSVToRGB(hsv.N1, 1f, 1f), tX * tY);

                        _colorPicker.SetPixel(j, i, color);
                    }
                }

                _colorPicker.Apply();

                rect.height = 150;
                rect.width = 150;
                rect.y += EditorGUIUtility.singleLineHeight + 7.5f;
                GUI.DrawTexture(rect, _colorPicker);

                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 150;
                pickedColor.N1 = GUI.HorizontalSlider(rect, pickedColor.N1, 0f, 1f);

                rect.y += EditorGUIUtility.singleLineHeight + 6f;
                pickedColor.N2 = GUI.HorizontalSlider(rect, pickedColor.N2, 0f, 1f);

                rect.y += EditorGUIUtility.singleLineHeight + 6f;
                pickedColor.N3 = GUI.HorizontalSlider(rect, pickedColor.N3, 0f, 1f);

                rect.y += EditorGUIUtility.singleLineHeight + 6f;
                pickedColor.Alpha = GUI.HorizontalSlider(rect, pickedColor.Alpha, 0f, 1f);

                rect.y += EditorGUIUtility.singleLineHeight + 6f;
                GUI.TextField(rect, "asd");

                memberInfo.SetValue(target, pickedColor);
            }

            //EditorGUI.DrawRect(position, );
            //EditorGUILayout.LabelField($"fw={EditorGUIUtility.fieldWidth}");
            //EditorGUI.DrawRect(new Rect(position.x, position.y, 100, 100), Color.green);
            //GUI.DrawTexture();
            //GUI.DrawTexture(position, Texture2D.blackTexture);

            Debug.Log("asd = "  + GUI.changed);
        }
    }
}