using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Interaction/Collectible")]
    [RequireComponent(typeof(Collider2D))]
    public class Collectible : MonoBehaviour, IPointsSource
    {
        [Header("Collection")]
        [Tooltip("Points awarded when a collector touches this object. Use 0 for no score.")]
        [SerializeField]
        private int points = 1;

        [Tooltip("Layers that can collect this object, usually the player. This object's Collider2D should have Is Trigger enabled.")]
        [SerializeField]
        private LayerMask collectorLayer = ~0;

        public event Action<int> PointsAwarded;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & collectorLayer) == 0)
            {
                return;
            }

            if (points != 0)
            {
                PointsAwarded?.Invoke(points);
            }

            Destroy(gameObject);
        }
    }
}
