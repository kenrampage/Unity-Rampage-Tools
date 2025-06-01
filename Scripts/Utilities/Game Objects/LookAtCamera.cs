using UnityEngine;

namespace KenRampage.Utilities.Camera
{
    /// <summary>
    /// Makes an object continuously face the camera provided by a CameraProvider.
    /// Updates orientation each frame to maintain consistent facing direction.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Camera/Look At Camera")]
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private CameraProvider _cameraProvider;
        private UnityEngine.Camera _cameraToLookAt;

        private void Start()
        {
            if (_cameraProvider != null)
            {
                _cameraToLookAt = _cameraProvider.GetCamera();
                if (_cameraToLookAt == null)
                {
                    Debug.LogError("CameraProvider did not provide a valid camera.");
                }
            }
            else
            {
                Debug.LogError("CameraProvider is not assigned.");
            }
        }

        private void LateUpdate()
        {
            if (_cameraToLookAt != null)
            {
                transform.LookAt(transform.position + _cameraToLookAt.transform.rotation * Vector3.forward,
                               _cameraToLookAt.transform.rotation * Vector3.up);
            }
        }
    }
}