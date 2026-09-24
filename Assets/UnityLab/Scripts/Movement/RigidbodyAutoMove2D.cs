using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Rigidbody Auto Move 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyAutoMove2D : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Direction of movement before speed is applied. This is normalized automatically.")]
        [SerializeField]
        private Vector2 direction = Vector2.right;

        [Tooltip("How fast the object moves.")]
        [SerializeField, Min(0f)]
        private float speed = 4f;

        [Tooltip("If enabled, Direction is interpreted in local space (relative to this object's rotation).")]
        [SerializeField]
        private bool useLocalDirection;

        [Header("Difficulty")]
        [Tooltip("How much Speed increases every second. Use 0 to keep a constant speed.")]
        [SerializeField, Min(0f)]
        private float speedIncreasePerSecond;

        [Tooltip("Maximum speed after ramping. Use -1 for no limit.")]
        [SerializeField]
        private float maximumSpeed = -1f;

        private Rigidbody2D body;
        private float currentSpeed;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            currentSpeed = speed;
        }

        private void FixedUpdate()
        {
            if (speedIncreasePerSecond > 0f)
            {
                currentSpeed += speedIncreasePerSecond * Time.fixedDeltaTime;
                if (maximumSpeed >= 0f)
                {
                    currentSpeed = Mathf.Min(currentSpeed, maximumSpeed);
                }
            }

            Vector2 moveDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
            if (useLocalDirection)
            {
                moveDirection = transform.TransformDirection(moveDirection);
            }

            body.linearVelocity = moveDirection * currentSpeed;
        }
    }
}
