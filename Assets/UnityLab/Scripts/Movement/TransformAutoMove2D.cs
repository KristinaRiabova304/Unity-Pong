using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Transform Auto Move 2D")]
    public class TransformAutoMove2D : MonoBehaviour
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

        private float currentSpeed;

        private void Awake()
        {
            currentSpeed = speed;
        }

        private void Update()
        {
            if (speedIncreasePerSecond > 0f)
            {
                currentSpeed += speedIncreasePerSecond * Time.deltaTime;
                if (maximumSpeed >= 0f)
                {
                    currentSpeed = Mathf.Min(currentSpeed, maximumSpeed);
                }
            }

            Vector3 moveDirection = direction.sqrMagnitude > 0f ? (Vector3)direction.normalized : Vector3.zero;
            if (useLocalDirection)
            {
                moveDirection = transform.TransformDirection(moveDirection);
            }

            transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
    }
}
