using UnityEngine;

namespace mFramework.Core.Extensions
{
    public static class RectExtensions
    {
        public static Rect Shift(this Rect rect, float x, float y = 0f)
        {
            return new Rect(rect) {x = rect.x + x, y = rect.y + y};
        }
    }
}