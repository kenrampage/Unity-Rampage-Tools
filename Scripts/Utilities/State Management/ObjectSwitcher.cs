using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace KenRampage.Utilities.StateManagement
{
    /// <summary>
    /// Base class for switching between different GameObjects based on a provided key.
    /// Only the GameObject with the matching key will be enabled, while others are disabled.
    /// 
    /// This component's design incorporates several key design patterns. It primarily acts as a **State Machine**,
    /// where each `GameObject` represents a distinct state. It uses the **Observer** pattern via `UnityEvent`s
    /// to notify other systems of state changes in a decoupled way. The public methods expose a **Command-like** API.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/State Management/Object Switcher")]
    public class ObjectSwitcher : MonoBehaviour
    {
        #region Nested Classes
        [Serializable]
        public class ObjectMapping
        {
            public string Key;
            public GameObject GameObject;
        }

        [Serializable] 
        public class Debug
        {
            public string PreviousKey;
            public string CurrentKey;
        }

        [Serializable]
        public class EventsClass
        {
            public UnityEvent<GameObject> OnObjectSwitch;
            public UnityEvent<GameObject> OnLastEntryReached;
            public UnityEvent<GameObject> OnFirstEntryReached;
        }

        public enum InitializationTiming
        {
            Awake,
            Start,
            OnEnable,
            Never
        }
        #endregion

        #region Fields
        
        [SerializeField] protected List<ObjectMapping> _objectMappings;
        [Space()]
        [Tooltip("The key of the object to activate when the script initializes.")]
        [SerializeField] protected string _startKey;
        [Tooltip("Determines when the Start Key is applied.")]
        [SerializeField] protected InitializationTiming _setStartKeyTiming = InitializationTiming.Never;
        [SerializeField] protected bool _loop = true;
        [Space()]
        [SerializeField] protected Debug _debug;
        [SerializeField] protected EventsClass _events;

        protected int _currentIndex = -1;
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            if (_setStartKeyTiming == InitializationTiming.Awake)
                SwitchObjects(_startKey);
        }

        protected virtual void Start()
        {
            if (_setStartKeyTiming == InitializationTiming.Start)
                SwitchObjects(_startKey);
        }

        protected virtual void OnEnable()
        {
            if (_setStartKeyTiming == InitializationTiming.OnEnable)
                SwitchObjects(_startKey);
        }
        #endregion

        #region Public Methods
        public void SwitchObjects(string key)
        {
            if (_objectMappings == null) return;
            if (_debug.CurrentKey == key && _currentIndex >= 0) return; // Only return if current key is same and a valid object is selected

            _debug.PreviousKey = _debug.CurrentKey;
            _debug.CurrentKey = key;

            _currentIndex = _objectMappings.FindIndex(m => m.Key == key);

            OnSwitchObjects(key); // This will deactivate all if key not found, which is desired
            
            // Only invoke if a valid object was found
            if (_currentIndex >= 0)
            {
                _events?.OnObjectSwitch?.Invoke(_objectMappings[_currentIndex].GameObject);
            }
        }

        public int GetCurrentIndex() => _currentIndex;

        public void SwitchObjects(int index)
        {
            if (_objectMappings == null || index < 0 || index >= _objectMappings.Count)
            {
                return;
            }

            SwitchObjects(_objectMappings[index].Key);
        }

        public void CycleForward()
        {
            if (_objectMappings == null || _objectMappings.Count == 0) return;

            int nextIndex = _currentIndex + 1;
            if (nextIndex >= _objectMappings.Count)
            {
                if (_currentIndex >= 0) // Guard against invalid index
                    _events?.OnLastEntryReached?.Invoke(_objectMappings[_currentIndex].GameObject);
                if (!_loop) return;
                nextIndex = 0;
            }
            SwitchObjects(nextIndex);
        }

        public void CycleBackward()
        {
            if (_objectMappings == null || _objectMappings.Count == 0) return;

            int nextIndex = _currentIndex - 1;
            if (nextIndex < 0)
            {
                if (_currentIndex >= 0) // Guard against invalid index
                    _events?.OnFirstEntryReached?.Invoke(_objectMappings[_currentIndex].GameObject);
                if (!_loop) return;
                nextIndex = _objectMappings.Count - 1;
            }
            SwitchObjects(nextIndex);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnSwitchObjects(string key)
        {
            foreach (var mapping in _objectMappings)
            {
                if (mapping.GameObject != null)
                {
                    mapping.GameObject.SetActive(false);
                }
            }

            foreach (var mapping in _objectMappings)
            {
                if (mapping.GameObject != null && mapping.Key == key)
                {
                    mapping.GameObject.SetActive(true);
                }
            }
        }
        #endregion
    }
}