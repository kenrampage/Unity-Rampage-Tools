using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent
using TMPro;             // Required for TMP_Dropdown
using System.Collections.Generic;
using System.Linq;       // For Linq operations like Select

namespace KenRampage.Utilities.UI.Bindings
{
    /// <summary>
    /// A serializable class to hold a display name and a UnityEvent for a dropdown option.
    /// </summary>
    [System.Serializable]
    public class DropdownOptionAction
    {
        [Tooltip("The name to display in the dropdown for this option.")]
        public string optionName = "New Option";
        [Tooltip("The image to display in the dropdown for this option (optional).")]
        public Sprite optionImage = null;
        [Tooltip("A color associated with this option. Can be used by the UnityEvent for custom visual feedback (e.g., image tint). Default is white.")]
        public Color optionColor = Color.white;
        [Tooltip("The UnityEvent to invoke when this option is selected.")]
        public UnityEvent onSelected = new UnityEvent();
    }

    /// <summary>
    /// Binds a TMP_Dropdown to a list of configurable actions (DropdownOptionAction).
    /// Each option in the dropdown, when selected, invokes its associated UnityEvent.
    /// Supports setting a default option on start.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/UI/Bindings/Bind Dropdown To Events")]
    public class BindDropdownToEvents : MonoBehaviour
    {
        #region Public Fields / UI References
        [Header("UI References")]
        [Tooltip("The TMP_Dropdown UI element to populate and listen to.")]
        public TMP_Dropdown targetDropdown;
        #endregion

        #region Dropdown Configuration / Actions
        [Header("Dropdown Actions")]
        [Tooltip("A list of actions to associate with dropdown options. Each entry defines a display name and a UnityEvent.")]
        public List<DropdownOptionAction> optionActions = new List<DropdownOptionAction>();
        #endregion

        #region Settings
        [Header("Settings")]
        [Tooltip("The index of the option to select by default when the scene starts. Set to -1 for no default selection (first item will be chosen if list is not empty). The corresponding UnityEvent will be invoked.")]
        public int defaultSelectionIndex = 0;

        [Tooltip("If true, the UnityEvent for the default selection will be invoked when Start is called.")]
        public bool invokeEventForDefaultSelectionOnStart = true;

        [Tooltip("If true, allows re-selecting the current option to re-trigger its 'On Selected' event. Primarily affects programmatic calls to SetSelectedOption or if the dropdown itself triggers onValueChanged on re-selection.")]
        public bool allowRetriggerOnReselect = false;

        [Header("Debug")]
        [Tooltip("Enable console logs for this component.")]
        public bool enableDebugLogs = true;
        #endregion

        #region Private Fields
        private bool _isInitialized = false;
        #endregion

        #region Public Properties
        /// <summary>Gets a value indicating whether the dropdown binder has been initialized.</summary>
        public bool IsInitialized => _isInitialized;
        #endregion

        #region Unity Lifecycle Methods
        void Start()
        {
            Initialize();
        }

        void OnDestroy()
        {
            // Clean up listener when the GameObject is destroyed
            if (targetDropdown != null)
            {
                targetDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the dropdown, populates it, sets the default selection, and subscribes to events.
        /// Can be called manually if Start order is an issue.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            if (targetDropdown == null)
            {
                if (enableDebugLogs) Debug.LogError($"[{GetType().Name}] Target TMP_Dropdown reference is not set!", this);
                enabled = false; // Disable component if dropdown is missing
                return;
            }

            if (optionActions == null || optionActions.Count == 0)
            {
                if (enableDebugLogs) Debug.LogWarning($"[{GetType().Name}] No actions provided in optionActions list.", this);
                targetDropdown.ClearOptions();
                targetDropdown.AddOptions(new List<string> { "No Options Available" });
                targetDropdown.interactable = false;
                enabled = false; // Disable component if no actions are available
                return;
            }

            PopulateDropdown();
            InitializeDefaultSelection();

            // Add listener for when the dropdown value changes
            targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            _isInitialized = true;
        }

        /// <summary>
        /// Allows external scripts to programmatically set the selected option by index.
        /// This will also trigger the OnDropdownValueChanged logic and associated UnityEvent.
        /// </summary>
        /// <param name="index">The index of the option to select.</param>
        public void SetSelectedOption(int index)
        {
            if (targetDropdown == null || !_isInitialized)
            {
                if (enableDebugLogs) Debug.LogWarning($"[{GetType().Name}] Cannot set selected option. Dropdown not initialized or null.", this);
                return;
            }
            if (index >= 0 && index < targetDropdown.options.Count)
            {
                if (!allowRetriggerOnReselect && targetDropdown.value == index) // Avoid re-triggering if already set and re-trigger is disallowed
                {
                    if (enableDebugLogs) Debug.Log($"[{GetType().Name}] SetSelectedOption: Dropdown already at index {index}. Re-triggering is disabled. No change.", this);
                    return;
                }
                targetDropdown.value = index; // This will trigger onValueChanged and thus OnDropdownValueChanged
            }
            else
            {
                if (enableDebugLogs) Debug.LogError($"[{GetType().Name}] SetSelectedOption: Index {index} is out of range for {targetDropdown.options.Count} options.", this);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populates the dropdown with names from the optionActions list.
        /// </summary>
        private void PopulateDropdown()
        {
            targetDropdown.ClearOptions();

            // Create a list of OptionData objects from the DropdownOptionAction configurations
            List<TMP_Dropdown.OptionData> displayNames = optionActions.Select(action =>
                new TMP_Dropdown.OptionData(action.optionName, action.optionImage, action.optionColor)
            ).ToList();

            // The color is now passed to the OptionData constructor.
            // How it's visually applied might depend on your TMP_Dropdown item template.
            targetDropdown.AddOptions(displayNames);

            if (enableDebugLogs) Debug.Log($"[{GetType().Name}] Dropdown populated with {displayNames.Count} options.", this);
        }

        /// <summary>
        /// Sets the initial selection in the dropdown and optionally invokes its UnityEvent.
        /// </summary>
        private void InitializeDefaultSelection()
        {
            int initialIndex = 0;

            if (defaultSelectionIndex >= 0 && defaultSelectionIndex < optionActions.Count)
            {
                initialIndex = defaultSelectionIndex;
            }
            else if (optionActions.Count > 0)
            {
                initialIndex = 0; // Default to the first item if defaultSelectionIndex is invalid
                if (defaultSelectionIndex != -1 && enableDebugLogs)
                {
                    Debug.LogWarning($"[{GetType().Name}] defaultSelectionIndex ({defaultSelectionIndex}) is out of range. Defaulting to index 0.", this);
                }
            }
            else
            {
                if (enableDebugLogs) Debug.LogWarning($"[{GetType().Name}] No options available to set a default selection.", this);
                return; // No actions to select
            }

            targetDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            targetDropdown.value = initialIndex;
            targetDropdown.RefreshShownValue();
            targetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            if (invokeEventForDefaultSelectionOnStart)
            {
                if (enableDebugLogs) Debug.Log($"[{GetType().Name}] Invoking event for default selection: '{optionActions[initialIndex].optionName}' at index {initialIndex}.", this);
                optionActions[initialIndex]?.onSelected?.Invoke();
            }
            else
            {
                if (enableDebugLogs) Debug.Log($"[{GetType().Name}] Default selection set to: '{optionActions[initialIndex].optionName}' at index {initialIndex}. Event invocation on start is disabled.", this);
            }
        }

        /// <summary>
        /// Called when the dropdown selection changes. Invokes the UnityEvent of the selected option.
        /// </summary>
        /// <param name="index">The new selected index in the dropdown.</param>
        private void OnDropdownValueChanged(int index)
        {
            if (index < 0 || index >= optionActions.Count)
            {
                if (enableDebugLogs) Debug.LogError($"[{GetType().Name}] Selected index {index} is out of range for optionActions.", this);
                return;
            }

            DropdownOptionAction selectedAction = optionActions[index];
            if (enableDebugLogs) Debug.Log($"[{GetType().Name}] Dropdown value changed. Selected: '{selectedAction.optionName}'. Invoking its UnityEvent.", this);
            selectedAction?.onSelected?.Invoke();
        }
        #endregion

    }
}