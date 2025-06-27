using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<float> and invokes UnityEvents with the updated value.
    /// Supports both float and integer event callbacks, with automatic rounding for integer events.
    /// Can optionally process the current value when enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Listener Float")]
    public class VariableListenerFloat : MonoBehaviour
    {
        [SerializeField] private FloatVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;

        public UnityEvent<float> Event;
        public UnityEvent<int> IntEvent;

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

        private void ProcessResponse(float value)
        {
            Event?.Invoke(value);
            IntEvent?.Invoke(Mathf.RoundToInt(value));
        }
    }
}