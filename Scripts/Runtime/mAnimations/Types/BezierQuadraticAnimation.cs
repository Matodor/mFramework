using System;
using mFramework.Core;
using mFramework.Core.Extensions;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class BezierQuadraticAnimationSettings : AnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Space RelativeTo = Space.Self;
    } 

    public class BezierQuadraticAnimation : BaseAnimation<BezierQuadraticAnimationSettings>
    {
        public override void Animate()
        {
            transform.Position(
                BezierHelper.Quadratic(EasingTime,
                    Settings.FirstPoint,
                    Settings.SecondPoint,
                    Settings.ThirdPoint),
                Settings.RelativeTo);
        }
    }
}