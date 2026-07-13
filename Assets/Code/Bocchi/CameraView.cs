using Code.System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Bocchi
{
    public class CameraView : MonoBehaviour
    {
        [Header("settings")]
        [SerializeField] float mouseSensitivity = 0.12f;
        [SerializeField] float defaultPitch;
        [SerializeField] float minPitch = -80f;
        [SerializeField] float maxPitch = 80f;
        [SerializeField] Vector3 cameraOffset = new Vector3(0f, 1.1f, 0f);
        [SerializeField] bool lockCursorOnEnable = true;
        [SerializeField] LayerMask cameraCollisionMask = ~0;
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] float cameraCollisionPadding = 0.1f;
        [SerializeField] bool hideVisualsFromMainCamera = true;
        [SerializeField] int firstPersonHiddenLayer = 8;

        [Header("components")]
        [SerializeField] CinemachineCamera bocchiCamera;
        [SerializeField] Camera renderCamera;
        [SerializeField] InputSO input;
        [SerializeField] Transform yawRoot;
        [SerializeField] Transform visualRoot;

        readonly RaycastHit[] _cameraHits = new RaycastHit[8];
        Transform[] _visualTransforms;
        int[] _visualLayers;
        int _originalCameraCullingMask;
        bool _cameraMaskCached;
        float _pitch;

        private void Awake()
        {
            yawRoot = yawRoot != null ? yawRoot : transform;
            input = input != null ? input : GetComponent<Player>()?.InputSO;
            visualRoot = visualRoot != null ? visualRoot : transform.Find("Visual");
            renderCamera = renderCamera != null ? renderCamera : Camera.main;
            _pitch = Mathf.Clamp(defaultPitch, minPitch, maxPitch);

            if (bocchiCamera != null)
            {
                bocchiCamera.Follow = null;
                bocchiCamera.LookAt = null;
            }

            CacheVisualLayers();
            ApplyFirstPersonCulling();
            ApplyCameraPose();
        }

        private void OnEnable()
        {
            ApplyFirstPersonCulling();

            if (!lockCursorOnEnable)
            {
                return;
            }

            SetCursorLocked(true);
        }

        private void OnDisable()
        {
            RestoreFirstPersonCulling();

            if (!lockCursorOnEnable)
            {
                return;
            }

            SetCursorLocked(false);
        }

        private void LateUpdate()
        {
            if (input == null || yawRoot == null)
            {
                return;
            }

            input.Refresh();

            if (input.ConsumeCursorTogglePressed())
            {
                SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);
            }

            Vector2 mouseDelta = input.ConsumeMouseDelta();

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (lockCursorOnEnable && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    SetCursorLocked(true);
                }

                ApplyCameraPose();
                return;
            }

            yawRoot.Rotate(Vector3.up, mouseDelta.x * mouseSensitivity, Space.World);
            _pitch = Mathf.Clamp(_pitch - mouseDelta.y * mouseSensitivity, minPitch, maxPitch);

            ApplyCameraPose();
        }

        private void ApplyCameraPose()
        {
            if (bocchiCamera == null || yawRoot == null)
            {
                return;
            }

            Quaternion yawRotation = Quaternion.Euler(0f, yawRoot.eulerAngles.y, 0f);
            Quaternion cameraRotation = yawRotation * Quaternion.Euler(_pitch, 0f, 0f);
            Vector3 pivotPosition = yawRoot.position + yawRotation * new Vector3(cameraOffset.x, cameraOffset.y, 0f);
            Vector3 desiredCameraPosition = pivotPosition + cameraRotation * new Vector3(0f, 0f, cameraOffset.z);
            Vector3 cameraPosition = ResolveCameraCollision(pivotPosition, desiredCameraPosition);

            bocchiCamera.transform.SetPositionAndRotation(cameraPosition, cameraRotation);
        }

        public void ResetView()
        {
            _pitch = Mathf.Clamp(defaultPitch, minPitch, maxPitch);
            ApplyCameraPose();
        }

        private void CacheVisualLayers()
        {
            if (!hideVisualsFromMainCamera || visualRoot == null)
            {
                return;
            }

            _visualTransforms = visualRoot.GetComponentsInChildren<Transform>(true);
            _visualLayers = new int[_visualTransforms.Length];

            for (int i = 0; i < _visualTransforms.Length; i++)
            {
                _visualLayers[i] = _visualTransforms[i] != null ? _visualTransforms[i].gameObject.layer : 0;
            }
        }

        private void ApplyFirstPersonCulling()
        {
            if (!hideVisualsFromMainCamera || _visualTransforms == null)
            {
                return;
            }

            for (int i = 0; i < _visualTransforms.Length; i++)
            {
                if (_visualTransforms[i] != null)
                {
                    _visualTransforms[i].gameObject.layer = firstPersonHiddenLayer;
                }
            }

            if (renderCamera == null)
            {
                return;
            }

            if (!_cameraMaskCached)
            {
                _originalCameraCullingMask = renderCamera.cullingMask;
                _cameraMaskCached = true;
            }

            renderCamera.cullingMask &= ~(1 << firstPersonHiddenLayer);
        }

        private void RestoreFirstPersonCulling()
        {
            if (_visualTransforms == null || _visualLayers == null)
            {
                return;
            }

            for (int i = 0; i < _visualTransforms.Length; i++)
            {
                if (_visualTransforms[i] != null)
                {
                    _visualTransforms[i].gameObject.layer = _visualLayers[i];
                }
            }

            if (_cameraMaskCached && renderCamera != null)
            {
                renderCamera.cullingMask = _originalCameraCullingMask;
            }
        }

        private Vector3 ResolveCameraCollision(Vector3 pivotPosition, Vector3 desiredCameraPosition)
        {
            Vector3 cameraVector = desiredCameraPosition - pivotPosition;
            float desiredDistance = cameraVector.magnitude;

            if (desiredDistance <= 0.001f)
            {
                return desiredCameraPosition;
            }

            Vector3 direction = cameraVector / desiredDistance;
            int hitCount = Physics.SphereCastNonAlloc(
                pivotPosition,
                cameraCollisionRadius,
                direction,
                _cameraHits,
                desiredDistance,
                cameraCollisionMask,
                QueryTriggerInteraction.Ignore);

            float nearestDistance = desiredDistance;

            for (int i = 0; i < hitCount; i++)
            {
                Transform hitTransform = _cameraHits[i].transform;
                if (hitTransform != null && yawRoot != null && hitTransform.IsChildOf(yawRoot))
                {
                    continue;
                }

                nearestDistance = Mathf.Min(nearestDistance, Mathf.Max(0f, _cameraHits[i].distance - cameraCollisionPadding));
            }

            return pivotPosition + direction * nearestDistance;
        }

        private void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
