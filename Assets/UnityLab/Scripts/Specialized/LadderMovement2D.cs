using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Specialized/Ladder Movement 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class LadderMovement2D : MonoBehaviour
    {
        [Header("Climbing")]
        [Tooltip("Layers that count as ladders. Ladder colliders should usually be triggers.")]
        [SerializeField]
        private LayerMask ladderLayer;

        [Tooltip("How fast the character climbs while overlapping a ladder.")]
        [SerializeField, Min(0f)]
        private float climbSpeed = 4f;

        [Tooltip("WASD uses W/S. Arrows uses Up/Down.")]
        [SerializeField]
        private InputScheme inputScheme = InputScheme.WASD;

        private Rigidbody2D body;
        private Collider2D cachedCollider;
        private float originalGravityScale;
        private readonly List<Collider2D> overlapResults = new List<Collider2D>();

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            cachedCollider = GetComponent<Collider2D>();
            originalGravityScale = body.gravityScale;
        }

        private void OnDisable()
        {
            if (body != null)
            {
                body.gravityScale = originalGravityScale;
            }
        }

        private void FixedUpdate()
        {
            if (IsOnLadder())
            {
                body.gravityScale = 0f;
                Vector2 velocity = body.linearVelocity;
                velocity.y = LabInput.GetVertical(inputScheme) * climbSpeed;
                body.linearVelocity = velocity;
            }
            else
            {
                body.gravityScale = originalGravityScale;
            }
        }

        private bool IsOnLadder()
        {
            if (cachedCollider != null)
            {
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(ladderLayer);
                filter.useTriggers = true;
                overlapResults.Clear();
                return cachedCollider.Overlap(filter, overlapResults) > 0;
            }

            return Physics2D.OverlapCircle(transform.position, 0.1f, ladderLayer) != null;
        }
    }
}
