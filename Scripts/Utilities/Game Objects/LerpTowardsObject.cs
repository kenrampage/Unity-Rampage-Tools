using UnityEngine;

namespace KenRampage.Utilities.Movement
{
    /// <summary>
    /// Smoothly moves this object towards a target object using linear interpolation (Lerp).
    /// Movement can be toggled on/off and the lerp speed is configurable.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Movement/Lerp Towards Object")]
    public class LerpTowardsObject : MonoBehaviour
    {
        [SerializeField] private Transform _targetObject;
        [SerializeField] private float _lerpSpeed = 5f;
        private bool _isActive = true;

        public void IsActive(bool b)
        {
            _isActive = b;
        }

        private void Update()
        {
            if (_isActive && _targetObject != null)
            {
                transform.position = Vector3.Lerp(transform.position, _targetObject.position, _lerpSpeed * Time.deltaTime);
            }
        }
    }
}