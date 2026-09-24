using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Main Menu Controller")]
    public class MainMenuController : MonoBehaviour
    {
        [Tooltip("Exact scene name from File > Build Settings loaded when New Game is pressed.")]
        [SerializeField]
        private string newGameSceneName = "PongArena1";

        public void StartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(newGameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
