using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Interaction/Crosshair 2D")]
    public class Crosshair2D : MonoBehaviour
    {
        [Header("Pointer")]
        [Tooltip("Camera used to convert the mouse position into the world. Leave empty to use the Main Camera.")]
        [SerializeField]
        private Camera targetCamera;

        [Tooltip("Optional object that follows the mouse, such as a crosshair sprite.")]
        [SerializeField]
        private Transform crosshairTransform;

        [Tooltip("Layers that can be clicked. Objects on other layers are ignored.")]
        [SerializeField]
        private LayerMask interactionLayerMask = ~0;

        [Tooltip("Mouse button used to interact. 0 is the left button.")]
        [SerializeField]
        private int mouseButton;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            Vector3 worldPosition = GetMouseWorldPosition();
            if (crosshairTransform != null)
            {
                crosshairTransform.position = worldPosition;
            }

            if (!Input.GetMouseButtonDown(mouseButton))
            {
                return;
            }

            Collider2D hit = Physics2D.OverlapPoint(worldPosition, interactionLayerMask);
            if (hit == null)
            {
                return;
            }

            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            interactable?.Interact();
        }

        private Vector3 GetMouseWorldPosition()
        {
            if (targetCamera == null)
            {
                return transform.position;
            }

            Vector3 zSource = crosshairTransform != null ? crosshairTransform.position : transform.position;
            Vector3 screen = Input.mousePosition;
            screen.z = targetCamera.WorldToScreenPoint(zSource).z;
            Vector3 world = targetCamera.ScreenToWorldPoint(screen);
            world.z = zSource.z;
            return world;
        }
    }
}
