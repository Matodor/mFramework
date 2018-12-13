using System;
using mFramework.Core;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class ScaleAnimationSettings : AnimationSettings
    {
        public Vector3 From = Vector3.one;
        public Vector3 To = Vector3.one;
    }

    public class ScaleAnimation : BaseAnimation<ScaleAnimationSettings>
    {
        public override void Animate()
        {
            transform.localScale = BezierHelper.Linear(
                EasingTime,
                Settings.From,
                Settings.To);
        }
    }
}