using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Spawning/Area Spawner 2D")]
    public class AreaSpawner2D : MonoBehaviour, ISpawnSource
    {
        [Header("What To Spawn")]
        [Tooltip("Prefabs that can be spawned. One is chosen at random each time.")]
        [SerializeField]
        private GameObject[] prefabs;

        [Tooltip("Size of the spawn rectangle, centered on this object.")]
        [SerializeField]
        private Vector2 areaSize = new Vector2(8f, 4f);

        [Header("Timing")]
        [Tooltip("Shortest wait between spawns.")]
        [SerializeField, Min(0f)]
        private float minimumSpawnInterval = 1f;

        [Tooltip("Longest wait between spawns.")]
        [SerializeField, Min(0f)]
        private float maximumSpawnInterval = 2f;

        [Tooltip("How many spawned objects may exist at once. Use -1 for no limit. 0 means nothing will spawn.")]
        [SerializeField, Min(-1)]
        private int maximumActiveObjects = 8;

        [Tooltip("If enabled, each spawned object gets a random Z rotation.")]
        [SerializeField]
        private bool randomRotation;

        [Header("Physics")]
        [Tooltip("Starting Rigidbody2D velocity. Ignored if the prefab has no Rigidbody2D.")]
        [SerializeField]
        private Vector2 initialVelocity;

        [Tooltip("Random extra velocity added in a random 2D direction.")]
        [SerializeField, Min(0f)]
        private float randomVelocityRange;

        [Tooltip("Starting impulse applied to the Rigidbody2D.")]
        [SerializeField]
        private Vector2 initialForce;

        [Tooltip("Random extra impulse added in a random 2D direction.")]
        [SerializeField, Min(0f)]
        private float randomForceRange;

        [Header("Difficulty")]
        [Tooltip("How much both spawn intervals shrink after each spawn. Use 0 to keep a steady rate.")]
        [SerializeField, Min(0f)]
        private float intervalDecreasePerSpawn;

        [Tooltip("Smallest spawn interval allowed after difficulty increases.")]
        [SerializeField, Min(0f)]
        private float fastestInterval = 0.2f;

        [Tooltip("Extra speed added to Initial Velocity after each spawn. Use 0 to keep speed constant.")]
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
            if (prefabs == null || prefabs.Length == 0)
            {
                return false;
            }

            return maximumActiveObjects < 0 || activeObjects.Count < maximumActiveObjects;
        }

        private void Spawn()
        {
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
            if (prefab == null)
            {
                return;
            }

            Vector3 position = transform.position + new Vector3(
                UnityEngine.Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                UnityEngine.Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f),
                0f);
            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f))
                : transform.rotation;

            GameObject instance = Instantiate(prefab, position, rotation);
            Rigidbody2D body = instance.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                Vector2 velocity = initialVelocity;
                if (extraSpeedPerSpawn > 0f && spawnCount > 0)
                {
                    Vector2 boostDirection = initialVelocity.sqrMagnitude > 0.0001f
                        ? initialVelocity.normalized
                        : Vector2.down;
                    velocity += boostDirection * extraSpeedPerSpawn * spawnCount;
                }

                if (randomVelocityRange > 0f)
                {
                    velocity += UnityEngine.Random.insideUnitCircle * randomVelocityRange;
                }

                body.linearVelocity = velocity;

                Vector2 force = initialForce;
                if (randomForceRange > 0f)
                {
                    force += UnityEngine.Random.insideUnitCircle * randomForceRange;
                }

                if (force.sqrMagnitude > 0f)
                {
                    body.AddForce(force, ForceMode2D.Impulse);
                }
            }

            activeObjects.Add(instance);
            Spawned?.Invoke(instance);
            IncreaseDifficulty();
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

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.35f);
            Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
        }
    }
}
