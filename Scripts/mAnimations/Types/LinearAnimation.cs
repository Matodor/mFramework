using System;
using mFramework.Core;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class LinearAnimationSettings : AnimationSettings
    {
        public Vector3 From;
        public Vector3 To;
        public Space RelativeTo = Space.Self;
    }

    public class LinearAnimation : BaseAnimation<LinearAnimationSettings>
    {
        public override void Animate()
        {
            if (Settings.RelativeTo == Space.World)
            {
                transform.position = BezierHelper.Linear(
                    EasingTime, Settings.From, Settings.To);
            }
            else
            {
                transform.localPosition = BezierHelper.Linear(
                    EasingTime, Settings.From, Settings.To);
            }
        }
    }
}