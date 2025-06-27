using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<bool> and invokes UnityEvents with the updated value.
    /// Can optionally process the current value when enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Listener Bool")]
    public class VariableListenerBool : MonoBehaviour
    {
        [SerializeField] private BoolVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;

        public UnityEvent<bool> Event;

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

        private void ProcessResponse(bool value)
        {
            Event?.Invoke(value);
        }
    }
}