using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Waypoint Mover 2D")]
    public class WaypointMover2D : MonoBehaviour
    {
        public enum MovementMode
        {
            Loop,
            PingPong,
            Random
        }

        [Header("Path")]
        [Tooltip("Transforms the object will travel between, in order.")]
        [SerializeField]
        private Transform[] waypoints;

        [Tooltip("How fast the object moves toward the next waypoint.")]
        [SerializeField, Min(0f)]
        private float speed = 3f;

        [Tooltip("Loop repeats from the start. Ping Pong goes back and forth. Random picks a random next point.")]
        [SerializeField]
        private MovementMode movementMode = MovementMode.Loop;

        [Tooltip("Seconds to wait after reaching a waypoint before moving to the next one.")]
        [SerializeField, Min(0f)]
        private float waitAtPoint;

        private int currentIndex;
        private int pingPongDirection = 1;
        private float waitTimer;

        private void Update()
        {
            Transform target = GetCurrentWaypoint();
            if (target == null)
            {
                ChooseNextWaypoint();
                return;
            }

            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if ((transform.position - target.position).sqrMagnitude <= 0.0001f)
            {
                if (waypoints.Length <= 1)
                {
                    return;
                }

                waitTimer = waitAtPoint;
                ChooseNextWaypoint();
            }
        }

        private Transform GetCurrentWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return null;
            }

            if (currentIndex < 0 || currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }

            return waypoints[currentIndex];
        }

        private void ChooseNextWaypoint()
        {
            if (waypoints == null || waypoints.Length <= 1)
            {
                return;
            }

            switch (movementMode)
            {
                case MovementMode.PingPong:
                    AdvancePingPong();
                    break;
                case MovementMode.Random:
                    currentIndex = ChooseRandomIndex();
                    break;
                default:
                    currentIndex = (currentIndex + 1) % waypoints.Length;
                    break;
            }
        }

        private void AdvancePingPong()
        {
            int next = currentIndex + pingPongDirection;
            if (next < 0 || next >= waypoints.Length)
            {
                pingPongDirection = -pingPongDirection;
                next = currentIndex + pingPongDirection;
            }

            currentIndex = Mathf.Clamp(next, 0, waypoints.Length - 1);
        }

        private int ChooseRandomIndex()
        {
            int next = Random.Range(0, waypoints.Length);
            if (waypoints.Length > 1 && next == currentIndex)
            {
                next = (next + 1) % waypoints.Length;
            }

            return next;
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Vector3 previous = transform.position;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Transform point = waypoints[i];
                if (point == null)
                {
                    continue;
                }

                Gizmos.DrawSphere(point.position, 0.08f);
                Gizmos.DrawLine(previous, point.position);
                previous = point.position;
            }

            if (movementMode == MovementMode.Loop && waypoints[0] != null)
            {
                Gizmos.DrawLine(previous, waypoints[0].position);
            }
        }
    }
}
