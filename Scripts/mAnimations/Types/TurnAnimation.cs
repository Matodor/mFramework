using System;
using mFramework.Core;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class TurnAnimationSettings : AnimationSettings
    {
        public float TurnValue;
        public Vector3 Axis = Vector3.forward;
        public Space RelativeTo = Space.Self;
    }

    public class TurnAnimation : BaseAnimation<TurnAnimationSettings>
    {
        public override void Animate()
        {
            transform.Rotate(
                Settings.Axis, 
                Settings.TurnValue * DeltaEasingTime,
                Settings.RelativeTo);
        }
    }
}