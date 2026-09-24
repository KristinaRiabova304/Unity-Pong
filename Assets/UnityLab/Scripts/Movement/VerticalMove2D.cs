using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Vertical Move 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class VerticalMove2D : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("How fast the object moves vertically. This replaces the current vertical speed each physics step, so gravity will not pull the object down.")]
        [SerializeField, Min(0f)]
        private float speed = 5f;

        [Tooltip("WASD uses W/S. Arrows uses Up/Down.")]
        [SerializeField]
        private InputScheme inputScheme = InputScheme.WASD;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float input = LabInput.GetVertical(inputScheme);
            Vector2 velocity = body.linearVelocity;
            velocity.y = input * speed;
            body.linearVelocity = velocity;
        }
    }
}
