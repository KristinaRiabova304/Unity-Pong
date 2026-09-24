using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Jump 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Jump2D : MonoBehaviour
    {
        [Header("Jump")]
        [Tooltip("Upward speed applied when the object jumps.")]
        [SerializeField, Min(0f)]
        private float jumpForce = 8f;

        [Tooltip("Empty object placed at the character's feet. Used to test whether the character is standing on ground.")]
        [SerializeField]
        private Transform groundCheckTransform;

        [Tooltip("How far around the ground check point to look for ground.")]
        [SerializeField, Min(0f)]
        private float groundCheckRadius = 0.1f;

        [Tooltip("Layers that count as ground or platforms. Put the player on a different layer so the ground check does not hit the player.")]
        [SerializeField]
        private LayerMask groundLayer = ~0;

        [Tooltip("Key that makes the character jump.")]
        [SerializeField]
        private KeyCode jumpKey = KeyCode.Space;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if (groundCheckTransform == null)
            {
                groundCheckTransform = transform;
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(jumpKey) || !IsGrounded())
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.y = jumpForce;
            body.linearVelocity = velocity;
        }

        private bool IsGrounded()
        {
            if (groundCheckTransform == null)
            {
                return false;
            }

            Collider2D hit = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
            return hit != null;
        }

        private void OnDrawGizmos()
        {
            Transform check = groundCheckTransform != null ? groundCheckTransform : transform;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(check.position, groundCheckRadius);
        }
    }
}
