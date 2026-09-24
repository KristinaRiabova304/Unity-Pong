using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Rotate And Thrust 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RotateAndThrust2D : MonoBehaviour
    {
        [Header("Steering")]
        [Tooltip("How quickly the object turns, in degrees per second.")]
        [SerializeField, Min(0f)]
        private float rotationSpeed = 180f;

        [Tooltip("Forward force applied while thrusting.")]
        [SerializeField, Min(0f)]
        private float thrustForce = 8f;

        [Tooltip("Maximum movement speed. Velocity is clamped to this value.")]
        [SerializeField, Min(0f)]
        private float maximumSpeed = 8f;

        [Tooltip("WASD uses A/D to turn and W to thrust. Arrows uses Left/Right to turn and Up to thrust.")]
        [SerializeField]
        private InputScheme inputScheme = InputScheme.WASD;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float rotateInput = LabInput.GetHorizontal(inputScheme);
            body.MoveRotation(body.rotation - rotateInput * rotationSpeed * Time.fixedDeltaTime);

            if (LabInput.GetVertical(inputScheme) > 0f)
            {
                body.AddForce(transform.right * thrustForce);
            }

            if (body.linearVelocity.magnitude > maximumSpeed)
            {
                body.linearVelocity = body.linearVelocity.normalized * maximumSpeed;
            }
        }
    }
}
