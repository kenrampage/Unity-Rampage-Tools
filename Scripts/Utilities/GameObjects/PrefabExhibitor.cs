using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KenRampage.Utilities.GameObjects
{
    /// <summary>
    /// PrefabExhibitor manages the display of prefabs from a collection, ensuring only one is active at a time.
    /// 
    /// This component acts as a "prefab jukebox" - it maintains a library of prefab references and controls
    /// which one is instantiated and displayed in the scene at any given moment. It automatically handles
    /// the lifecycle of instantiated objects, destroying the previous instance when a new selection is made.
    /// 
    /// The system is robust to external destruction of the active prefab instance and will maintain
    /// correct state even if the current instance is destroyed by another system.
    /// 
    /// Design Patterns:
    /// - Object Factory: Creates and destroys prefab instances on demand
    /// - State Manager: Maintains knowledge of which prefab is currently active
    /// - Command Pattern: Provides clean public interface for other scripts to control prefab selection
    /// - Observer Pattern: Uses Unity Events to notify listeners when prefab instances change
    /// 
    /// Usage:
    /// 1. Add this component to a GameObject
    /// 2. Populate the prefab library with desired prefabs
    /// 3. Set a spawn point (defaults to this GameObject's transform)
    /// 4. Call ShowNext(), ShowPrevious(), etc. methods to control which prefab is displayed
    /// 5. Subscribe to onPrefabChanged events to react to prefab switches
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Game Objects/Prefab Exhibitor")]
    public class PrefabExhibitor : MonoBehaviour
    {
        #region Fields and Properties

        [Tooltip("Collection of prefabs available for exhibition")]
        public List<GameObject> prefabLibrary = new List<GameObject>();
        
        [Tooltip("The transform where prefabs will be instantiated")]
        public Transform spawnPoint;

        /// <summary>
        /// Event triggered when a new prefab is instantiated.
        /// Passes the newly created GameObject instance.
        /// </summary>
        [Tooltip("Called when a new prefab is instantiated")]
        public UnityEvent<GameObject> onPrefabChanged = new UnityEvent<GameObject>();

        private GameObject currentInstance;
        private int currentPrefabIndex = -1;

        /// <summary>
        /// Gets the currently displayed prefab instance
        /// </summary>
        public GameObject CurrentInstance {
            get {
                // Handle case where instance was destroyed externally
                if (currentInstance == null && currentPrefabIndex != -1)
                {
                    currentPrefabIndex = -1;
                }
                return currentInstance;
            }
        }

        /// <summary>
        /// Gets the index of the currently displayed prefab in the library
        /// </summary>
        public int CurrentPrefabIndex {
            get {
                // Handle case where instance was destroyed externally
                if (currentInstance == null && currentPrefabIndex != -1)
                {
                    currentPrefabIndex = -1;
                }
                return currentPrefabIndex;
            }
        }

        /// <summary>
        /// Gets the number of prefabs in the library
        /// </summary>
        public int PrefabCount => prefabLibrary.Count;

        /// <summary>
        /// Gets whether a prefab is currently being exhibited
        /// </summary>
        public bool HasActivePrefab => currentInstance != null;

        #endregion

        #region Unity Lifecycle Methods

        private void Start()
        {
            // Validate the spawn point
            if (spawnPoint == null)
            {
                spawnPoint = transform;
                Debug.LogWarning($"[{nameof(PrefabExhibitor)}] No spawn point assigned. Using this object's transform.");
            }
        }

        private void Update()
        {
            // Check if our instance was destroyed externally
            if (currentInstance == null && currentPrefabIndex != -1)
            {
                // Update our state to reflect the instance is gone
                currentPrefabIndex = -1;
                
                // Notify listeners that the instance is gone
                onPrefabChanged.Invoke(null);
            }
        }

        private void OnDestroy()
        {
            // Clean up any instantiated prefab when this component is destroyed
            ClearActive();
        }

        #endregion

        #region Public Control Methods (Inspector-Compatible)

        /// <summary>
        /// Displays the next prefab in the library (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Show Next")]
        public void ShowNext()
        {
            Next();
        }
        
        /// <summary>
        /// Displays the previous prefab in the library (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Show Previous")]
        public void ShowPrevious()
        {
            Previous();
        }
        
        /// <summary>
        /// Displays a specific prefab by its index in the library (Inspector-compatible)
        /// </summary>
        public void ShowPrefabByIndex(int index)
        {
            SetCurrentPrefab(index);
        }
        
        /// <summary>
        /// Displays the first prefab in the library (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Show First")]
        public void ShowFirst()
        {
            First();
        }

        /// <summary>
        /// Displays the last prefab in the library (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Show Last")]
        public void ShowLast()
        {
            Last();
        }

        /// <summary>
        /// Displays a random prefab from the library (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Show Random")]
        public void ShowRandom()
        {
            Random();
        }

        /// <summary>
        /// Destroys the currently active prefab instance without spawning a new one.
        /// Does NOT affect the prefab library. (Inspector-compatible)
        /// </summary>
        [ContextMenu("Prefab Exhibitor/Clear Active")]
        public void ClearActive()
        {
            if (currentInstance != null)
            {
                Destroy(currentInstance);
                currentInstance = null;
                currentPrefabIndex = -1;
                
                // Invoke the change event with null to indicate clearing
                onPrefabChanged.Invoke(null);
            }
        }

        #endregion

        #region Public Control Methods (With Return Values)

        /// <summary>
        /// Displays the next prefab in the library
        /// </summary>
        /// <returns>The newly instantiated GameObject or null if the library is empty</returns>
        public GameObject Next()
        {
            if (prefabLibrary.Count == 0)
                return null;
            
            // Check if our instance was destroyed externally
            if (currentInstance == null && currentPrefabIndex != -1)
            {
                // Update state to reflect the instance is gone
                currentPrefabIndex = -1;
            }
                
            int nextIndex = (currentPrefabIndex + 1) % prefabLibrary.Count;
            return SetCurrentPrefab(nextIndex);
        }
        
        /// <summary>
        /// Displays the previous prefab in the library
        /// </summary>
        /// <returns>The newly instantiated GameObject or null if the library is empty</returns>
        public GameObject Previous()
        {
            if (prefabLibrary.Count == 0)
                return null;
            
            // Check if our instance was destroyed externally
            if (currentInstance == null && currentPrefabIndex != -1)
            {
                // Update state to reflect the instance is gone
                currentPrefabIndex = -1;
            }
                
            int prevIndex = currentPrefabIndex <= 0 ? 
                prefabLibrary.Count - 1 : currentPrefabIndex - 1;
            return SetCurrentPrefab(prevIndex);
        }
        
        /// <summary>
        /// Displays a specific prefab by its index in the library
        /// </summary>
        /// <param name="index">Index of the prefab to display</param>
        /// <returns>The newly instantiated GameObject or null if the index is invalid</returns>
        public GameObject SetCurrentPrefab(int index)
        {
            if (prefabLibrary.Count == 0)
            {
                Debug.LogWarning($"[{nameof(PrefabExhibitor)}] Cannot set prefab: library is empty.");
                return null;
            }
                
            if (index < 0 || index >= prefabLibrary.Count)
            {
                Debug.LogError($"[{nameof(PrefabExhibitor)}] Invalid prefab index: {index}. Valid range is 0-{prefabLibrary.Count-1}");
                return null;
            }
            
            // If we're already showing this prefab and it still exists, do nothing
            if (currentPrefabIndex == index && currentInstance != null)
                return currentInstance;
            
            // Clear the current instance if there is one
            ClearActive();
            
            // Instantiate the new prefab
            currentPrefabIndex = index;
            GameObject prefab = prefabLibrary[currentPrefabIndex];
            currentInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            
            // Invoke the change event
            onPrefabChanged.Invoke(currentInstance);
            
            return currentInstance;
        }
        
        /// <summary>
        /// Displays a specific prefab by direct reference
        /// </summary>
        /// <param name="prefab">Reference to the prefab to display</param>
        /// <returns>The newly instantiated GameObject or null if the prefab is not in the library</returns>
        public GameObject SetCurrentPrefab(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError($"[{nameof(PrefabExhibitor)}] Cannot set null prefab.");
                return null;
            }
            
            int index = prefabLibrary.IndexOf(prefab);
            if (index >= 0)
            {
                return SetCurrentPrefab(index);
            }
            else
            {
                Debug.LogError($"[{nameof(PrefabExhibitor)}] Prefab not found in library: {prefab.name}");
                return null;
            }
        }

        /// <summary>
        /// Displays the first prefab in the library
        /// </summary>
        /// <returns>The newly instantiated GameObject or null if the library is empty</returns>
        public GameObject First()
        {
            if (prefabLibrary.Count == 0)
                return null;
                
            return SetCurrentPrefab(0);
        }

        /// <summary>
        /// Displays the last prefab in the library
        /// </summary>
        /// <returns>The newly instantiated GameObject or null if the library is empty</returns>
        public GameObject Last()
        {
            if (prefabLibrary.Count == 0)
                return null;
                
            return SetCurrentPrefab(prefabLibrary.Count - 1);
        }

        /// <summary>
        /// Displays a random prefab from the library
        /// </summary>
        /// <returns>The newly instantiated GameObject or null if the library is empty</returns>
        public GameObject Random()
        {
            if (prefabLibrary.Count == 0)
                return null;
                
            int randomIndex = UnityEngine.Random.Range(0, prefabLibrary.Count);
            return SetCurrentPrefab(randomIndex);
        }

        #endregion
    }
}