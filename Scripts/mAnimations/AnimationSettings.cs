using System;
using mFramework.Core;

namespace mFramework.Animations
{
    [Serializable]
    public class AnimationSettings
    {
        public AnimationPlayType PlayType = AnimationPlayType.PlayOnce;
        public EasingFunctions EasingType = EasingFunctions.Linear;
        public ulong MaxRepeats = 0;
        public float Duration = 1f;
        public float AnimateEvery = 0f;
        public float StartDelay = 0f;
    }
}