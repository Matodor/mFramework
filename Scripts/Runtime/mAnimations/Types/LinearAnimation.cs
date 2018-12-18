using System;
using mFramework.Core;
using mFramework.Core.Extensions;
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
            transform.Position(
                BezierHelper.Linear(EasingTime, Settings.From, Settings.To), 
                Settings.RelativeTo);
        }
    }
}