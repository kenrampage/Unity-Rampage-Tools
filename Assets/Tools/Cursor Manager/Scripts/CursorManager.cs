using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

namespace Tools.UGUI.CursorManager
{
    public class CursorManager : MonoBehaviour
    {
        public enum State
        {
            Default,
            Hover,
            Drag,
            Click
        }

        [System.Serializable]
        public class CursorSet
        {
            public GameObject defaultCursor;
            public GameObject hoverCursor;
            public GameObject dragCursor;
            public GameObject clickCursor;
        }

        [SerializeField] private CursorSet cursorSet; // Reference to the CursorSets
        [SerializeField] private bool _setScreenPosition; // Whether to set the screen position of the cursor object
        [SerializeField] private bool _hideHardwareCursor; // Whether to hide the hardware cursor

        private bool _clickOn;
        private bool _hoverOn;
        private bool _isDragging;
        private bool _dragStartedWhileHovering;
        private Vector2 _lastPointerPosition;

        [SerializeField] private State currentState;

        public UnityEvent OnClickStatusChanged;
        public UnityEvent OnHoverStatusChanged;

        private void OnEnable()
        {
            HideCursor();
        }

        private void OnDisable()
        {
            ShowCursor();
        }

        private void Update()
        {
            // Get the current input module from the current event system
            var inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;

            if (inputModule == null)
            {
                Debug.LogWarning("Current input module is not an InputSystemUIInputModule.");
                return;
            }

            // Read the current state of the click action
            _clickOn = inputModule.leftClick.action.ReadValue<float>() > 0;

            // Read the current pointer position
            Vector2 pointerPosition = inputModule.point.action.ReadValue<Vector2>();

            // Set the screen position of the cursor object if enabled
            if (_setScreenPosition)
            {
                transform.position = pointerPosition;
            }

            // Check if the pointer is over a UI element
            _hoverOn = EventSystem.current.IsPointerOverGameObject();

            // Determine if dragging has started while hovering
            if (_clickOn && !_isDragging && _hoverOn && Vector2.Distance(pointerPosition, _lastPointerPosition) > 0.1f)
            {
                _isDragging = true;
                _dragStartedWhileHovering = true;
            }

            // Reset dragging state when click is released
            if (!_clickOn)
            {
                _isDragging = false;
                _dragStartedWhileHovering = false;
            }

            _lastPointerPosition = pointerPosition;

            // Calculate and update the new state
            CalculateNewState();

            // Hide the cursor if required
            HideCursor();
        }

        private void CalculateNewState()
        {
            State newState;

            if (_clickOn && _hoverOn && !_isDragging)
            {
                newState = State.Click;
            }
            else if (_isDragging && _dragStartedWhileHovering)
            {
                newState = State.Drag;
            }
            else if (_hoverOn)
            {
                newState = State.Hover;
            }
            else
            {
                newState = State.Default;
            }

            if (newState != currentState)
            {
                currentState = newState;
                HandleStateChange();
            }
        }

        private void HandleStateChange()
        {
            //Debug.Log($"State changed to: {currentState}");
            UpdateCursorVisibility();
        }

        private void UpdateCursorVisibility()
        {
            // Disable all cursors
            cursorSet.defaultCursor.SetActive(false);
            cursorSet.hoverCursor.SetActive(false);
            cursorSet.dragCursor.SetActive(false);
            cursorSet.clickCursor.SetActive(false);

            // Enable the cursor for the current state
            switch (currentState)
            {
                case State.Default:
                    cursorSet.defaultCursor.SetActive(true);
                    break;
                case State.Hover:
                    cursorSet.hoverCursor.SetActive(true);
                    break;
                case State.Drag:
                    cursorSet.dragCursor.SetActive(true);
                    break;
                case State.Click:
                    cursorSet.clickCursor.SetActive(true);
                    break;
            }
        }

        private void ShowCursor()
        {
            if (_hideHardwareCursor)
            {
                Cursor.visible = true;
            }
        }

        private void HideCursor()
        {
            if (_hideHardwareCursor)
            {
                Cursor.visible = false;
            }
        }
    }
}
