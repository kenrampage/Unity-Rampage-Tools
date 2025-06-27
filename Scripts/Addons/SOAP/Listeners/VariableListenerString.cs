using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<string> and invokes UnityEvents with the updated value.
    /// Can optionally process the current value when enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Listener String")]
    public class VariableListenerString : MonoBehaviour
    {
        [SerializeField] private StringVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;

        public UnityEvent<string> Event;

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

        private void ProcessResponse(string value)
        {
            Event?.Invoke(value);
        }
    }
}