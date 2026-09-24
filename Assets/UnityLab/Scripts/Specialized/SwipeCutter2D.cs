using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Specialized/Swipe Cutter 2D")]
    public class SwipeCutter2D : MonoBehaviour
    {
        [Header("Slice")]
        [Tooltip("Camera used to convert the mouse position into the world. Leave empty to use the Main Camera.")]
        [SerializeField]
        private Camera targetCamera;

        [Tooltip("Layers that can be sliced, such as fruit.")]
        [SerializeField]
        private LayerMask sliceLayer = ~0;

        [Tooltip("The mouse must travel at least this far before slicing starts. Prevents a simple click from cutting.")]
        [SerializeField, Min(0f)]
        private float minimumSwipeDistance = 0.15f;

        [Tooltip("If enabled, the swipe path is drawn in the Scene view using Debug.DrawLine.")]
        [SerializeField]
        private bool debugDraw;

        private readonly HashSet<MonoBehaviour> slicedThisSwipe = new HashSet<MonoBehaviour>();
        private Vector3 previousWorldPosition;
        private float travelledDistance;
        private bool swiping;

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

            Vector3 currentWorldPosition = GetMouseWorldPosition();

            if (Input.GetMouseButtonDown(0))
            {
                swiping = true;
                travelledDistance = 0f;
                slicedThisSwipe.Clear();
                previousWorldPosition = currentWorldPosition;
            }

            if (swiping && Input.GetMouseButton(0))
            {
                float step = Vector3.Distance(previousWorldPosition, currentWorldPosition);
                travelledDistance += step;
                if (travelledDistance >= minimumSwipeDistance)
                {
                    SliceAlongPath(previousWorldPosition, currentWorldPosition);
                }

                if (debugDraw)
                {
                    Debug.DrawLine(previousWorldPosition, currentWorldPosition, Color.red);
                }

                previousWorldPosition = currentWorldPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                swiping = false;
                slicedThisSwipe.Clear();
            }
        }

        private void SliceAlongPath(Vector3 from, Vector3 to)
        {
            RaycastHit2D[] hits = Physics2D.LinecastAll(from, to, sliceLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i].collider;
                if (hit == null)
                {
                    continue;
                }

                ISliceTarget target = hit.GetComponentInParent<ISliceTarget>();
                if (target is not MonoBehaviour targetBehaviour || targetBehaviour == null)
                {
                    continue;
                }

                if (!slicedThisSwipe.Add(targetBehaviour))
                {
                    continue;
                }

                target.Slice();
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            if (targetCamera == null)
            {
                return transform.position;
            }

            Vector3 screen = Input.mousePosition;
            screen.z = targetCamera.WorldToScreenPoint(transform.position).z;
            Vector3 world = targetCamera.ScreenToWorldPoint(screen);
            world.z = transform.position.z;
            return world;
        }
    }
}
