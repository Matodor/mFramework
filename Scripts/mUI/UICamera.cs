// ReSharper disable ArrangeAccessorOwnerBody
using UnityEngine;

namespace mFramework.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(Camera))]
    public sealed class UICamera : MonoBehaviour
    {
        public Vector2 DeltaSize
        {
            get
            {
                return new Vector2(
                    _camera.orthographicSize * _camera.aspect * 2,
                    _camera.orthographicSize * 2);
            }
        }

        private Camera _camera;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Init();
        }

        private void Init()
        {
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.depth = 0;
            _camera.orthographic = true;
            _camera.orthographicSize = 5f;
            _camera.farClipPlane = 1f;
            _camera.nearClipPlane = -1;
            _camera.backgroundColor = Color.gray;
        }

        // ReSharper disable once UnusedMember.Local
        private void Reset()
        {
            Init();
        }
    }
}