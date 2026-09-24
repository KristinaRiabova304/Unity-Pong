using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Score Zone 2D")]
    [RequireComponent(typeof(Collider2D))]
    public class ScoreZone2D : MonoBehaviour
    {
        [Header("Scoring")]
        [Tooltip("Which player receives the points. Used by Two Player Score.")]
        [SerializeField]
        private ScoreSide scoreSide = ScoreSide.PlayerOne;

        [Tooltip("Points awarded when a valid object enters this zone.")]
        [SerializeField]
        private int points = 1;

        [Tooltip("Layers that can score here, usually the ball.")]
        [SerializeField]
        private LayerMask targetLayer = ~0;

        [Header("Ball")]
        [Tooltip("Optional ball to reset after a point. Drag the object that has Ball Starter 2D.")]
        [SerializeField]
        private BallStarter2D ballToReset;

        public ScoreSide ScoreSide => scoreSide;
        public event Action<ScoreSide, int> Scored;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryScore(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryScore(collision.gameObject);
        }

        private void TryScore(GameObject other)
        {
            if (other == null || ((1 << other.layer) & targetLayer) == 0)
            {
                return;
            }

            if (points != 0)
            {
                Scored?.Invoke(scoreSide, points);
            }

            if (ballToReset != null)
            {
                ballToReset.ResetBall();
            }
        }
    }
}
