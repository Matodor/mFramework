using System;
using mFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class UIColorAnimationSettings : AnimationSettings
    {
        public UIColor From = UIColor.White;
        public UIColor To = UIColor.Black;
    }

    public class UIColorAnimation : BaseAnimation<UIColorAnimationSettings>
    {
        public Graphic Graphic;

        protected override void Awake()
        {
            base.Awake();

            Graphic = Graphic ?? GetComponent<Graphic>();
        }

        public override void Animate()
        {
            if (Graphic != null)
            {
                Graphic.color = (Color) UIColor.Lerp(
                    Settings.From,
                    Settings.To,
                    EasingTime);
            }
        }
    }
}