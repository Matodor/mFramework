using UnityEngine;

namespace mFramework.UI
{
    public struct UIColor
    {
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

        public UIColor(float n1, float n2, float n3, float alpha, ColorType type)
        {
            N1 = n1;
            N2 = n2;
            N3 = n3;
            Alpha = alpha;
            Type = type;
        }
    }
}