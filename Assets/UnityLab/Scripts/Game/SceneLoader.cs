using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Scene Loader")]
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene")]
        [Tooltip("Exact scene name from File > Build Settings. Leave empty if Reload Current Scene is enabled.")]
        [SerializeField]
        private string sceneName;

        [Tooltip("If enabled, the current scene is reloaded instead of loading Scene Name.")]
        [SerializeField]
        private bool reloadCurrentScene;

        [Tooltip("Seconds to wait before loading. Uses unscaled time, so it still works after the game pauses.")]
        [SerializeField, Min(0f)]
        private float delay;

        [Header("Trigger")]
        [Tooltip("If enabled, a matching collider entering this object loads the scene. The collider should be a trigger.")]
        [SerializeField]
        private bool loadOnTrigger;

        [Tooltip("Layers that can start the load, usually the player.")]
        [SerializeField]
        private LayerMask triggerLayer = ~0;

        private bool loading;

        public void Load()
        {
            if (loading)
            {
                return;
            }

            loading = true;
            StartCoroutine(LoadRoutine());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!loadOnTrigger || other == null)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & triggerLayer) == 0)
            {
                return;
            }

            Load();
        }

        private IEnumerator LoadRoutine()
        {
            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            Time.timeScale = 1f;
            if (reloadCurrentScene || string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield break;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
