using UnityEngine;

namespace mFramework.Core.Extensions
{
    public static class BehaviourExtensions
    {
        public static void Position(this Transform transform, 
            Vector3 pos, Space relativeTo)
        {
            if (relativeTo == Space.World)
                transform.position = pos;
            else
                transform.localPosition = pos;
        }
    }
}