using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Combat/Obstacle")]
    public class Obstacle : MonoBehaviour
    {
        [Header("Obstacle")]
        [Tooltip("Layers that trigger this obstacle, usually the player.")]
        [SerializeField]
        private LayerMask targetLayer = ~0;

        [Tooltip("What Game Controller should do. Game Over shows the lose panel. Respawn reloads the scene. Damage removes health. This component only reports the choice.")]
        [SerializeField]
        private ObstacleResponse response = ObstacleResponse.GameOver;

        [Tooltip("Used only when Response is Damage. How much health to remove.")]
        [SerializeField, Min(0)]
        private int damageAmount = 1;

        public ObstacleResponse Response => response;
        public int DamageAmount => damageAmount;

        public event Action<ObstacleHit> Hit;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryReport(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryReport(other.gameObject);
        }

        private void TryReport(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (((1 << target.layer) & targetLayer) == 0)
            {
                return;
            }

            Hit?.Invoke(new ObstacleHit(this, target, response, damageAmount));
        }
    }
}
