using UnityEngine;

namespace KenRampage.Utilities.Camera
{
    /// <summary>
    /// Provides access to a camera based on tag or falls back to the main camera.
    /// Useful for maintaining consistent camera references across different systems.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Camera/Camera Provider")]
    public class CameraProvider : MonoBehaviour
    {
        [SerializeField] private string _cameraTag;
        private UnityEngine.Camera _camera;

        private void OnEnable()
        {
            FindCamera();
        }

        private void FindCamera()
        {
            GameObject cameraObject = null;

            if (_cameraTag != string.Empty)
            {
                cameraObject = GameObject.FindWithTag(_cameraTag);
            }

            if (cameraObject != null && cameraObject.GetComponent<UnityEngine.Camera>() != null)
            {
                _camera = cameraObject.GetComponent<UnityEngine.Camera>();
            }
            else
            {
                if (UnityEngine.Camera.main != null)
                {
                    _camera = UnityEngine.Camera.main;
                }
                else
                {
                    Debug.LogError("No main camera found in the scene.");
                    _camera = null;
                }
            }
        }

        public UnityEngine.Camera GetCamera()
        {
            return _camera;
        }
    }
}