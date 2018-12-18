using System;
using UnityEngine;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class TurnAroundAnimationSettings : TurnAnimationSettings
    {
        public Vector3 Point;
    }

    public class TurnAroundAnimation : BaseAnimation<TurnAroundAnimationSettings>
    {
        public override void Animate()
        {
            transform.RotateAround(
                Settings.RelativeTo == Space.Self 
                    ? transform.TransformPoint(Settings.Point) 
                    : Settings.Point,
                Settings.Axis,
                Settings.TurnValue * DeltaEasingTime);
        }
    }
}