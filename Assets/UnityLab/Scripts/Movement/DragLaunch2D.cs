using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Drag Launch 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class DragLaunch2D : MonoBehaviour
    {
        [Header("Launch")]
        [Tooltip("How far the object can be pulled from its starting point.")]
        [SerializeField, Min(0f)]
        private float maximumDragDistance = 3f;

        [Tooltip("How strongly the object is thrown when you release the mouse.")]
        [SerializeField, Min(0f)]
        private float launchPower = 8f;

        [Tooltip("If enabled, the object can be launched only once.")]
        [SerializeField]
        private bool launchOnce = true;

        private Rigidbody2D body;
        private Collider2D cachedCollider;
        private Camera cachedCamera;
        private bool dragging;
        private bool launched;
        private Vector3 dragOrigin;
        private RigidbodyType2D originalBodyType;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            cachedCollider = GetComponent<Collider2D>();
            cachedCamera = Camera.main;
        }

        private void Update()
        {
            if (launched && launchOnce)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) && WasClicked())
            {
                BeginDrag();
            }

            if (dragging && Input.GetMouseButton(0))
            {
                UpdateDrag();
            }

            if (dragging && Input.GetMouseButtonUp(0))
            {
                ReleaseDrag();
            }
        }

        private bool WasClicked()
        {
            Vector2 worldPoint = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(worldPoint);
            return hit == cachedCollider;
        }

        private void BeginDrag()
        {
            dragging = true;
            dragOrigin = transform.position;
            originalBodyType = body.bodyType;
            body.bodyType = RigidbodyType2D.Kinematic;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        private void UpdateDrag()
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 offset = mouseWorld - dragOrigin;
            offset.z = 0f;
            if (offset.magnitude > maximumDragDistance)
            {
                offset = offset.normalized * maximumDragDistance;
            }

            transform.position = dragOrigin + offset;
        }

        private void ReleaseDrag()
        {
            dragging = false;
            Vector3 dragVector = transform.position - dragOrigin;
            dragVector.z = 0f;

            body.bodyType = originalBodyType;
            body.AddForce(-(Vector2)dragVector * launchPower, ForceMode2D.Impulse);

            if (launchOnce)
            {
                launched = true;
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
            }

            if (cachedCamera == null)
            {
                return transform.position;
            }

            Vector3 screen = Input.mousePosition;
            screen.z = cachedCamera.WorldToScreenPoint(transform.position).z;
            Vector3 world = cachedCamera.ScreenToWorldPoint(screen);
            world.z = transform.position.z;
            return world;
        }
    }
}
