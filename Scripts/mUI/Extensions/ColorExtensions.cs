using UnityEngine;

namespace mFramework.UI.Extensions
{
    public static class ColorExtensions
    {
        public static UIColor Inverted(this UIColor color)
        {
            if (color.Type == ColorType.RGBA)
                color = UIColor.ToHSV(color);
            return new UIColor(((color.N1 + 180) % 360) / 360f, color.N2, color.N3, color.Alpha, ColorType.HSV);
        }

        /// <summary>
        /// Make color darken
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="value">Normilized value [0, 1]</param>
        /// <returns></returns>
        public static Color Darken(this Color color, float value)
        {
            return color.ChangeColorBrightness(-value);
        }

        /// <summary>
        /// Make color Lighter
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="value">Normilized value [0, 1]</param>
        /// <returns></returns>
        public static Color Lighter(this Color color, float value)
        {
            return color.ChangeColorBrightness(+value);
        }

        /// <summary>
        /// Change color brightness
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="correctionFactor">Normilized correction factor [-1, 1]</param>
        /// <returns></returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                color.r *= correctionFactor;
                color.g *= correctionFactor;
                color.b *= correctionFactor;
            }
            else
            {
                color.r = (1f - color.r) * correctionFactor + color.r;
                color.g = (1f - color.g) * correctionFactor + color.g;
                color.b = (1f - color.b) * correctionFactor + color.b;
            }

            return color;
        }
    }
}