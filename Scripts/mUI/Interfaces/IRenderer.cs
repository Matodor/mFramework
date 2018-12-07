using UnityEngine;

namespace mFramework.UI.Interfaces
{
    public interface IRenderer
    {
        Renderer Renderer { get; }
    }

    public interface IRenderer<out T> where T : Renderer
    {
        T Renderer { get; }
    }
}