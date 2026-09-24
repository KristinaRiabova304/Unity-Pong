using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Spawning/Points Spawner 2D")]
    public class PointsSpawner2D : MonoBehaviour, ISpawnSource
    {
        [Header("What To Spawn")]
        [Tooltip("Prefabs that can be spawned. One is chosen at random each time.")]
        [SerializeField]
        private GameObject[] prefabs;

        [Tooltip("Transforms that mark legal spawn positions. One is chosen at random each time.")]
        [SerializeField]
        private Transform[] spawnPoints;

        [Header("Timing")]
        [Tooltip("Shortest wait between spawns.")]
        [SerializeField, Min(0f)]
        private float minimumSpawnInterval = 1f;

        [Tooltip("Longest wait between spawns.")]
        [SerializeField, Min(0f)]
        private float maximumSpawnInterval = 2f;

        [Tooltip("How many spawned objects may exist at once. Use -1 for no limit. 0 means nothing will spawn.")]
        [SerializeField, Min(-1)]
        private int maximumActiveObjects = 5;

        [Header("Difficulty")]
        [Tooltip("How much both spawn intervals shrink after each spawn. Use 0 to keep a steady rate.")]
        [SerializeField, Min(0f)]
        private float intervalDecreasePerSpawn;

        [Tooltip("Smallest spawn interval allowed after difficulty increases.")]
        [SerializeField, Min(0f)]
        private float fastestInterval = 0.2f;

        [Tooltip("Extra speed added to a spawned Rigidbody2D after each spawn. Use 0 to keep speed constant.")]
        [SerializeField, Min(0f)]
        private float extraSpeedPerSpawn;

        public event Action<GameObject> Spawned;

        private readonly List<GameObject> activeObjects = new List<GameObject>();
        private float spawnTimer;
        private float currentMinimumInterval;
        private float currentMaximumInterval;
        private int spawnCount;

        private void Start()
        {
            currentMinimumInterval = minimumSpawnInterval;
            currentMaximumInterval = maximumSpawnInterval;
            spawnTimer = GetNextInterval();
        }

        private void Update()
        {
            RemoveDestroyedObjects();
            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f || !CanSpawn())
            {
                return;
            }

            Spawn();
            spawnTimer = GetNextInterval();
        }

        private bool CanSpawn()
        {
            if (prefabs == null || prefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            {
                return false;
            }

            return maximumActiveObjects < 0 || activeObjects.Count < maximumActiveObjects;
        }

        private void Spawn()
        {
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
            Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            if (prefab == null || point == null)
            {
                return;
            }

            GameObject instance = Instantiate(prefab, point.position, point.rotation);
            ApplySpawnSpeed(instance);
            activeObjects.Add(instance);
            Spawned?.Invoke(instance);
            IncreaseDifficulty();
        }

        private void ApplySpawnSpeed(GameObject instance)
        {
            Rigidbody2D body = instance.GetComponent<Rigidbody2D>();
            if (body == null || extraSpeedPerSpawn <= 0f || spawnCount <= 0)
            {
                return;
            }

            Vector2 extra = extraSpeedPerSpawn * spawnCount * Vector2.down;
            if (body.linearVelocity.sqrMagnitude > 0.0001f)
            {
                extra = body.linearVelocity.normalized * extraSpeedPerSpawn * spawnCount;
            }

            body.linearVelocity += extra;
        }

        private void IncreaseDifficulty()
        {
            spawnCount++;
            if (intervalDecreasePerSpawn <= 0f)
            {
                return;
            }

            currentMinimumInterval = Mathf.Max(fastestInterval, currentMinimumInterval - intervalDecreasePerSpawn);
            currentMaximumInterval = Mathf.Max(currentMinimumInterval, currentMaximumInterval - intervalDecreasePerSpawn);
        }

        private void RemoveDestroyedObjects()
        {
            for (int i = activeObjects.Count - 1; i >= 0; i--)
            {
                if (activeObjects[i] == null)
                {
                    activeObjects.RemoveAt(i);
                }
            }
        }

        private float GetNextInterval()
        {
            float min = Mathf.Min(currentMinimumInterval, currentMaximumInterval);
            float max = Mathf.Max(currentMinimumInterval, currentMaximumInterval);
            return UnityEngine.Random.Range(min, max);
        }
    }
}
