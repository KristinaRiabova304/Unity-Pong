using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Win Trigger 2D")]
    [RequireComponent(typeof(Collider2D))]
    public class WinTrigger2D : MonoBehaviour
    {
        [Header("Finish")]
        [Tooltip("Layers that can finish the level, usually the player. This collider should have Is Trigger enabled.")]
        [SerializeField]
        private LayerMask targetLayer = ~0;

        private bool hasTriggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasTriggered || other == null)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & targetLayer) == 0)
            {
                return;
            }

            hasTriggered = true;
            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.Win();
            }
        }
    }
}
