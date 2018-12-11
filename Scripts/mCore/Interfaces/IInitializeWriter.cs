#if UNITY_EDITOR
namespace mFramework.Core.Interfaces
{
    public interface IInitializeWriter
    {
        void GenerateInitialize(IClassWriter _, string identifier);
    }
}
#endif