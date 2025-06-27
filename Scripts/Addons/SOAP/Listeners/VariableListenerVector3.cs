using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<Vector3> and invokes UnityEvents with the updated value.
    /// Can optionally process the current value when enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Listener Vector3")]
    public class VariableListenerVector3 : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;

        public UnityEvent<Vector3> OnValueChangedEvent;

        private void OnEnable()
        {
            if (_scriptableVariable != null)
            {
                _scriptableVariable.OnValueChanged += ProcessResponse;

                if (_processOnEnable)
                {
                    ProcessResponse(_scriptableVariable.Value);
                }
            }
            else
            {
                Debug.LogWarning("ScriptableVariable<Vector3> is not assigned in the inspector.", this);
            }
        }

        private void OnDisable()
        {
            if (_scriptableVariable != null)
            {
                _scriptableVariable.OnValueChanged -= ProcessResponse;
            }
        }

        private void ProcessResponse(Vector3 value)
        {
            OnValueChangedEvent?.Invoke(value);
        }
    }
}