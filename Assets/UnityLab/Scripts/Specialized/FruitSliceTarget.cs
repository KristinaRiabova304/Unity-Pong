using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Specialized/Fruit Slice Target")]
    [RequireComponent(typeof(Collider2D))]
    public class FruitSliceTarget : MonoBehaviour, ISliceTarget, IPointsSource
    {
        [Header("Slice")]
        [Tooltip("Points awarded when this object is sliced. Use 0 for no score.")]
        [SerializeField]
        private int points = 1;

        public event Action<int> PointsAwarded;

        public void Slice()
        {
            if (points != 0)
            {
                PointsAwarded?.Invoke(points);
            }

            Destroy(gameObject);
        }
    }
}
