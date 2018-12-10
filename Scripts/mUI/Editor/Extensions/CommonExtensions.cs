using UnityEngine;

namespace mFramework.Editor.Extensions
{
    public static class CommonExtensions
    {
        public static string StringCtor(this Quaternion v)
        {
            return $"new Quaternion({v.x}f, {v.y}f, {v.z}f,{v.w}f)";
        }

        public static string StringCtor(this Vector2 v)
        {
            return $"new Vector2({v.x}f, {v.y}f)";
        }

        public static string StringCtor(this Vector3 v)
        {
            return $"new Vector3({v.x}f, {v.y}f, {v.z}f)";
        }
    }
}