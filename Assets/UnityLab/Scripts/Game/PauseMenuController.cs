using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Pause Menu Controller")]
    public class PauseMenuController : MonoBehaviour
    {
        [Tooltip("Panel with Resume / Main Menu shown while paused.")]
        [SerializeField]
        private GameObject pausePanel;

        [Tooltip("Key that toggles the pause panel.")]
        [SerializeField]
        private KeyCode pauseKey = KeyCode.Escape;

        [Tooltip("Scene loaded by Main Menu. Must match a scene name in Build Settings.")]
        [SerializeField]
        private string mainMenuSceneName = "MainMenu";

        [Tooltip("Other panel that already paused the game (for example a win screen). Pause is ignored while it is active.")]
        [SerializeField]
        private GameObject blockingPanel;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (blockingPanel != null && blockingPanel.activeSelf)
            {
                return;
            }

            if (Input.GetKeyDown(pauseKey))
            {
                if (IsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Pause()
        {
            if (blockingPanel != null && blockingPanel.activeSelf)
            {
                return;
            }

            IsPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
