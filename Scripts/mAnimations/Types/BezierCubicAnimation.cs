using System;
using mFramework.Core;
using mFramework.Core.Extensions;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class BezierCubicAnimationSettings : AnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Vector2 FourthPoint;
        public Space RelativeTo = Space.Self;
    }

    public class BezierCubicAnimation : BaseAnimation<BezierCubicAnimationSettings>
    {
        public override void Animate()
        {
            transform.Position(
                BezierHelper.Cubic(EasingTime,
                    Settings.FirstPoint,
                    Settings.SecondPoint,
                    Settings.ThirdPoint,
                    Settings.FourthPoint),
                Settings.RelativeTo);
        }
    }
}