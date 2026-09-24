using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Flap 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Flap2D : MonoBehaviour
    {
        [Header("Flap")]
        [Tooltip("Upward impulse applied each time the player flaps.")]
        [SerializeField, Min(0f)]
        private float flapForce = 6f;

        [Tooltip("If enabled, a left mouse click also flaps.")]
        [SerializeField]
        private bool useMouseClick = true;

        [Tooltip("Keyboard key that flaps. Space is the usual choice.")]
        [SerializeField]
        private KeyCode keyboardKey = KeyCode.Space;

        [Tooltip("If enabled, downward speed is cleared before the flap so each flap feels consistent.")]
        [SerializeField]
        private bool resetVerticalVelocity = true;

        [Tooltip("Maximum downward speed. Prevents the object from falling too fast.")]
        [SerializeField, Min(0f)]
        private float maximumFallSpeed = 12f;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (WasFlapPressed())
            {
                Flap();
            }
        }

        private void FixedUpdate()
        {
            Vector2 velocity = body.linearVelocity;
            if (velocity.y < -maximumFallSpeed)
            {
                velocity.y = -maximumFallSpeed;
                body.linearVelocity = velocity;
            }
        }

        private bool WasFlapPressed()
        {
            if (Input.GetKeyDown(keyboardKey))
            {
                return true;
            }

            return useMouseClick && Input.GetMouseButtonDown(0);
        }

        private void Flap()
        {
            Vector2 velocity = body.linearVelocity;
            if (resetVerticalVelocity)
            {
                velocity.y = 0f;
                body.linearVelocity = velocity;
            }

            body.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        }
    }
}
