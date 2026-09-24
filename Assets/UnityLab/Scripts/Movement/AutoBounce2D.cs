using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Auto Bounce 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AutoBounce2D : MonoBehaviour
    {
        [Header("Bounce")]
        [Tooltip("Upward speed applied when bouncing off a platform.")]
        [SerializeField, Min(0f)]
        private float bounceForce = 10f;

        [Tooltip("Layers that this object can bounce on.")]
        [SerializeField]
        private LayerMask platformLayer = ~0;

        [Tooltip("If enabled, bounce only when the object is falling or standing still vertically.")]
        [SerializeField]
        private bool onlyWhileFalling = true;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryBounce(collision.gameObject);
        }

        private void TryBounce(GameObject other)
        {
            if (((1 << other.layer) & platformLayer) == 0)
            {
                return;
            }

            if (onlyWhileFalling && body.linearVelocity.y > 0.01f)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.y = bounceForce;
            body.linearVelocity = velocity;
        }
    }
}
