using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Combat/Damage On Collision")]
    public class DamageOnCollision : MonoBehaviour
    {
        [Header("Damage")]
        [Tooltip("How much health is removed from the target.")]
        [SerializeField, Min(0)]
        private int damage = 1;

        [Tooltip("Layers that can receive this damage, usually the player or enemies.")]
        [SerializeField]
        private LayerMask targetLayer = ~0;

        [Tooltip("If enabled, this object is destroyed after it successfully deals damage.")]
        [SerializeField]
        private bool destroySelfOnHit;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryDamage(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void TryDamage(Collider2D other)
        {
            if (other == null)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & targetLayer) == 0)
            {
                return;
            }

            Health health = other.GetComponentInParent<Health>();
            if (health == null)
            {
                return;
            }

            health.TakeDamage(damage);

            if (destroySelfOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
