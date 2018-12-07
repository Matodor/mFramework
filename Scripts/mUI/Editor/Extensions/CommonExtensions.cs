using UnityEngine;

namespace mFramework.Editor.Extensions
{
    public static class CommonExtensions
    {
        public static string String(this Vector3 v)
        {
            return $"new Vector3({v.x}f, {v.y}f, {v.z}f)";
        }
    }
}