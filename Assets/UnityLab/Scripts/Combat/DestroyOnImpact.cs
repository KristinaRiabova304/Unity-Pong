using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Combat/Destroy On Impact")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DestroyOnImpact : MonoBehaviour
    {
        [Header("Impact")]
        [Tooltip("Collisions slower than this speed are ignored.")]
        [SerializeField, Min(0f)]
        private float minimumImpactSpeed = 3f;

        [Tooltip("Base damage applied when the impact is strong enough.")]
        [SerializeField, Min(0)]
        private int damage = 1;

        [Tooltip("Extra damage added from speed: final damage = Damage + impact speed × this value.")]
        [SerializeField, Min(0f)]
        private float damageMultiplier = 0.5f;

        [Tooltip("Layers that can be damaged by this impact. This component never destroys the target; it only calls Take Damage on Health.")]
        [SerializeField]
        private LayerMask targetLayer = ~0;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            if (impactSpeed < minimumImpactSpeed)
            {
                return;
            }

            if (((1 << collision.gameObject.layer) & targetLayer) == 0)
            {
                return;
            }

            Health health = collision.collider.GetComponentInParent<Health>();
            if (health == null)
            {
                return;
            }

            int appliedDamage = Mathf.Max(0, Mathf.RoundToInt(damage + impactSpeed * damageMultiplier));
            if (appliedDamage <= 0)
            {
                return;
            }

            health.TakeDamage(appliedDamage);
        }
    }
}
