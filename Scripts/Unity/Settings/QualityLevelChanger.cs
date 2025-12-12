using UnityEngine;
using Obvious.Soap;
// TESTs

namespace KenRampage.Unity.Settings
{
    /// <summary>
    /// Manages quality level settings and provides methods to change, increment, or decrement the current quality level.
    /// Updates StringVariable and IntVariable to reflect current quality settings.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Settings/Quality Level Changer")]
    public class QualityLevelChanger : MonoBehaviour
    {
        [SerializeField] private StringVariable _qualityLevelName;
        [SerializeField] private IntVariable _qualityLevelIndex;

        private void Start()
        {
            _qualityLevelName.Value = QualitySettings.names[QualitySettings.GetQualityLevel()];
            _qualityLevelIndex.Value = QualitySettings.GetQualityLevel();
        }

        public void ChangeQualityLevel(int qualityLevelIndex)
        {
            if (qualityLevelIndex >= 0 && qualityLevelIndex < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(qualityLevelIndex, true);
                _qualityLevelName.Value = QualitySettings.names[qualityLevelIndex];
                _qualityLevelIndex.Value = qualityLevelIndex;
            }
        }

        public void IncrementQualityLevel()
        {
            int currentQualityLevelIndex = QualitySettings.GetQualityLevel();
            int newQualityLevelIndex = (currentQualityLevelIndex + 1) % QualitySettings.names.Length;
            ChangeQualityLevel(newQualityLevelIndex);
        }

        public void DecrementQualityLevel()
        {
            int currentQualityLevelIndex = QualitySettings.GetQualityLevel();
            int newQualityLevelIndex = (currentQualityLevelIndex - 1 + QualitySettings.names.Length) % QualitySettings.names.Length;
            ChangeQualityLevel(newQualityLevelIndex);
        }
    }
}