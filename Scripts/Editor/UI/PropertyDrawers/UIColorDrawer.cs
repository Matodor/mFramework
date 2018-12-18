using System;
using System.Linq;
using System.Reflection;
using mFramework.Core;
using mFramework.Core.Extensions;
using mFramework.Core.Interfaces;
using mFramework.UI;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace mFramework.Editor.UI
{
    [CustomPropertyDrawer(typeof(UIColor))]
    public class UIColorDrawer : PropertyDrawer
    {
        private const int Height = 75;
        private const int Width = 75;

        private bool _foldout;
        private Texture2D _colorPicker;
        private bool _updateRequered;
        private ICachedMemberInfo _memberInfo;
        private ICachedMemberInfo _sliderModeMemberInfo;

        private object _target;

        //ColorPicker.SliderMode m_SliderMode

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _foldout 
                //? 16f * 2f + 150 + 15f + (16f + 6f) * 5
                ? 307f
                : 16f;
        }

        private void CacheColorPicker()
        {
            if (_sliderModeMemberInfo != null)
                return;

            var colorPickerType = typeof(EditorApplication)
                .Assembly.GetType("UnityEditor.ColorPicker");

            var cachedType = mCore.GetCachedType(colorPickerType);
            foreach (var cachedField in cachedType.CachedFields)
            {
                Debug.Log($"asd = {cachedField.FieldInfo.Name}");
            }

            _sliderModeMemberInfo = cachedType.CachedFields.First(f => f.FieldInfo.Name == "m_SliderMode");
        }

        private void CacheProperty(SerializedProperty property)
        {
            if (_memberInfo != null && _target != null)
                return;

            _target = property.serializedObject.targetObject;
            _memberInfo = null;
            var path = property.propertyPath.Split('.');

            for (var i = 0; i < path.Length; i++)
            {
                var cachedType = mCore.GetCachedType(_target.GetType());
                _memberInfo = cachedType.GetFieldOrProperty(path[i]);
                var value = _memberInfo.GetValue(_target);

                if (i + 1 < path.Length)
                    _target = value;
            }
        }

        private UIColor GetValue()
        {
            return (UIColor) _memberInfo.GetValue(_target);
        }

        private void SetValue(UIColor color)
        {
            _memberInfo.SetValue(_target, color);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CacheColorPicker();
            CacheProperty(property);

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            var indentLevel = EditorGUI.indentLevel;

            if (_colorPicker == null)
            {
                _colorPicker = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
                _updateRequered = true;
            }

            var rect = new Rect(
                 position.x,
                 position.y,
                 position.width,
                 EditorGUIUtility.singleLineHeight
             );
            
            var pickedColor = GetValue();
            _foldout = EditorGUI.Foldout(rect, _foldout, property.displayName);

            rect.width -= EditorGUIUtility.labelWidth;
            rect.x += EditorGUIUtility.labelWidth;
            EditorGUI.DrawRect(rect, (Color) pickedColor);
            
            if (_foldout)
            {
                EditorGUI.indentLevel = 0;
                rect.y += EditorGUIUtility.singleLineHeight + 6f;

                var colorType = (ColorType)EditorGUI.EnumPopup(rect, pickedColor.Type);
                if (colorType == ColorType.HSV && pickedColor.Type == ColorType.RGBA)
                {
                    pickedColor = UIColor.ToHSV(pickedColor);
                    _updateRequered = true;
                }
                else if (colorType == ColorType.RGBA && pickedColor.Type == ColorType.HSV)
                {
                    pickedColor = UIColor.ToRGBA(pickedColor);
                    _updateRequered = true;
                }
                
                rect.height = 150;
                rect.width = 150;
                rect.y += EditorGUIUtility.singleLineHeight + 7.5f;

                var hsv = UIColor.ToHSV(pickedColor);

                EditorGUI.DrawTextureTransparent(rect, _colorPicker, ScaleMode.StretchToFill);

                if (Event.current.type == EventType.MouseDown ||
                    Event.current.type == EventType.MouseDrag)
                {
                    var clickPos = Event.current.mousePosition;
                    if (rect.Contains(clickPos))
                    {
                        var tX = (clickPos.x - rect.x) / rect.width;
                        var tY = 1f - (clickPos.y - rect.y) / rect.height;

                        Debug.Log($"{clickPos} tx={tX} ty={tY}");
                    }
                }
                
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 150;
                EditorGUI.BeginChangeCheck();
                {
                    pickedColor.N1 = GUI.HorizontalSlider(rect, pickedColor.N1, 0f, 1f);

                    rect.y += EditorGUIUtility.singleLineHeight + 6f;
                    pickedColor.N2 = GUI.HorizontalSlider(rect, pickedColor.N2, 0f, 1f);

                    rect.y += EditorGUIUtility.singleLineHeight + 6f;
                    pickedColor.N3 = GUI.HorizontalSlider(rect, pickedColor.N3, 0f, 1f);

                    rect.y += EditorGUIUtility.singleLineHeight + 6f;
                    pickedColor.Alpha = GUI.HorizontalSlider(rect, pickedColor.Alpha, 0f, 1f);
                }
                _updateRequered |= EditorGUI.EndChangeCheck();

                EditorGUI.BeginChangeCheck();
                {
                    rect.y += EditorGUIUtility.singleLineHeight + 6f;
                    var hex = ColorUtility.ToHtmlStringRGB((Color) pickedColor);

                    EditorGUI.LabelField(rect.Shift(-100), "Hexadecimal");
                    hex = EditorGUI.TextField(rect, hex);

                    if (EditorGUI.EndChangeCheck())
                    {
                        _updateRequered = true;
                    }
                }

                if (_updateRequered)
                {
                    for (var i = 0; i < Height; i++)
                    {
                        for (var j = 0; j < Width; j++)
                        {
                            var tY = (float)i / Height;
                            var tX = (float)j / Width;

                            var color = Color.Lerp(Color.black, Color.white, tY);
                            color = Color.Lerp(color, Color.HSVToRGB(hsv.N1, 1f, 1f), tX * tY);

                            _colorPicker.SetPixel(j, i, color);
                        }
                    }

                    _colorPicker.Apply();
                    _updateRequered = false;
                    SetValue(pickedColor);
                }
            }

            EditorGUI.indentLevel = indentLevel;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }
    }
}