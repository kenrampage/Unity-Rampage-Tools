using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace KenRampage.Utilities.SceneManagement
{
    /// <summary>
    /// Handles asynchronous scene loading operations with error checking and logging.
    /// Provides a clean interface for loading scenes by name.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Scene Management/Scene Loader")]
    public class SceneLoader : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                StartCoroutine(LoadSceneAsync(sceneName));
            }
            else
            {
                Debug.LogError("Scene name is empty!");
            }
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Debug.Log("Scene loaded successfully: " + sceneName);
        }
    }
}