using UnityEngine;

namespace mFramework.Animations.Types
{
    [SerializeField]
    public class BezierCubicAnimationSettings : AnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Vector2 FourthPoint;
        public Space RelativeTo = Space.World;
    }

    public class BezierCubicAnimation : BaseAnimation<BezierCubicAnimationSettings>
    {
        public override void Animate()
        {
            
        }
    }
}