using System;
using UnityEngine;

namespace mFramework.UI
{
    [Serializable]
    public struct UIColor
    {
        public static readonly UIColor White = new UIColor(1, 1, 1, 1, ColorType.RGBA);
        public static readonly UIColor Black = new UIColor(0, 0, 0, 1, ColorType.RGBA);

        /// <summary>
        /// Color type
        /// </summary>
        public readonly ColorType Type;

        /// <summary>
        /// Normalized color component (R - rgba, H - hsv)
        /// </summary>
        public float N1;

        /// <summary>
        /// Normalized color component (G - rgba, S - hsv)
        /// </summary>
        public float N2;

        /// <summary>
        /// Normalized color component (B - rgba, V - hsv)
        /// </summary>
        public float N3;

        /// <summary>
        /// Normalized opacity (alpha channel)
        /// </summary>
        public float Alpha;

        public UIColor(Color color)
        {
            N1 = color.r;
            N2 = color.g;
            N3 = color.b;
            Alpha = color.a;
            Type = ColorType.RGBA;
        }

        public UIColor(float n1, float n2, float n3, float alpha, ColorType type)
        {
            N1 = n1;
            N2 = n2;
            N3 = n3;
            Alpha = alpha;
            Type = type;
        }

        public static UIColor ToRGBA(UIColor hsv)
        {
            if (hsv.Type == ColorType.RGBA)
                return hsv;

            var color = Color.HSVToRGB(hsv.N1, hsv.N2, hsv.N3);
            return new UIColor(color.r, color.g, color.b, hsv.Alpha, ColorType.RGBA);
        }

        public static UIColor ToHSV(UIColor rgbColor)
        {
            if (rgbColor.Type == ColorType.HSV)
                return rgbColor;

            float h, s, v;
            if (rgbColor.N3 > rgbColor.N2 && rgbColor.N3 > rgbColor.N1)
                RGBToHSVHelper(4f, rgbColor.N3, rgbColor.N1, rgbColor.N2, out h, out s, out v);
            else if (rgbColor.N2 > rgbColor.N1)
                RGBToHSVHelper(2f, rgbColor.N2, rgbColor.N3, rgbColor.N1, out h, out s, out v);
            else
                RGBToHSVHelper(0.0f, rgbColor.N1, rgbColor.N2, rgbColor.N3, out h, out s, out v);

            return new UIColor(h, s, v, rgbColor.Alpha, ColorType.HSV);
        }
        
        private static void RGBToHSVHelper(float offset, float dominantcolor, 
            float colorOne, float colorTwo, 
            out float h, out float s, out float v)
        {
            v = dominantcolor;
            if (v != 0.0)
            {
                var num1 = colorOne <= colorTwo ? colorOne : colorTwo;
                var num2 = v - num1;
                if (num2 != 0.0)
                {
                    s = num2 / v;
                    h = offset + (colorOne - colorTwo) / num2;
                }
                else
                {
                    s = 0.0f;
                    h = offset + (colorOne - colorTwo);
                }
                h /= 6f;
                if (h >= 0.0)
                    return;
                ++h;
            }
            else
            {
                s = 0.0f;
                h = 0.0f;
            }
        }

        /// <summary>
        /// Create UIColor by the given color in hex format
        /// </summary>
        /// <param name="hexColor">Html color in hex format (example "#ffffff")</param>
        /// <param name="opacity">Normilized opacity</param>
        /// <returns></returns>
        public static UIColor HEX(string hexColor, float opacity = 1f)
        {
            Color color;
            if (!ColorUtility.TryParseHtmlString(hexColor, out color))
                throw new Exception("Can't parse color");

            return new UIColor(color.r, color.g, color.b, opacity, ColorType.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given normalized r,g,b,a color components
        /// </summary>
        /// <param name="r">Normalized red color component</param>
        /// <param name="g">Normalized green color component</param>
        /// <param name="b">Normalized blue color component</param>
        /// <param name="a">Normalized alpha color component</param>
        /// <returns></returns>
        public static UIColor RGBA(float r, float g, float b, float a = 1f)
        {
            return new UIColor(r, g, b, a, ColorType.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="r">Red color component [0, 255]</param>
        /// <param name="g">Green color component [0, 255]</param>
        /// <param name="b">Blue color component [0, 255]</param>
        /// <param name="a">Alpha color component [0, 255]</param>
        /// <returns></returns>
        public static UIColor RGBA(byte r, byte g, byte b, byte a = 255)
        {
            return new UIColor(r / 255f, g / 255f, b / 255f, a / 255f, ColorType.RGBA);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="h">Normalized hue color component</param>
        /// <param name="s">Normalized saturation  color component</param>
        /// <param name="v">Normalized value color component</param>
        /// <param name="a">Normalized alpha color component</param>
        /// <returns></returns>
        public static UIColor HSV(float h, float s, float v, float a = 1)
        {
            return new UIColor(h, s, v, a, ColorType.HSV);
        }

        /// <summary>
        /// Create UIColor by the given r,g,b,a color components
        /// </summary>
        /// <param name="h">Hue color component [0, 360]</param>
        /// <param name="s">Saturation  color component [0, 255]</param>
        /// <param name="v">Value color component [0, 255]</param>
        /// <param name="a">Alpha color component [0, 255]</param>
        /// <returns></returns>
        public static UIColor HSV(int h, byte s, byte v, byte a = 255)
        {
            return new UIColor(h / 360f, s / 255f, v / 255f, a / 255f, ColorType.HSV);
        }

        /// <summary>
        /// Create grayscaled color by the given r,g,b,a color components
        /// </summary>
        /// <param name="r">Normalized red color component</param>
        /// <param name="g">Normalized green color component</param>
        /// <param name="b">Normalized blue color component</param>
        /// <param name="a">Normalized alpha color component</param>
        /// <returns></returns>
        public static UIColor Grayscale(float r, float g, float b, float a = 1)
        {
            var y = 0.2126f * r + 0.7152f * g + 0.0722f * b;
            return new UIColor(y, y, y, a, ColorType.RGBA);
        }

        public static UIColor Lerp(UIColor a, UIColor b, float t)
        {
            t = Mathf.Clamp01(t);
            return new UIColor(
                n1: a.N1 + (b.N1 - a.N1) * t,
                n2: a.N2 + (b.N2 - a.N2) * t,
                n3: a.N3 + (b.N3 - a.N3) * t,
                alpha: a.Alpha + (b.Alpha - a.Alpha) * t,
                type: a.Type
            );
        }

        public static explicit operator Color(UIColor color)
        {
            if (color.Type == ColorType.HSV)
                color = ToRGBA(color);
            return new Color(color.N1, color.N2, color.N3, color.Alpha);
        }

        public static explicit operator UIColor(Color color)
        {
            return new UIColor(color.r, color.g, color.b, color.a, ColorType.RGBA);
        }

        public override string ToString()
        {
            return $"({N1} {N2} {N3} {Alpha} {Type})";
        }
    }
}