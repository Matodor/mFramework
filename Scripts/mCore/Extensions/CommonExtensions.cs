using UnityEngine;

namespace mFramework.Core.Extensions
{
    public static class CommonExtensions
    {
        public static string ToString(this bool v)
        {
            return v ? "true" : "false";
        }

        public static string StringCtor(this Color v)
        {
            return $"new Color({v.r}f, {v.g}f, {v.b}f,{v.a}f)";
        }

        public static string StringCtor(this Quaternion v)
        {
            return $"new Quaternion({v.x}f, {v.y}f, {v.z}f,{v.w}f)";
        }

        public static string StringCtor(this Vector4 v)
        {
            return $"new Vector4({v.x}f, {v.y}f, {v.z}f,{v.w}f)";
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