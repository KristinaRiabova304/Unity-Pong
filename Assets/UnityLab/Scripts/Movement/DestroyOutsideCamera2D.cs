using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Destroy Outside Camera 2D")]
    public class DestroyOutsideCamera2D : MonoBehaviour
    {
        [Header("Bounds")]
        [Tooltip("Camera used to test visibility. Leave empty to use the Main Camera.")]
        [SerializeField]
        private Camera targetCamera;

        [Tooltip("Extra viewport space before destroying the object. 0.1 means 10% of the screen.")]
        [SerializeField, Min(0f)]
        private float padding = 0.1f;

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

            if (targetCamera == null)
            {
                return;
            }

            Vector3 viewport = targetCamera.WorldToViewportPoint(transform.position);
            bool outside =
                viewport.x < -padding ||
                viewport.x > 1f + padding ||
                viewport.y < -padding ||
                viewport.y > 1f + padding ||
                viewport.z < 0f;

            if (outside)
            {
                Destroy(gameObject);
            }
        }
    }
}
