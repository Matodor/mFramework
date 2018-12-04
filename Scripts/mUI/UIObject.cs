using UnityEngine;

namespace mFramework.UI
{
    public abstract class UIObject : MonoBehaviour
    {
        public virtual void Awake()
        {

        }

        protected virtual void Start() {}
        protected virtual void Update() {}
        protected virtual void OnEnable() {}
        protected virtual void OnDisable() {}
        protected virtual void OnDestroy() {}
    }
}
