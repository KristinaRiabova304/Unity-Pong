using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Specialized/Ball Starter 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallStarter2D : MonoBehaviour
    {
        [Header("Launch")]
        [Tooltip("Speed given to the ball when it starts or is reset.")]
        [SerializeField, Min(0f)]
        private float initialSpeed = 6f;

        [Tooltip("If enabled, the launch direction is chosen at random.")]
        [SerializeField]
        private bool randomizeDirection = true;

        [Tooltip("Smallest allowed X amount in the launch direction. Prevents the ball from moving only up and down.")]
        [SerializeField, Min(0f)]
        private float minimumHorizontalComponent = 0.3f;

        [Tooltip("Smallest allowed Y amount in the launch direction. Prevents the ball from moving only left and right.")]
        [SerializeField, Min(0f)]
        private float minimumVerticalComponent = 0.3f;

        private Rigidbody2D body;
        private Vector3 startPosition;
        private float startRotation;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            startPosition = transform.position;
            startRotation = body.rotation;
        }

        private void Start()
        {
            Launch();
        }

        private void FixedUpdate()
        {
            // A kinematic paddle can inject extra energy into a bounce (it moves through the
            // ball's collision unaffected by physics), letting speed climb until the ball tunnels
            // through a wall in a single step. Re-clamp to the launch speed every step instead.
            float speedSqr = body.linearVelocity.sqrMagnitude;
            float maxSpeedSqr = initialSpeed * initialSpeed;
            if (speedSqr > maxSpeedSqr && speedSqr > 0f)
            {
                body.linearVelocity *= Mathf.Sqrt(maxSpeedSqr / speedSqr);
            }
        }

        public void ResetBall()
        {
            transform.position = startPosition;
            body.rotation = startRotation;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            Launch();
        }

        private void Launch()
        {
            body.linearVelocity = ChooseDirection() * initialSpeed;
        }

        private Vector2 ChooseDirection()
        {
            float x = Mathf.Max(minimumHorizontalComponent, 0.01f);
            float y = Mathf.Max(minimumVerticalComponent, 0.01f);

            if (randomizeDirection)
            {
                x = Random.Range(Mathf.Max(minimumHorizontalComponent, 0.01f), 1f);
                y = Random.Range(Mathf.Max(minimumVerticalComponent, 0.01f), 1f);
                if (Random.value < 0.5f)
                {
                    x = -x;
                }

                if (Random.value < 0.5f)
                {
                    y = -y;
                }
            }

            Vector2 direction = new Vector2(x, y);
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = Vector2.one;
            }

            return direction.normalized;
        }
    }
}
