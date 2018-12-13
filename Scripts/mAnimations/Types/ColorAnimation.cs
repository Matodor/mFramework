using System;
using UnityEngine;
using UnityEngine.UI;

namespace mFramework.Animations.Types
{
    [Serializable]
    public class ColorAnimationSettings : AnimationSettings
    {
        public Color From = Color.white;
        public Color To = Color.black;
    }

    public class ColorAnimation : BaseAnimation<ColorAnimationSettings>
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
                Graphic.color = Color.LerpUnclamped(Settings.From, Settings.To, EasingTime);
        }
    }
}