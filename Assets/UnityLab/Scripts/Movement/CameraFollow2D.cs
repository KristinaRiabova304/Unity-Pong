using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Camera Follow 2D")]
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Follow")]
        [Tooltip("Object the camera should follow. If empty, the first object with Target Tag is used.")]
        [SerializeField]
        private Transform target;

        [Tooltip("Tag used to find a target when Target is empty. Usually Player.")]
        [SerializeField]
        private string targetTag = "Player";

        [Tooltip("Offset from the target position.")]
        [SerializeField]
        private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Tooltip("If enabled, the camera follows left and right movement.")]
        [SerializeField]
        private bool followX = true;

        [Tooltip("If enabled, the camera follows up and down movement.")]
        [SerializeField]
        private bool followY = true;

        private void LateUpdate()
        {
            if (target == null)
            {
                TryFindTarget();
            }

            if (target == null)
            {
                return;
            }

            Vector3 position = transform.position;
            Vector3 desired = target.position + offset;
            if (followX)
            {
                position.x = desired.x;
            }

            if (followY)
            {
                position.y = desired.y;
            }

            position.z = desired.z;
            transform.position = position;
        }

        private void TryFindTarget()
        {
            if (string.IsNullOrWhiteSpace(targetTag))
            {
                return;
            }

            try
            {
                GameObject found = GameObject.FindGameObjectWithTag(targetTag);
                if (found != null)
                {
                    target = found.transform;
                }
            }
            catch (UnityException)
            {
            }
        }
    }
}
