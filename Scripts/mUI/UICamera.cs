// ReSharper disable ArrangeAccessorOwnerBody
using UnityEngine;

namespace mFramework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class UICamera : MonoBehaviour
    {
        public Vector2 DeltaSize
        {
            get
            {
                return new Vector2(
                    Camera.orthographicSize * Camera.aspect * 2,
                    Camera.orthographicSize * 2);
            }
        }

        public Camera Camera { get; private set; }

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Init();
        }

        private void Init()
        {
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.depth = 0;
            Camera.orthographic = true;
            Camera.orthographicSize = 5f;
            Camera.farClipPlane = 1f;
            Camera.nearClipPlane = -1;
            Camera.backgroundColor = Color.gray;
        }

        // ReSharper disable once UnusedMember.Local
        private void Reset()
        {
            Init();
        }
    }
}