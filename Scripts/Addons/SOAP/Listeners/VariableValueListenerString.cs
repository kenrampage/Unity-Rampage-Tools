using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;
using System;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<string> and triggers corresponding UnityEvents when the value matches defined conditions.
    /// Supports multiple value-based responses through a configurable list of EventValueResponseString entries.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Value Listener String")]
    public class VariableValueListenerString : MonoBehaviour
    {
        [SerializeField] private StringVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;
        [SerializeField] private List<EventValueResponseString> _valueResponses;

        [Serializable]
        public class EventValueResponseString
        {
            public string Value;
            public UnityEvent<string> Event;
        }

        private void OnEnable()
        {
            _scriptableVariable.OnValueChanged += ProcessValueResponses;

            if (_processOnEnable)
            {
                ProcessValueResponses(_scriptableVariable.Value);
            }
        }

        private void OnDisable()
        {
            _scriptableVariable.OnValueChanged -= ProcessValueResponses;
        }

        private void ProcessValueResponses(string value)
        {
            foreach (var response in _valueResponses)
            {
                if (response.Value == value)
                {
                    response.Event?.Invoke(value);
                }
            }
        }
    }
}