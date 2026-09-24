using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Movement/Move Towards Target 2D")]
    public class MoveTowardsTarget2D : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Object to chase. If empty, the first object with Target Tag is used.")]
        [SerializeField]
        private Transform target;

        [Tooltip("Tag used to find a target when Target is empty. Usually Player.")]
        [SerializeField]
        private string targetTag = "Player";

        [Header("Movement")]
        [Tooltip("How fast the object moves toward the target.")]
        [SerializeField, Min(0f)]
        private float speed = 3f;

        [Tooltip("Stop moving once this close to the target.")]
        [SerializeField, Min(0f)]
        private float stopDistance = 0.1f;

        private void Update()
        {
            if (target == null)
            {
                TryFindTarget();
            }

            if (target == null)
            {
                return;
            }

            Vector3 toTarget = target.position - transform.position;
            toTarget.z = 0f;
            if (toTarget.magnitude <= stopDistance)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
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
                // The tag is not defined in Tag Manager. Students can add it later.
            }
        }
    }
}
