using System;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class ShiftAnimationSettings : AnimationSettings
    {
        public Vector3 Shift = Vector3.zero;
        public Space RelativeTo = Space.Self;
    }

    public class ShiftAnimation : BaseAnimation<ShiftAnimationSettings>
    {
        public override void Animate()
        {
            transform.Translate(
                Settings.Shift * DeltaEasingTime, 
                Settings.RelativeTo);
        }
    }
}