using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Combat/Shooter 2D")]
    public class Shooter2D : MonoBehaviour
    {
        [Header("Projectile")]
        [Tooltip("Prefab spawned when firing. Give it a Rigidbody2D if it should fly with physics.")]
        [SerializeField]
        private GameObject projectilePrefab;

        [Tooltip("Where the projectile appears. Its rotation chooses the fire direction. Leave empty to use this object.")]
        [SerializeField]
        private Transform spawnPoint;

        [Tooltip("Initial speed given to the projectile's Rigidbody2D.")]
        [SerializeField, Min(0f)]
        private float projectileSpeed = 10f;

        [Tooltip("Seconds to wait between shots.")]
        [SerializeField, Min(0f)]
        private float cooldown = 0.25f;

        [Tooltip("Mouse button used to fire. 0 is the left button.")]
        [SerializeField]
        private int fireMouseButton;

        [Tooltip("If enabled, holding the mouse button fires whenever cooldown allows.")]
        [SerializeField]
        private bool useMouse = true;

        [Tooltip("Optional keyboard key that also fires, for example Space. None means keyboard is ignored.")]
        [SerializeField]
        private KeyCode keyboardKey = KeyCode.None;

        [Tooltip("Which local axis of the Spawn Point is 'forward'. Right is the red axis. Up is the green axis.")]
        [SerializeField]
        private ForwardAxis2D forwardAxis = ForwardAxis2D.Right;

        private float nextFireTime;

        private void Awake()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
        }

        private void Update()
        {
            if ((!WasFireHeld()) || Time.time < nextFireTime)
            {
                return;
            }

            Fire();
        }

        private bool WasFireHeld()
        {
            if (useMouse && Input.GetMouseButton(fireMouseButton))
            {
                return true;
            }

            return keyboardKey != KeyCode.None && Input.GetKey(keyboardKey);
        }

        private void Fire()
        {
            if (projectilePrefab == null || spawnPoint == null)
            {
                return;
            }

            nextFireTime = Time.time + cooldown;
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            Vector3 forward = forwardAxis == ForwardAxis2D.Up ? spawnPoint.up : spawnPoint.right;
            Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.linearVelocity = ((Vector2)forward).normalized * projectileSpeed;
            }
        }
    }
}
