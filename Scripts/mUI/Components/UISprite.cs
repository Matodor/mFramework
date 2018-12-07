using mFramework.UI.Interfaces;
using UnityEngine;

namespace mFramework.UI
{
    public class UISprite : UIComponent, IRenderer<SpriteRenderer>, IRenderer
    {
        public SpriteRenderer Renderer { get; private set; }
        Renderer IRenderer.Renderer => Renderer;

        protected UISprite()
        {
        }

        protected override void Awake()
        {
            base.Awake();

            Renderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }
}