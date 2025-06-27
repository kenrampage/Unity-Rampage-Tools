using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Obvious.Soap;
using System;

namespace KenRampage.Addons.SOAP.Listeners
{
    /// <summary>
    /// Listens for changes in a ScriptableVariable<bool> and triggers corresponding UnityEvents when the value matches defined conditions.
    /// Supports multiple value-based responses through a configurable list of EventValueResponseBool entries.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Listeners/Variable Value Listener Bool")]
    public class VariableValueListenerBool : MonoBehaviour
    {
        [SerializeField] private BoolVariable _scriptableVariable;
        [SerializeField] private bool _processOnEnable;
        [SerializeField] private List<EventValueResponseBool> _valueResponses;

        [Serializable]
        public class EventValueResponseBool
        {
            public bool Value;
            public UnityEvent<bool> Event;
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

        private void ProcessValueResponses(bool value)
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