//using System;
//using mFramework.UI;
//using UnityEngine;
//using UnityEditor;
//using UnityEngine.Accessibility;

//namespace mFramework.Editor.UI
//{
//    [CustomPropertyDrawer(typeof(UIColor))]
//    internal class ColorPicker : PropertyDrawer
//    {
//        private static int s_Slider2Dhash = "Slider2D".GetHashCode();
//        [SerializeField] private bool m_ShowPresets = true;
//        [SerializeField] private bool m_IsOSColorPicker = false;
//        [SerializeField] private bool m_ShowAlpha = true;
//        private float m_RTextureG = -1f;
//        private float m_RTextureB = -1f;
//        private float m_GTextureR = -1f;
//        private float m_GTextureB = -1f;
//        private float m_BTextureR = -1f;
//        private float m_BTextureG = -1f;
//        private float m_HueTextureS = -1f;
//        private float m_HueTextureV = -1f;
//        private float m_SatTextureH = -1f;
//        private float m_SatTextureV = -1f;
//        private float m_ValTextureH = -1f;
//        private float m_ValTextureS = -1f;
//        [NonSerialized] private int m_TextureColorBoxMode = -1;
//        [SerializeField] private float m_LastConstant = -1f;
//        [SerializeField] private ColorPicker.ColorBoxMode m_ColorBoxMode = ColorPicker.ColorBoxMode.HSV;
//        [SerializeField] private ColorPicker.SliderMode m_SliderMode = ColorPicker.SliderMode.HSV;
//        private float m_OldAlpha = -1f;
//        [SerializeField] private int m_ModalUndoGroup = -1;
//        private float m_ExposureSliderMax = 10f;
//        private const string k_HeightPrefKey = "CPickerHeight";
//        private const string k_ShowPresetsPrefKey = "CPPresetsShow";
//        private const string k_SliderModePrefKey = "CPSliderMode";
//        private const string k_SliderModeHDRPrefKey = "CPSliderModeHDR";
//        private const float k_DefaultExposureSliderMax = 10f;
//        [SerializeField] private bool m_HDR;
//        [SerializeField] private ColorMutator m_Color;
//        [SerializeField] private Texture2D m_ColorSlider;
//        [SerializeField] private Color[] m_Colors;
//        private const int kHueRes = 64;
//        private const int kColorBoxSize = 32;
//        [SerializeField] private Texture2D m_ColorBox;
//        [SerializeField] private Texture2D m_RTexture;
//        [SerializeField] private Texture2D m_GTexture;
//        [SerializeField] private Texture2D m_BTexture;
//        [SerializeField] private Texture2D m_HueTexture;
//        [SerializeField] private Texture2D m_SatTexture;
//        [SerializeField] private Texture2D m_ValTexture;
//        [NonSerialized] private bool m_ColorSpaceBoxDirty;
//        [SerializeField] private Texture2D m_AlphaTexture;
//        [SerializeField] private GUIView m_DelegateView;
//        private Action<Color> m_ColorChangedCallback;
//        private float m_FloatSliderMaxOnMouseDown;
//        private bool m_DraggingFloatSlider;
//        private PresetLibraryEditor<ColorPresetLibrary> m_ColorLibraryEditor;
//        private PresetLibraryEditorState m_ColorLibraryEditorState;
//        private static ColorPicker s_Instance;
//        private static Texture2D s_LeftGradientTexture;
//        private static Texture2D s_RightGradientTexture;

//        public static string presetsEditorPrefID
//        {
//            get { return "Color"; }
//        }

//        public static Color color
//        {
//            get { return ColorPicker.instance.m_Color.exposureAdjustedColor; }
//            set
//            {
//                ColorPicker.instance.SetColor(value);
//                ColorPicker.instance.Repaint();
//            }
//        }

//        public static bool visible
//        {
//            get { return (UnityEngine.Object) ColorPicker.s_Instance != (UnityEngine.Object) null; }
//        }

//        public static ColorPicker instance
//        {
//            get
//            {
//                if (!(bool) ((UnityEngine.Object) ColorPicker.s_Instance))
//                {
//                    UnityEngine.Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(ColorPicker));
//                    if (objectsOfTypeAll != null && objectsOfTypeAll.Length > 0)
//                        ColorPicker.s_Instance = (ColorPicker) objectsOfTypeAll[0];
//                    if (!(bool) ((UnityEngine.Object) ColorPicker.s_Instance))
//                    {
//                        ColorPicker.s_Instance = ScriptableObject.CreateInstance<ColorPicker>();
//                        ColorPicker.s_Instance.wantsMouseMove = true;
//                    }
//                }

//                return ColorPicker.s_Instance;
//            }
//        }

//        public static int originalKeyboardControl { get; private set; }

//        private static void swap(ref float f1, ref float f2)
//        {
//            float num = f1;
//            f1 = f2;
//            f2 = num;
//        }

//        private Vector2 Slider2D(Rect rect, Vector2 value, Vector2 maxvalue, Vector2 minvalue, GUIStyle thumbStyle)
//        {
//            int controlId = GUIUtility.GetControlID(ColorPicker.s_Slider2Dhash, FocusType.Passive);
//            if ((double) maxvalue.x < (double) minvalue.x)
//                ColorPicker.swap(ref maxvalue.x, ref minvalue.x);
//            if ((double) maxvalue.y < (double) minvalue.y)
//                ColorPicker.swap(ref maxvalue.y, ref minvalue.y);
//            Event current = Event.current;
//            switch (current.GetTypeForControl(controlId))
//            {
//                case EventType.MouseDown:
//                    if (rect.Contains(current.mousePosition))
//                    {
//                        GUIUtility.hotControl = controlId;
//                        GUIUtility.keyboardControl = 0;
//                        value.x = (float) (((double) current.mousePosition.x - (double) rect.x) / (double) rect.width *
//                                           ((double) maxvalue.x - (double) minvalue.x));
//                        value.y = (float) (((double) current.mousePosition.y - (double) rect.y) / (double) rect.height *
//                                           ((double) maxvalue.y - (double) minvalue.y));
//                        GUI.changed = true;
//                        Event.current.Use();
//                        break;
//                    }

//                    break;
//                case EventType.MouseUp:
//                    if (GUIUtility.hotControl == controlId)
//                    {
//                        GUIUtility.hotControl = 0;
//                        current.Use();
//                        break;
//                    }

//                    break;
//                case EventType.MouseDrag:
//                    if (GUIUtility.hotControl == controlId)
//                    {
//                        value.x = (float) (((double) current.mousePosition.x - (double) rect.x) / (double) rect.width *
//                                           ((double) maxvalue.x - (double) minvalue.x));
//                        value.y = (float) (((double) current.mousePosition.y - (double) rect.y) / (double) rect.height *
//                                           ((double) maxvalue.y - (double) minvalue.y));
//                        value.x = Mathf.Clamp(value.x, minvalue.x, maxvalue.x);
//                        value.y = Mathf.Clamp(value.y, minvalue.y, maxvalue.y);
//                        GUI.changed = true;
//                        Event.current.Use();
//                        break;
//                    }

//                    break;
//                case EventType.Repaint:
//                    Color color = GUI.color;
//                    GUI.color = (double) VisionUtility.ComputePerceivedLuminance(ColorPicker.color) <= 0.5
//                        ? ColorPicker.Styles.lowLuminanceContentColor
//                        : ColorPicker.Styles.highLuminanceContentColor;
//                    Rect position = new Rect()
//                    {
//                        size = thumbStyle.CalcSize(GUIContent.none),
//                        center = new Vector2(value.x / (maxvalue.x - minvalue.x) * rect.width + rect.x,
//                            value.y / (maxvalue.y - minvalue.y) * rect.height + rect.y)
//                    };
//                    thumbStyle.Draw(position, GUIContent.none, controlId);
//                    GUI.color = color;
//                    break;
//            }

//            return value;
//        }

//        private void RGBSliders()
//        {
//            float channelNormalized1 = this.m_Color.GetColorChannelNormalized(RgbaChannel.R);
//            float channelNormalized2 = this.m_Color.GetColorChannelNormalized(RgbaChannel.G);
//            float channelNormalized3 = this.m_Color.GetColorChannelNormalized(RgbaChannel.B);
//            this.m_RTexture = this.Update1DSlider(this.m_RTexture, 32, channelNormalized2, channelNormalized3,
//                ref this.m_RTextureG, ref this.m_RTextureB, 0, false);
//            this.m_GTexture = this.Update1DSlider(this.m_GTexture, 32, channelNormalized1, channelNormalized3,
//                ref this.m_GTextureR, ref this.m_GTextureB, 1, false);
//            this.m_BTexture = this.Update1DSlider(this.m_BTexture, 32, channelNormalized1, channelNormalized2,
//                ref this.m_BTextureR, ref this.m_BTextureG, 2, false);
//            this.RGBSlider("R", RgbaChannel.R, this.m_RTexture);
//            GUILayout.Space(6f);
//            this.RGBSlider("G", RgbaChannel.G, this.m_GTexture);
//            GUILayout.Space(6f);
//            this.RGBSlider("B", RgbaChannel.B, this.m_BTexture);
//            GUILayout.Space(6f);
//        }

//        private void RGBSlider(string label, RgbaChannel channel, Texture2D sliderBackground)
//        {
//            switch (this.m_SliderMode)
//            {
//                case ColorPicker.SliderMode.RGB:
//                    float colorChannel = (float) this.m_Color.GetColorChannel(channel);
//                    EditorGUI.BeginChangeCheck();
//                    float num1 = EditorGUILayout.SliderWithTexture(GUIContent.Temp(label), colorChannel, 0.0f,
//                        (float) byte.MaxValue, EditorGUI.kIntFieldFormatString, 0.0f, (float) byte.MaxValue,
//                        sliderBackground);
//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        this.m_Color.SetColorChannel(channel, num1 / (float) byte.MaxValue);
//                        this.OnColorChanged(true);
//                    }

//                    this.m_DraggingFloatSlider = false;
//                    break;
//                case ColorPicker.SliderMode.RGBFloat:
//                    float colorChannelHdr = this.m_Color.GetColorChannelHdr(channel);
//                    float maxColorComponent = (Color) this.m_Color.color.maxColorComponent;
//                    EventType type = Event.current.type;
//                    float num2 = !this.m_HDR || (double) this.m_Color.exposureAdjustedColor.maxColorComponent <= 1.0
//                        ? 1f
//                        : this.m_Color.exposureAdjustedColor.maxColorComponent / maxColorComponent;
//                    float textFieldMax = !this.m_HDR ? 1f : float.MaxValue;
//                    EditorGUI.BeginChangeCheck();
//                    float num3 = EditorGUILayout.SliderWithTexture(GUIContent.Temp(label), colorChannelHdr, 0.0f,
//                        !this.m_DraggingFloatSlider ? num2 : this.m_FloatSliderMaxOnMouseDown,
//                        EditorGUI.kFloatFieldFormatString, 0.0f, textFieldMax, sliderBackground);
//                    switch (type)
//                    {
//                        case EventType.MouseDown:
//                            this.m_FloatSliderMaxOnMouseDown = num2;
//                            this.m_DraggingFloatSlider = true;
//                            break;
//                        case EventType.MouseUp:
//                            this.m_DraggingFloatSlider = false;
//                            break;
//                    }

//                    if (!EditorGUI.EndChangeCheck())
//                        break;
//                    this.m_Color.SetColorChannelHdr(channel, num3);
//                    this.OnColorChanged(true);
//                    break;
//            }
//        }

//        private Texture2D Update1DSlider(Texture2D tex, int xSize, float const1, float const2, ref float oldConst1,
//            ref float oldConst2, int idx, bool hsvSpace)
//        {
//            if (!(bool) ((UnityEngine.Object) tex) || (double) const1 != (double) oldConst1 ||
//                (double) const2 != (double) oldConst2)
//            {
//                if (!(bool) ((UnityEngine.Object) tex))
//                    tex = ColorPicker.MakeTexture(xSize, 2);
//                Color[] colorArray = new Color[xSize * 2];
//                Color topLeftColor = Color.black;
//                Color rightGradient = Color.black;
//                switch (idx)
//                {
//                    case 0:
//                        topLeftColor = new Color(0.0f, const1, const2, 1f);
//                        rightGradient = new Color(1f, 0.0f, 0.0f, 0.0f);
//                        break;
//                    case 1:
//                        topLeftColor = new Color(const1, 0.0f, const2, 1f);
//                        rightGradient = new Color(0.0f, 1f, 0.0f, 0.0f);
//                        break;
//                    case 2:
//                        topLeftColor = new Color(const1, const2, 0.0f, 1f);
//                        rightGradient = new Color(0.0f, 0.0f, 1f, 0.0f);
//                        break;
//                    case 3:
//                        topLeftColor = (Color) this.m_Color.color;
//                        topLeftColor.a = 0.0f;
//                        rightGradient = new Color(0.0f, 0.0f, 0.0f, 1f);
//                        break;
//                }

//                ColorPicker.FillArea(xSize, 2, colorArray, topLeftColor, rightGradient,
//                    new Color(0.0f, 0.0f, 0.0f, 0.0f), !hsvSpace && this.m_HDR);
//                if (hsvSpace)
//                    ColorPicker.HSVToRGBArray(colorArray, this.m_HDR);
//                oldConst1 = const1;
//                oldConst2 = const2;
//                tex.SetPixels(colorArray);
//                tex.Apply();
//            }

//            return tex;
//        }

//        private void HSVSliders()
//        {
//            float colorChannel1 = this.m_Color.GetColorChannel(HsvChannel.H);
//            float colorChannel2 = this.m_Color.GetColorChannel(HsvChannel.S);
//            float colorChannel3 = this.m_Color.GetColorChannel(HsvChannel.V);
//            this.m_HueTexture = this.Update1DSlider(this.m_HueTexture, 64, 1f, 1f, ref this.m_HueTextureS,
//                ref this.m_HueTextureV, 0, true);
//            this.m_SatTexture = this.Update1DSlider(this.m_SatTexture, 32, colorChannel1,
//                Mathf.Max(colorChannel3, 0.2f), ref this.m_SatTextureH, ref this.m_SatTextureV, 1, true);
//            this.m_ValTexture = this.Update1DSlider(this.m_ValTexture, 32, colorChannel1, colorChannel2,
//                ref this.m_ValTextureH, ref this.m_ValTextureS, 2, true);
//            EditorGUI.BeginChangeCheck();
//            float num1 = EditorGUILayout.SliderWithTexture(GUIContent.Temp("H"), colorChannel1 * 360f, 0.0f, 360f,
//                EditorGUI.kIntFieldFormatString, this.m_HueTexture);
//            if (EditorGUI.EndChangeCheck())
//            {
//                this.m_Color.SetColorChannel(HsvChannel.H, num1 / 360f);
//                this.OnColorChanged(true);
//            }

//            GUILayout.Space(6f);
//            EditorGUI.BeginChangeCheck();
//            float num2 = EditorGUILayout.SliderWithTexture(GUIContent.Temp("S"), colorChannel2 * 100f, 0.0f, 100f,
//                EditorGUI.kIntFieldFormatString, this.m_SatTexture);
//            if (EditorGUI.EndChangeCheck())
//            {
//                this.m_Color.SetColorChannel(HsvChannel.S, num2 / 100f);
//                this.OnColorChanged(true);
//            }

//            GUILayout.Space(6f);
//            EditorGUI.BeginChangeCheck();
//            float num3 = EditorGUILayout.SliderWithTexture(GUIContent.Temp("V"), colorChannel3 * 100f, 0.0f, 100f,
//                EditorGUI.kIntFieldFormatString, this.m_ValTexture);
//            if (EditorGUI.EndChangeCheck())
//            {
//                this.m_Color.SetColorChannel(HsvChannel.V, num3 / 100f);
//                this.OnColorChanged(true);
//            }

//            GUILayout.Space(6f);
//        }

//        private static void FillArea(int xSize, int ySize, Color[] retval, Color topLeftColor, Color rightGradient,
//            Color downGradient, bool convertToGamma)
//        {
//            Color color1 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
//            Color color2 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
//            if (xSize > 1)
//                color1 = rightGradient / (float) (xSize - 1);
//            if (ySize > 1)
//                color2 = downGradient / (float) (ySize - 1);
//            Color color3 = topLeftColor;
//            int num = 0;
//            for (int index1 = 0; index1 < ySize; ++index1)
//            {
//                Color color4 = color3;
//                for (int index2 = 0; index2 < xSize; ++index2)
//                {
//                    retval[num++] = !convertToGamma ? color4 : color4.gamma;
//                    color4 += color1;
//                }

//                color3 += color2;
//            }
//        }

//        private static void HSVToRGBArray(Color[] colors, bool convertToGamma)
//        {
//            int length = colors.Length;
//            for (int index = 0; index < length; ++index)
//            {
//                Color color = colors[index];
//                Color rgb = Color.HSVToRGB(color.r, color.g, color.b);
//                rgb.a = color.a;
//                colors[index] = !convertToGamma ? rgb : rgb.gamma;
//            }
//        }

//        public static Texture2D MakeTexture(int width, int height)
//        {
//            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
//            texture2D.hideFlags = HideFlags.HideAndDontSave;
//            texture2D.wrapMode = TextureWrapMode.Clamp;
//            return texture2D;
//        }

//        private void DrawColorSpaceBox(Rect colorBoxRect, float constantValue)
//        {
//            if (Event.current.type != EventType.Repaint)
//                return;
//            if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode) this.m_TextureColorBoxMode)
//            {
//                int width = 32;
//                int height = 32;
//                if ((UnityEngine.Object) this.m_ColorBox == (UnityEngine.Object) null)
//                    this.m_ColorBox = ColorPicker.MakeTexture(width, height);
//                if (this.m_ColorBox.width != width || this.m_ColorBox.height != height)
//                    this.m_ColorBox.Resize(width, height);
//            }

//            if (this.m_ColorBoxMode != (ColorPicker.ColorBoxMode) this.m_TextureColorBoxMode ||
//                (double) this.m_LastConstant != (double) constantValue || this.m_ColorSpaceBoxDirty)
//            {
//                this.m_Colors = this.m_ColorBox.GetPixels(0);
//                ColorPicker.FillArea(this.m_ColorBox.width, this.m_ColorBox.height, this.m_Colors,
//                    new Color(this.m_Color.GetColorChannel(HsvChannel.H), 0.0f, 0.0f, 1f),
//                    new Color(0.0f, 1f, 0.0f, 0.0f), new Color(0.0f, 0.0f, 1f, 0.0f), false);
//                ColorPicker.HSVToRGBArray(this.m_Colors, this.m_HDR);
//                this.m_ColorBox.SetPixels(this.m_Colors, 0);
//                this.m_ColorBox.Apply(true);
//                this.m_LastConstant = constantValue;
//                this.m_TextureColorBoxMode = (int) this.m_ColorBoxMode;
//            }

//            Graphics.DrawTexture(colorBoxRect, (Texture) this.m_ColorBox,
//                new Rect(0.5f / (float) this.m_ColorBox.width, 0.5f / (float) this.m_ColorBox.height,
//                    (float) (1.0 - 1.0 / (double) this.m_ColorBox.width),
//                    (float) (1.0 - 1.0 / (double) this.m_ColorBox.height)), 0, 0, 0, 0, Color.grey);
//        }

//        public string currentPresetLibrary
//        {
//            get
//            {
//                this.InitializePresetsLibraryIfNeeded();
//                return this.m_ColorLibraryEditor.currentLibraryWithoutExtension;
//            }
//            set
//            {
//                this.InitializePresetsLibraryIfNeeded();
//                this.m_ColorLibraryEditor.currentLibraryWithoutExtension = value;
//            }
//        }

//        private void InitializePresetsLibraryIfNeeded()
//        {
//            if (this.m_ColorLibraryEditorState == null)
//            {
//                this.m_ColorLibraryEditorState = new PresetLibraryEditorState(ColorPicker.presetsEditorPrefID);
//                this.m_ColorLibraryEditorState.TransferEditorPrefsState(true);
//            }

//            if (this.m_ColorLibraryEditor != null)
//                return;
//            this.m_ColorLibraryEditor = new PresetLibraryEditor<ColorPresetLibrary>(
//                new ScriptableObjectSaveLoadHelper<ColorPresetLibrary>("colors", SaveType.Text),
//                this.m_ColorLibraryEditorState, new Action<int, object>(this.OnClickedPresetSwatch));
//            this.m_ColorLibraryEditor.previewAspect = 1f;
//            this.m_ColorLibraryEditor.minMaxPreviewHeight = new Vector2(14f, 14f);
//            this.m_ColorLibraryEditor.settingsMenuRightMargin = 2f;
//            this.m_ColorLibraryEditor.useOnePixelOverlappedGrid = true;
//            this.m_ColorLibraryEditor.alwaysShowScrollAreaHorizontalLines = false;
//            this.m_ColorLibraryEditor.marginsForGrid = new RectOffset(0, 0, 0, 0);
//            this.m_ColorLibraryEditor.marginsForList = new RectOffset(0, 5, 2, 2);
//            this.m_ColorLibraryEditor.InitializeGrid(
//                233f - (float) (ColorPicker.Styles.background.padding.left +
//                                ColorPicker.Styles.background.padding.right));
//        }

//        private void OnClickedPresetSwatch(int clickCount, object presetObject)
//        {
//            Color color = (Color) presetObject;
//            if (!this.m_HDR && (double) color.maxColorComponent > 1.0)
//                color = (Color) new ColorMutator(color).color;
//            this.SetColor(color);
//        }

//        private Color GetGUIColor(Color color)
//        {
//            return !this.m_HDR ? color : color.gamma;
//        }

//        private void DoColorSwatchAndEyedropper()
//        {
//            GUILayout.BeginHorizontal();
//            if (GUILayout.Button(ColorPicker.Styles.eyeDropper, GUIStyle.none, GUILayout.Width(40f),
//                GUILayout.ExpandWidth(false)))
//            {
//                GUIUtility.keyboardControl = 0;
//                EyeDropper.Start((GUIView) this.m_Parent);
//                this.m_ColorBoxMode = ColorPicker.ColorBoxMode.EyeDropper;
//                GUIUtility.ExitGUI();
//            }

//            Color exposureAdjustedColor = this.m_Color.exposureAdjustedColor;
//            Rect rect = GUILayoutUtility.GetRect((GUIContent) ColorPicker.Styles.currentColorSwatchFill,
//                ColorPicker.Styles.currentColorSwatch, new GUILayoutOption[1]
//                {
//                    GUILayout.ExpandWidth(true)
//                });
//            Vector2 vector2 =
//                ColorPicker.Styles.currentColorSwatch.CalcSize((GUIContent) ColorPicker.Styles.currentColorSwatchFill);
//            Rect position = new Rect()
//            {
//                size = vector2,
//                y = rect.y,
//                x = rect.xMax - vector2.x
//            };
//            Color backgroundColor = GUI.backgroundColor;
//            Color contentColor = GUI.contentColor;
//            int controlId = GUIUtility.GetControlID(FocusType.Passive);
//            if (Event.current.type == EventType.Repaint)
//            {
//                GUI.backgroundColor = (double) this.m_Color.exposureAdjustedColor.a != 1.0 ? Color.white : Color.clear;
//                GUI.contentColor = this.GetGUIColor(this.m_Color.exposureAdjustedColor);
//                ColorPicker.Styles.currentColorSwatch.Draw(position,
//                    (GUIContent) ColorPicker.Styles.currentColorSwatchFill, controlId);
//            }

//            position.x -= position.width;
//            GUI.backgroundColor = (double) this.m_Color.originalColor.a != 1.0 ? Color.white : Color.clear;
//            GUI.contentColor = this.GetGUIColor(this.m_Color.originalColor);
//            if (GUI.Button(position, (GUIContent) ColorPicker.Styles.originalColorSwatchFill,
//                ColorPicker.Styles.originalColorSwatch))
//            {
//                this.m_Color.Reset();
//                Event.current.Use();
//                this.OnColorChanged(true);
//            }

//            GUI.backgroundColor = backgroundColor;
//            GUI.contentColor = contentColor;
//            GUILayout.EndHorizontal();
//        }

//        private void DoColorSpaceGUI()
//        {
//            GUIStyle background =
//                !this.m_HDR ? ColorPicker.Styles.hueDialBackground : ColorPicker.Styles.hueDialBackgroundHDR;
//            Vector2 vector2_1 = background.CalcSize(GUIContent.none);
//            Rect rect1 = GUILayoutUtility.GetRect(vector2_1.x, vector2_1.y);
//            switch (this.m_ColorBoxMode)
//            {
//                case ColorPicker.ColorBoxMode.HSV:
//                    rect1.x += (float) (((double) rect1.width - (double) rect1.height) * 0.5);
//                    rect1.width = rect1.height;
//                    float colorChannel = this.m_Color.GetColorChannel(HsvChannel.H);
//                    Color contentColor = GUI.contentColor;
//                    GUI.contentColor = this.GetGUIColor(Color.HSVToRGB(colorChannel, 1f, 1f));
//                    EditorGUI.BeginChangeCheck();
//                    float t = EditorGUI.AngularDial(rect1, GUIContent.none, colorChannel * 360f,
//                        (GUIContent) ColorPicker.Styles.hueDialThumbFill.image, background,
//                        ColorPicker.Styles.hueDialThumb);
//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        this.m_Color.SetColorChannel(HsvChannel.H, Mathf.Repeat(t, 360f) / 360f);
//                        this.OnColorChanged(true);
//                    }

//                    GUI.contentColor = contentColor;
//                    int num = Mathf.FloorToInt(Mathf.Sqrt(2f) *
//                                               (rect1.width * 0.5f - ColorPicker.Styles.hueDialThumbSize));
//                    if ((num & 1) == 1)
//                        ++num;
//                    Rect rect2 = new Rect()
//                    {
//                        size = Vector2.one * (float) num,
//                        center = rect1.center
//                    };
//                    Rect rect3 = ColorPicker.Styles.colorBoxPadding.Remove(rect2);
//                    this.DrawColorSpaceBox(rect3, this.m_Color.GetColorChannel(HsvChannel.H));
//                    EditorGUI.BeginChangeCheck();
//                    Vector2 vector2_2 = new Vector2(this.m_Color.GetColorChannel(HsvChannel.S),
//                        1f - this.m_Color.GetColorChannel(HsvChannel.V));
//                    vector2_2 = this.Slider2D(rect3, vector2_2, Vector2.zero, Vector2.one,
//                        ColorPicker.Styles.colorBoxThumb);
//                    if (!EditorGUI.EndChangeCheck())
//                        break;
//                    this.m_Color.SetColorChannel(HsvChannel.S, vector2_2.x);
//                    this.m_Color.SetColorChannel(HsvChannel.V, 1f - vector2_2.y);
//                    this.OnColorChanged(true);
//                    break;
//                case ColorPicker.ColorBoxMode.EyeDropper:
//                    EyeDropper.DrawPreview(rect1);
//                    break;
//            }
//        }

//        private void DoColorSliders(float availableWidth)
//        {
//            float labelWidth = EditorGUIUtility.labelWidth;
//            float fieldWidth = EditorGUIUtility.fieldWidth;
//            EditorGUIUtility.labelWidth = availableWidth - 72f;
//            EditorGUIUtility.fieldWidth = 72f;
//            this.m_SliderMode = (ColorPicker.SliderMode) EditorGUILayout.IntPopup(GUIContent.Temp(" "),
//                (int) this.m_SliderMode, ColorPicker.Styles.sliderModeLabels, ColorPicker.Styles.sliderModeValues,
//                new GUILayoutOption[0]);
//            GUILayout.Space(6f);
//            EditorGUIUtility.labelWidth = labelWidth;
//            EditorGUIUtility.fieldWidth = fieldWidth;
//            EditorGUIUtility.labelWidth = 14f;
//            if (this.m_SliderMode == ColorPicker.SliderMode.HSV)
//                this.HSVSliders();
//            else
//                this.RGBSliders();
//            if (this.m_ShowAlpha)
//            {
//                this.m_AlphaTexture = this.Update1DSlider(this.m_AlphaTexture, 32, 0.0f, 0.0f, ref this.m_OldAlpha,
//                    ref this.m_OldAlpha, 3, false);
//                float sliderMax = 1f;
//                string fieldFormatString = EditorGUI.kFloatFieldFormatString;
//                switch (this.m_SliderMode)
//                {
//                    case ColorPicker.SliderMode.RGB:
//                        sliderMax = (float) byte.MaxValue;
//                        fieldFormatString = EditorGUI.kIntFieldFormatString;
//                        break;
//                    case ColorPicker.SliderMode.HSV:
//                        sliderMax = 100f;
//                        fieldFormatString = EditorGUI.kIntFieldFormatString;
//                        break;
//                }

//                Rect controlRect = EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]);
//                if (Event.current.type == EventType.Repaint)
//                {
//                    Rect rect = controlRect;
//                    rect.xMin += EditorGUIUtility.labelWidth;
//                    rect.xMax -= EditorGUIUtility.fieldWidth + 5f;
//                    Rect screenRect = ColorPicker.Styles.sliderBackground.padding.Remove(rect);
//                    Rect sourceRect = new Rect()
//                    {
//                        x = 0.0f,
//                        y = 0.0f,
//                        width = screenRect.width / screenRect.height,
//                        height = 1f
//                    };
//                    Graphics.DrawTexture(screenRect, (Texture) ColorPicker.Styles.alphaSliderCheckerBackground,
//                        sourceRect, 0, 0, 0, 0);
//                }

//                EditorGUI.BeginChangeCheck();
//                float sliderValue = this.m_Color.GetColorChannelNormalized(RgbaChannel.A) * sliderMax;
//                float num = EditorGUI.SliderWithTexture(controlRect, GUIContent.Temp("A"), sliderValue, 0.0f, sliderMax,
//                    fieldFormatString, this.m_AlphaTexture, new GUILayoutOption[0]);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    this.m_Color.SetColorChannel(RgbaChannel.A, num / sliderMax);
//                    this.OnColorChanged(true);
//                }

//                GUILayout.Space(6f);
//            }

//            EditorGUIUtility.labelWidth = labelWidth;
//        }

//        private void DoHexField(float availableWidth)
//        {
//            float labelWidth = EditorGUIUtility.labelWidth;
//            float fieldWidth = EditorGUIUtility.fieldWidth;
//            EditorGUIUtility.labelWidth = availableWidth - 72f;
//            EditorGUIUtility.fieldWidth = 72f;
//            EditorGUI.BeginChangeCheck();
//            Color32 color32 = EditorGUILayout.HexColorTextField(ColorPicker.Styles.hexLabel, this.m_Color.color, false);
//            if (EditorGUI.EndChangeCheck())
//            {
//                this.m_Color.SetColorChannel(RgbaChannel.R, color32.r);
//                this.m_Color.SetColorChannel(RgbaChannel.G, color32.g);
//                this.m_Color.SetColorChannel(RgbaChannel.B, color32.b);
//                this.OnColorChanged(true);
//            }

//            EditorGUIUtility.labelWidth = labelWidth;
//            EditorGUIUtility.fieldWidth = fieldWidth;
//        }

//        private void DoExposureSlider()
//        {
//            float labelWidth = EditorGUIUtility.labelWidth;
//            EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(ColorPicker.Styles.exposureValue).x +
//                                          (float) EditorStyles.label.margin.right;
//            Rect rect = GUILayoutUtility.GetRect(0.0f, EditorGUIUtility.singleLineHeight);
//            EditorGUI.BeginChangeCheck();
//            float num = EditorGUI.Slider(rect, ColorPicker.Styles.exposureValue, this.m_Color.exposureValue,
//                -this.m_ExposureSliderMax, this.m_ExposureSliderMax, float.MinValue, float.MaxValue);
//            if (EditorGUI.EndChangeCheck())
//            {
//                this.m_Color.exposureValue = num;
//                this.OnColorChanged(true);
//            }

//            EditorGUIUtility.labelWidth = labelWidth;
//        }

//        private void DoExposureSwatches()
//        {
//            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, ColorPicker.Styles.exposureSwatch,
//                new GUILayoutOption[1]
//                {
//                    GUILayout.ExpandWidth(true)
//                });
//            int num1 = 5;
//            Rect position = new Rect()
//            {
//                x = rect.x + (float) (((double) rect.width -
//                                       (double) num1 * (double) ColorPicker.Styles.exposureSwatch.fixedWidth) * 0.5),
//                y = rect.y,
//                width = ColorPicker.Styles.exposureSwatch.fixedWidth,
//                height = ColorPicker.Styles.exposureSwatch.fixedHeight
//            };
//            Color backgroundColor = GUI.backgroundColor;
//            Color contentColor = GUI.contentColor;
//            for (int index = 0; index < num1; ++index)
//            {
//                int num2 = index - num1 / 2;
//                Color gamma = (this.m_Color.exposureAdjustedColor * Mathf.Pow(2f, (float) num2)).gamma;
//                gamma.a = 1f;
//                GUI.backgroundColor = gamma;
//                GUI.contentColor = (double) VisionUtility.ComputePerceivedLuminance(gamma) >= 0.5
//                    ? ColorPicker.Styles.highLuminanceContentColor
//                    : ColorPicker.Styles.lowLuminanceContentColor;
//                if (GUI.Button(position,
//                    GUIContent.Temp(num2 != 0
//                        ? (num2 >= 0 ? string.Format("+{0}", (object) num2) : num2.ToString())
//                        : (string) null), ColorPicker.Styles.exposureSwatch))
//                {
//                    this.m_Color.exposureValue = Mathf.Clamp(this.m_Color.exposureValue + (float) num2,
//                        -this.m_ExposureSliderMax, this.m_ExposureSliderMax);
//                    this.OnColorChanged(true);
//                }

//                if (num2 == 0 && Event.current.type == EventType.Repaint)
//                {
//                    GUI.backgroundColor = GUI.contentColor;
//                    ColorPicker.Styles.selectedExposureSwatchStroke.Draw(position, false, false, false, false);
//                }

//                position.x += position.width;
//            }

//            GUI.backgroundColor = backgroundColor;
//            GUI.contentColor = contentColor;
//        }

//        private void DoPresetsGUI()
//        {
//            Rect rect = GUILayoutUtility.GetRect(ColorPicker.Styles.presetsToggle, EditorStyles.foldout);
//            rect.xMax -= 17f;
//            this.m_ShowPresets = EditorGUI.Foldout(rect, this.m_ShowPresets, ColorPicker.Styles.presetsToggle, true);
//            if (!this.m_ShowPresets)
//                return;
//            GUILayout.Space((float) -((double) EditorGUIUtility.singleLineHeight +
//                                      (double) EditorGUIUtility.standardVerticalSpacing));
//            this.m_ColorLibraryEditor.OnGUI(
//                GUILayoutUtility.GetRect(0.0f, Mathf.Clamp(this.m_ColorLibraryEditor.contentHeight, 20f, 250f)),
//                (object) ColorPicker.color);
//        }

//        private void OnGUI()
//        {
//            this.InitializePresetsLibraryIfNeeded();
//            if (Event.current.type == EventType.ExecuteCommand)
//            {
//                switch (Event.current.commandName)
//                {
//                    case "EyeDropperUpdate":
//                        this.Repaint();
//                        break;
//                    case "EyeDropperClicked":
//                        this.m_ColorBoxMode = ColorPicker.ColorBoxMode.HSV;
//                        Color color = EyeDropper.GetLastPickedColor();
//                        if (this.m_HDR)
//                            color = color.linear;
//                        this.m_Color.SetColorChannelHdr(RgbaChannel.R, color.r);
//                        this.m_Color.SetColorChannelHdr(RgbaChannel.G, color.g);
//                        this.m_Color.SetColorChannelHdr(RgbaChannel.B, color.b);
//                        this.m_Color.SetColorChannelHdr(RgbaChannel.A, color.a);
//                        this.OnColorChanged(true);
//                        break;
//                    case "EyeDropperCancelled":
//                        this.OnEyedropperCancelled();
//                        break;
//                }
//            }

//            Rect rect = EditorGUILayout.BeginVertical(ColorPicker.Styles.background, new GUILayoutOption[0]);
//            float width = EditorGUILayout.GetControlRect(false, 1f, EditorStyles.numberField, new GUILayoutOption[0])
//                .width;
//            EditorGUIUtility.labelWidth = width - 45f;
//            EditorGUIUtility.fieldWidth = 45f;
//            GUILayout.Space(10f);
//            this.DoColorSwatchAndEyedropper();
//            GUILayout.Space(10f);
//            this.DoColorSpaceGUI();
//            GUILayout.Space(10f);
//            this.DoColorSliders(width);
//            this.DoHexField(width);
//            GUILayout.Space(6f);
//            if (this.m_HDR)
//            {
//                this.DoExposureSlider();
//                GUILayout.Space(6f);
//                this.DoExposureSwatches();
//                GUILayout.Space(6f);
//            }

//            this.DoPresetsGUI();
//            this.HandleCopyPasteEvents();
//            EditorGUILayout.EndVertical();
//            if ((double) rect.height > 0.0 && Event.current.type == EventType.Repaint)
//                this.SetHeight(rect.height);
//            if (Event.current.type == EventType.KeyDown)
//            {
//                switch (Event.current.keyCode)
//                {
//                    case KeyCode.Return:
//                    case KeyCode.KeypadEnter:
//                        this.Close();
//                        break;
//                    case KeyCode.Escape:
//                        if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.EyeDropper)
//                        {
//                            EyeDropper.End();
//                            this.OnEyedropperCancelled();
//                            break;
//                        }

//                        Undo.RevertAllDownToGroup(this.m_ModalUndoGroup);
//                        this.m_Color.Reset();
//                        this.OnColorChanged(false);
//                        this.Close();
//                        GUIUtility.ExitGUI();
//                        break;
//                }
//            }

//            if (this.m_ColorBoxMode == ColorPicker.ColorBoxMode.EyeDropper &&
//                Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "NewKeyboardFocus")
//            {
//                EyeDropper.End();
//                this.OnEyedropperCancelled();
//            }

//            if ((Event.current.type != EventType.MouseDown || Event.current.button == 1) &&
//                Event.current.type != EventType.ContextClick)
//                return;
//            GUIUtility.keyboardControl = 0;
//            this.Repaint();
//        }

//        private void OnEyedropperCancelled()
//        {
//            this.Repaint();
//            this.m_ColorBoxMode = ColorPicker.ColorBoxMode.HSV;
//        }

//        private void SetHeight(float newHeight)
//        {
//            if ((double) newHeight == (double) this.position.height)
//                return;
//            this.minSize = new Vector2(233f, newHeight);
//            this.maxSize = new Vector2(233f, newHeight);
//        }

//        private void HandleCopyPasteEvents()
//        {
//            Event current = Event.current;
//            switch (current.type)
//            {
//                case EventType.ValidateCommand:
//                    switch (current.commandName)
//                    {
//                        case "Copy":
//                        case "Paste":
//                            current.Use();
//                            return;
//                        case null:
//                            return;
//                        default:
//                            return;
//                    }
//                case EventType.ExecuteCommand:
//                    switch (current.commandName)
//                    {
//                        case "Copy":
//                            ColorClipboard.SetColor(ColorPicker.color);
//                            current.Use();
//                            return;
//                        case "Paste":
//                            Color color;
//                            if (!ColorClipboard.TryGetColor(this.m_HDR, out color))
//                                return;
//                            if (!this.m_ShowAlpha)
//                                color.a = this.m_Color.GetColorChannelNormalized(RgbaChannel.A);
//                            this.SetColor(color);
//                            GUI.changed = true;
//                            current.Use();
//                            return;
//                        case null:
//                            return;
//                        default:
//                            return;
//                    }
//            }
//        }

//        public static Texture2D GetGradientTextureWithAlpha1To0()
//        {
//            return ColorPicker.s_LeftGradientTexture ?? (ColorPicker.s_LeftGradientTexture =
//                       ColorPicker.CreateGradientTexture("ColorPicker_1To0_Gradient", 4, 4, new Color(1f, 1f, 1f, 1f),
//                           new Color(1f, 1f, 1f, 0.0f)));
//        }

//        public static Texture2D GetGradientTextureWithAlpha0To1()
//        {
//            return ColorPicker.s_RightGradientTexture ?? (ColorPicker.s_RightGradientTexture =
//                       ColorPicker.CreateGradientTexture("ColorPicker_0To1_Gradient", 4, 4, new Color(1f, 1f, 1f, 0.0f),
//                           new Color(1f, 1f, 1f, 1f)));
//        }

//        private static Texture2D CreateGradientTexture(string name, int width, int height, Color leftColor,
//            Color rightColor)
//        {
//            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
//            texture2D.name = name;
//            texture2D.hideFlags = HideFlags.HideAndDontSave;
//            Color[] colors = new Color[width * height];
//            for (int index1 = 0; index1 < width; ++index1)
//            {
//                Color color = Color.Lerp(leftColor, rightColor, (float) index1 / (float) (width - 1));
//                for (int index2 = 0; index2 < height; ++index2)
//                    colors[index2 * width + index1] = color;
//            }

//            texture2D.SetPixels(colors);
//            texture2D.wrapMode = TextureWrapMode.Clamp;
//            texture2D.Apply();
//            return texture2D;
//        }

//        private void OnColorChanged(bool exitGUI = true)
//        {
//            this.m_OldAlpha = -1f;
//            this.m_ColorSpaceBoxDirty = true;
//            this.m_ExposureSliderMax = Mathf.Max(this.m_ExposureSliderMax, this.m_Color.exposureValue);
//            if ((UnityEngine.Object) this.m_DelegateView != (UnityEngine.Object) null)
//            {
//                Event e = EditorGUIUtility.CommandEvent("ColorPickerChanged");
//                if (!this.m_IsOSColorPicker)
//                    this.Repaint();
//                this.m_DelegateView.SendEvent(e);
//                if (!this.m_IsOSColorPicker && exitGUI)
//                    GUIUtility.ExitGUI();
//            }

//            if (this.m_ColorChangedCallback == null)
//                return;
//            this.m_ColorChangedCallback(ColorPicker.color);
//        }

//        private void SetColor(Color c)
//        {
//            if (this.m_IsOSColorPicker)
//            {
//                OSColorPicker.color = c;
//            }
//            else
//            {
//                this.m_Color.SetColorChannelHdr(RgbaChannel.R, c.r);
//                this.m_Color.SetColorChannelHdr(RgbaChannel.G, c.g);
//                this.m_Color.SetColorChannelHdr(RgbaChannel.B, c.b);
//                this.m_Color.SetColorChannelHdr(RgbaChannel.A, c.a);
//                this.OnColorChanged(true);
//                this.Repaint();
//            }
//        }

//        public static void Show(GUIView viewToUpdate, Color col, bool showAlpha = true, bool hdr = false)
//        {
//            ColorPicker.Show(viewToUpdate, (Action<Color>) null, col, showAlpha, hdr);
//        }

//        public static void Show(Action<Color> colorChangedCallback, Color col, bool showAlpha = true, bool hdr = false)
//        {
//            ColorPicker.Show((GUIView) null, colorChangedCallback, col, showAlpha, hdr);
//        }

//        private static void Show(GUIView viewToUpdate, Action<Color> colorChangedCallback, Color col, bool showAlpha,
//            bool hdr)
//        {
//            ColorPicker instance = ColorPicker.instance;
//            instance.m_HDR = hdr;
//            instance.m_Color = new ColorMutator(col);
//            instance.m_ShowAlpha = showAlpha;
//            instance.m_DelegateView = viewToUpdate;
//            instance.m_ColorChangedCallback = colorChangedCallback;
//            instance.m_ModalUndoGroup = Undo.GetCurrentGroup();
//            instance.m_ExposureSliderMax = Mathf.Max(instance.m_ExposureSliderMax, instance.m_Color.exposureValue);
//            ColorPicker.originalKeyboardControl = GUIUtility.keyboardControl;
//            if (instance.m_HDR)
//            {
//                instance.m_IsOSColorPicker = false;
//                instance.m_SliderMode = (ColorPicker.SliderMode) EditorPrefs.GetInt("CPSliderModeHDR", 0);
//            }

//            if (instance.m_IsOSColorPicker)
//            {
//                instance.SetColor(col);
//                OSColorPicker.Show(showAlpha);
//            }
//            else
//            {
//                instance.titleContent =
//                    !hdr
//                        ? EditorGUIUtility.TrTextContent("Color", (string) null, (Texture) null)
//                        : EditorGUIUtility.TrTextContent("HDR Color", (string) null, (Texture) null);
//                float y = (float) EditorPrefs.GetInt("CPickerHeight", (int) instance.position.height);
//                instance.minSize = new Vector2(233f, y);
//                instance.maxSize = new Vector2(233f, y);
//                instance.InitializePresetsLibraryIfNeeded();
//                instance.ShowAuxWindow();
//            }
//        }

//        private void PollOSColorPicker()
//        {
//            if (!this.m_IsOSColorPicker)
//                return;
//            if (!OSColorPicker.visible || Application.platform != RuntimePlatform.OSXEditor)
//            {
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this);
//            }
//            else
//            {
//                Color color = OSColorPicker.color;
//                if ((Color) this.m_Color.color != color)
//                {
//                    this.m_Color.SetColorChannel(RgbaChannel.R, color.r);
//                    this.m_Color.SetColorChannel(RgbaChannel.G, color.g);
//                    this.m_Color.SetColorChannel(RgbaChannel.B, color.b);
//                    this.m_Color.SetColorChannel(RgbaChannel.A, color.a);
//                    this.OnColorChanged(true);
//                }
//            }
//        }

//        private void OnEnable()
//        {
//            this.hideFlags = HideFlags.DontSave;
//            this.m_IsOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
//            this.hideFlags = HideFlags.DontSave;
//            EditorApplication.update += new EditorApplication.CallbackFunction(this.PollOSColorPicker);
//            EditorGUIUtility.editingTextField = true;
//            this.m_SliderMode = (ColorPicker.SliderMode) EditorPrefs.GetInt("CPSliderMode", 0);
//            this.m_ShowPresets = EditorPrefs.GetInt("CPPresetsShow", 1) != 0;
//        }

//        private void OnDisable()
//        {
//            EditorPrefs.SetInt(!this.m_HDR ? "CPSliderMode" : "CPSliderModeHDR", (int) this.m_SliderMode);
//            EditorPrefs.SetInt("CPPresetsShow", !this.m_ShowPresets ? 0 : 1);
//            EditorPrefs.SetInt("CPickerHeight", (int) this.position.height);
//        }

//        public void OnDestroy()
//        {
//            Undo.CollapseUndoOperations(this.m_ModalUndoGroup);
//            if ((bool) ((UnityEngine.Object) this.m_ColorSlider))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_ColorSlider);
//            if ((bool) ((UnityEngine.Object) this.m_ColorBox))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_ColorBox);
//            if ((bool) ((UnityEngine.Object) this.m_RTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_RTexture);
//            if ((bool) ((UnityEngine.Object) this.m_GTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_GTexture);
//            if ((bool) ((UnityEngine.Object) this.m_BTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_BTexture);
//            if ((bool) ((UnityEngine.Object) this.m_HueTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_HueTexture);
//            if ((bool) ((UnityEngine.Object) this.m_SatTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_SatTexture);
//            if ((bool) ((UnityEngine.Object) this.m_ValTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_ValTexture);
//            if ((bool) ((UnityEngine.Object) this.m_AlphaTexture))
//                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_AlphaTexture);
//            ColorPicker.s_Instance = (ColorPicker) null;
//            if (this.m_IsOSColorPicker)
//                OSColorPicker.Close();
//            EditorApplication.update -= new EditorApplication.CallbackFunction(this.PollOSColorPicker);
//            if (this.m_ColorLibraryEditorState != null)
//                this.m_ColorLibraryEditorState.TransferEditorPrefsState(false);
//            if (this.m_ColorLibraryEditor != null)
//                this.m_ColorLibraryEditor.UnloadUsedLibraries();
//            GUIUtility.keyboardControl = ColorPicker.originalKeyboardControl;
//            ColorPicker.originalKeyboardControl = 0;
//        }

//        private enum ColorBoxMode
//        {
//            HSV,
//            EyeDropper,
//        }

//        private enum SliderMode
//        {
//            RGB,
//            RGBFloat,
//            HSV,
//        }

//        private static class Styles
//        {
//            public static readonly RectOffset colorBoxPadding = new RectOffset(6, 6, 6, 6);
//            public static readonly Color lowLuminanceContentColor = Color.white;
//            public static readonly Color highLuminanceContentColor = Color.black;
//            public static readonly GUIStyle originalColorSwatch = (GUIStyle) "ColorPickerOriginalColor";
//            public static readonly GUIStyle currentColorSwatch = (GUIStyle) "ColorPickerCurrentColor";
//            public static readonly GUIStyle colorBoxThumb = (GUIStyle) "ColorPicker2DThumb";
//            public static readonly GUIStyle hueDialBackground = (GUIStyle) "ColorPickerHueRing";
//            public static readonly GUIStyle hueDialBackgroundHDR = (GUIStyle) "ColorPickerHueRing-HDR";
//            public static readonly GUIStyle hueDialThumb = (GUIStyle) "ColorPickerHueRingThumb";
//            public static readonly GUIStyle sliderBackground = (GUIStyle) "ColorPickerSliderBackground";
//            public static readonly GUIStyle sliderThumb = (GUIStyle) "ColorPickerHorizThumb";
//            public static readonly GUIStyle background = new GUIStyle((GUIStyle) "ColorPickerBackground");
//            public static readonly GUIStyle exposureSwatch = (GUIStyle) "ColorPickerExposureSwatch";

//            public static readonly GUIStyle selectedExposureSwatchStroke =
//                (GUIStyle) "ColorPickerCurrentExposureSwatchBorder";

//            public static readonly GUIContent eyeDropper =
//                EditorGUIUtility.TrIconContent("EyeDropper.Large", "Pick a color from the screen.");

//            public static readonly GUIContent exposureValue = EditorGUIUtility.TrTextContent("Intensity",
//                "Number of stops to over- or under-expose the color.", (Texture) null);

//            public static readonly GUIContent hexLabel =
//                EditorGUIUtility.TrTextContent("Hexadecimal", (string) null, (Texture) null);

//            public static readonly GUIContent presetsToggle =
//                EditorGUIUtility.TrTextContent("Swatches", (string) null, (Texture) null);

//            public static readonly ScalableGUIContent originalColorSwatchFill = new ScalableGUIContent(string.Empty,
//                "The original color. Click this swatch to reset the color picker to this value.",
//                "ColorPicker-OriginalColor");

//            public static readonly ScalableGUIContent currentColorSwatchFill =
//                new ScalableGUIContent(string.Empty, "The new color.", "ColorPicker-CurrentColor");

//            public static readonly ScalableGUIContent hueDialThumbFill =
//                new ScalableGUIContent("ColorPicker-HueRing-Thumb-Fill");

//            public static readonly Texture2D alphaSliderCheckerBackground =
//                EditorGUIUtility.LoadRequired("Previews/Textures/textureChecker.png") as Texture2D;

//            public static readonly GUIContent[] sliderModeLabels = new GUIContent[3]
//            {
//                EditorGUIUtility.TrTextContent("RGB 0-255", (string) null, (Texture) null),
//                EditorGUIUtility.TrTextContent("RGB 0-1.0", (string) null, (Texture) null),
//                EditorGUIUtility.TrTextContent("HSV", (string) null, (Texture) null)
//            };

//            public static readonly int[] sliderModeValues = new int[3]
//            {
//                0,
//                1,
//                2
//            };

//            public const float fixedWindowWidth = 233f;
//            public const float hexFieldWidth = 72f;
//            public const float sliderModeFieldWidth = 72f;
//            public const float channelSliderLabelWidth = 14f;
//            public const float sliderTextFieldWidth = 45f;
//            public const float extraVerticalSpacing = 6f;
//            public const float highLuminanceThreshold = 0.5f;
//            public static readonly float hueDialThumbSize;

//            static Styles()
//            {
//                Vector2 vector2 =
//                    ColorPicker.Styles.hueDialThumb.CalcSize((GUIContent) ColorPicker.Styles.hueDialThumbFill);
//                ColorPicker.Styles.hueDialThumbSize = Mathf.Max(vector2.x, vector2.y);
//            }
//        }
//    }
//}