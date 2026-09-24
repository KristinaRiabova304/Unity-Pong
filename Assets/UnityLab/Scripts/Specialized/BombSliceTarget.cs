using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Specialized/Bomb Slice Target")]
    [RequireComponent(typeof(Collider2D))]
    public class BombSliceTarget : MonoBehaviour, ISliceTarget, IPointsSource
    {
        [Header("Bomb")]
        [Tooltip("Points awarded when sliced. Use a negative value for a penalty. Use 0 for no score change.")]
        [SerializeField]
        private int points;

        [Tooltip("If enabled, slicing this object ends the game.")]
        [SerializeField]
        private bool causeGameOver = true;

        public event Action<int> PointsAwarded;

        public void Slice()
        {
            if (points != 0)
            {
                PointsAwarded?.Invoke(points);
            }

            if (causeGameOver)
            {
                GameController controller = FindFirstObjectByType<GameController>();
                if (controller != null)
                {
                    controller.GameOver();
                }
            }

            Destroy(gameObject);
        }
    }
}
