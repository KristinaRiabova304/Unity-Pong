using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Score Manager")]
    public class ScoreManager : MonoBehaviour
    {
        [Header("UI")]
        [Tooltip("Text that shows the current score.")]
        [SerializeField]
        private TMP_Text scoreText;

        [Tooltip("Text placed in front of the number, for example 'Score: '.")]
        [SerializeField]
        private string scorePrefix = "Score: ";

        [Header("Win")]
        [Tooltip("Win when the score reaches this value. Use 0 to disable automatic winning.")]
        [SerializeField, Min(0)]
        private int winScore;

        public int Score { get; private set; }

        private readonly HashSet<MonoBehaviour> subscribedSources = new HashSet<MonoBehaviour>();
        private readonly HashSet<MonoBehaviour> subscribedSpawners = new HashSet<MonoBehaviour>();
        private bool hasWon;

        private void Start()
        {
            Score = 0;
            UpdateScoreText();
            SubscribeExistingSources();
            SubscribeSpawners();
        }

        private void OnDestroy()
        {
            UnsubscribeAll();
        }

        private void SubscribeExistingSources()
        {
            MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IPointsSource source)
                {
                    SubscribeSource(source);
                }
            }
        }

        private void SubscribeSpawners()
        {
            PointsSpawner2D[] pointSpawners = FindObjectsByType<PointsSpawner2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < pointSpawners.Length; i++)
            {
                SubscribeSpawner(pointSpawners[i]);
            }

            AreaSpawner2D[] areaSpawners = FindObjectsByType<AreaSpawner2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < areaSpawners.Length; i++)
            {
                SubscribeSpawner(areaSpawners[i]);
            }
        }

        private void SubscribeSpawner(ISpawnSource spawner)
        {
            if (spawner is not MonoBehaviour behaviour || behaviour == null || subscribedSpawners.Contains(behaviour))
            {
                return;
            }

            subscribedSpawners.Add(behaviour);
            spawner.Spawned += OnSpawned;
        }

        private void OnSpawned(GameObject spawned)
        {
            if (spawned == null)
            {
                return;
            }

            MonoBehaviour[] behaviours = spawned.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IPointsSource source)
                {
                    SubscribeSource(source);
                }
            }
        }

        private void SubscribeSource(IPointsSource source)
        {
            if (source is not MonoBehaviour behaviour || behaviour == null || subscribedSources.Contains(behaviour))
            {
                return;
            }

            subscribedSources.Add(behaviour);
            source.PointsAwarded += OnPointsAwarded;
        }

        private void OnPointsAwarded(int points)
        {
            if (this == null)
            {
                return;
            }

            Score += points;
            UpdateScoreText();

            if (!hasWon && winScore > 0 && Score >= winScore)
            {
                hasWon = true;
                GameController controller = FindFirstObjectByType<GameController>();
                if (controller != null)
                {
                    controller.Win();
                }
            }
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = scorePrefix + Score;
            }
        }

        private void UnsubscribeAll()
        {
            foreach (MonoBehaviour behaviour in subscribedSources)
            {
                if (behaviour is IPointsSource source)
                {
                    source.PointsAwarded -= OnPointsAwarded;
                }
            }

            foreach (MonoBehaviour behaviour in subscribedSpawners)
            {
                if (behaviour is ISpawnSource spawner)
                {
                    spawner.Spawned -= OnSpawned;
                }
            }

            subscribedSources.Clear();
            subscribedSpawners.Clear();
        }
    }
}
