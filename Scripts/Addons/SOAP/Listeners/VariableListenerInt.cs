using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<int> and invokes UnityEvents with the updated value.
    /// Supports both integer and float event callbacks, with automatic type conversion for float events.
    /// Can optionally process the current value when enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Listener Int")]
    public class VariableListenerInt : MonoBehaviour
    {
        [SerializeField] private IntVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;

        public UnityEvent<int> Event;
        public UnityEvent<float> FloatEvent;

        private void OnEnable()
        {
            _scriptableVariable.OnValueChanged += ProcessResponse;

            if (_processOnEnable)
            {
                ProcessResponse(_scriptableVariable.Value);
            }
        }

        private void OnDisable()
        {
            _scriptableVariable.OnValueChanged -= ProcessResponse;
        }

        private void ProcessResponse(int value)
        {
            Event?.Invoke(value);
            FloatEvent?.Invoke(value);
        }
    }
}