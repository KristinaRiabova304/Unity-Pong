using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Horizontal Move 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class HorizontalMove2D : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("How fast the object moves horizontally.")]
        [SerializeField, Min(0f)]
        private float speed = 5f;

        [Tooltip("WASD uses A/D. Arrows uses Left/Right.")]
        [SerializeField]
        private InputScheme inputScheme = InputScheme.WASD;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float input = LabInput.GetHorizontal(inputScheme);
            Vector2 velocity = body.linearVelocity;
            velocity.x = input * speed;
            body.linearVelocity = velocity;
        }
    }
}
