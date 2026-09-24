using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Target Counter")]
    public class TargetCounter : MonoBehaviour
    {
        [Header("Targets")]
        [Tooltip("Layers that count as targets, such as bricks or enemies. Do not include the player.")]
        [SerializeField]
        private LayerMask targetLayer;

        [Tooltip("Optional tag filter. Leave empty to ignore tags.")]
        [SerializeField]
        private string targetTag;

        [Tooltip("If enabled, the game is won when no registered targets remain.")]
        [SerializeField]
        private bool winWhenZero = true;

        private readonly HashSet<GameObject> targets = new HashSet<GameObject>();
        private readonly HashSet<Health> subscribedHealth = new HashSet<Health>();
        private readonly HashSet<MonoBehaviour> subscribedSpawners = new HashSet<MonoBehaviour>();
        private bool hasWon;
        private bool everHadTarget;

        private void Start()
        {
            RegisterExistingTargets();
            SubscribeSpawners();
        }

        private void Update()
        {
            RemoveDestroyedTargets();
            TryWin();
        }

        private void OnDestroy()
        {
            foreach (Health health in subscribedHealth)
            {
                if (health != null)
                {
                    health.Died -= OnHealthDied;
                }
            }

            foreach (MonoBehaviour behaviour in subscribedSpawners)
            {
                if (behaviour is ISpawnSource spawner)
                {
                    spawner.Spawned -= OnSpawned;
                }
            }

            subscribedHealth.Clear();
            subscribedSpawners.Clear();
            targets.Clear();
        }

        private void RegisterExistingTargets()
        {
            Health[] healths = FindObjectsByType<Health>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < healths.Length; i++)
            {
                if (Matches(healths[i].gameObject))
                {
                    Register(healths[i].gameObject);
                }
            }

            Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (Matches(colliders[i].gameObject))
                {
                    Register(colliders[i].gameObject);
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

            Health health = spawned.GetComponentInChildren<Health>(true);
            if (health != null && Matches(health.gameObject))
            {
                Register(health.gameObject);
                return;
            }

            if (Matches(spawned))
            {
                Register(spawned);
            }
        }

        private void Register(GameObject target)
        {
            if (target == null || targets.Contains(target) || !Matches(target))
            {
                return;
            }

            targets.Add(target);
            everHadTarget = true;

            Health health = target.GetComponent<Health>();
            if (health != null && subscribedHealth.Add(health))
            {
                health.Died += OnHealthDied;
            }
        }

        private void OnHealthDied(Health health)
        {
            if (health != null)
            {
                targets.Remove(health.gameObject);
            }

            TryWin();
        }

        private void RemoveDestroyedTargets()
        {
            targets.RemoveWhere(target => target == null);
        }

        private bool Matches(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            if (((1 << target.layer) & targetLayer) == 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(targetTag))
            {
                return true;
            }

            try
            {
                return target.CompareTag(targetTag);
            }
            catch (UnityException)
            {
                return false;
            }
        }

        private void TryWin()
        {
            if (!winWhenZero || hasWon || !everHadTarget || targets.Count > 0)
            {
                return;
            }

            hasWon = true;
            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.Win();
            }
        }
    }
}
