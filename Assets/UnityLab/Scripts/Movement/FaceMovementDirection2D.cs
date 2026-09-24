using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Face Movement Direction 2D")]
    public class FaceMovementDirection2D : MonoBehaviour
    {
        private const float MinMovementSqr = 0.0001f;

        [Header("Rotation")]
        [Tooltip("The object that should turn to face the movement direction. Leave empty to rotate this object.")]
        [SerializeField]
        private Transform objectToRotate;

        [Tooltip("Which local axis should point along the movement direction. Right is the red axis. Up is the green axis.")]
        [SerializeField]
        private ForwardAxis2D forwardAxis = ForwardAxis2D.Right;

        private Rigidbody2D body;
        private Vector3 previousPosition;
        private Vector2 lastDirection = Vector2.right;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if (objectToRotate == null)
            {
                objectToRotate = transform;
            }
        }

        private void Start()
        {
            previousPosition = transform.position;
        }

        private void Update()
        {
            Vector2 movement = ReadMovement();
            if (movement.sqrMagnitude >= MinMovementSqr)
            {
                lastDirection = movement.normalized;
            }

            ApplyRotation();
        }

        private Vector2 ReadMovement()
        {
            if (body != null)
            {
                return body.linearVelocity;
            }

            Vector3 displacement = transform.position - previousPosition;
            previousPosition = transform.position;
            if (Time.deltaTime <= 0f)
            {
                return Vector2.zero;
            }

            return displacement / Time.deltaTime;
        }

        private void ApplyRotation()
        {
            if (objectToRotate == null || lastDirection.sqrMagnitude < MinMovementSqr)
            {
                return;
            }

            float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
            if (forwardAxis == ForwardAxis2D.Up)
            {
                angle -= 90f;
            }

            objectToRotate.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
