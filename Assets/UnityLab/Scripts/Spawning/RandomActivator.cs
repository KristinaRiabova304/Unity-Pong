using System.Collections.Generic;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Spawning/Random Activator")]
    public class RandomActivator : MonoBehaviour
    {
        [Header("Objects")]
        [Tooltip("Existing scene objects to turn on at random. These should usually start disabled.")]
        [SerializeField]
        private GameObject[] objects;

        [Header("Timing")]
        [Tooltip("Shortest wait before activating another object.")]
        [SerializeField, Min(0f)]
        private float minimumDelay = 0.5f;

        [Tooltip("Longest wait before activating another object.")]
        [SerializeField, Min(0f)]
        private float maximumDelay = 1.5f;

        [Tooltip("How long an activated object stays on, in seconds. Use -1 to leave it on.")]
        [SerializeField, Min(-1f)]
        private float activeDuration = 1f;

        [Tooltip("How many listed objects may be active at once. Use -1 for no limit. 0 means nothing will activate.")]
        [SerializeField, Min(-1)]
        private int maximumSimultaneousObjects = 1;

        [Header("Difficulty")]
        [Tooltip("How much both delays shrink after each activation. Use 0 to keep a steady rate.")]
        [SerializeField, Min(0f)]
        private float delayDecreasePerActivation;

        [Tooltip("Smallest delay allowed after difficulty increases.")]
        [SerializeField, Min(0f)]
        private float fastestDelay = 0.2f;

        private readonly List<ActiveEntry> activeEntries = new List<ActiveEntry>();
        private float delayTimer;
        private float currentMinimumDelay;
        private float currentMaximumDelay;

        private void Start()
        {
            currentMinimumDelay = minimumDelay;
            currentMaximumDelay = maximumDelay;
            delayTimer = GetNextDelay();
        }

        private void Update()
        {
            UpdateActiveEntries();

            delayTimer -= Time.deltaTime;
            if (delayTimer > 0f || !CanActivateMore())
            {
                return;
            }

            GameObject next = PickInactiveObject();
            if (next == null)
            {
                delayTimer = GetNextDelay();
                return;
            }

            next.SetActive(true);
            activeEntries.Add(new ActiveEntry
            {
                Target = next,
                Remaining = activeDuration
            });
            IncreaseDifficulty();
            delayTimer = GetNextDelay();
        }

        private void UpdateActiveEntries()
        {
            for (int i = activeEntries.Count - 1; i >= 0; i--)
            {
                ActiveEntry entry = activeEntries[i];
                if (entry.Target == null || !entry.Target.activeSelf)
                {
                    activeEntries.RemoveAt(i);
                    continue;
                }

                if (activeDuration < 0f)
                {
                    continue;
                }

                entry.Remaining -= Time.deltaTime;
                if (entry.Remaining <= 0f)
                {
                    entry.Target.SetActive(false);
                    activeEntries.RemoveAt(i);
                }
                else
                {
                    activeEntries[i] = entry;
                }
            }
        }

        private bool CanActivateMore()
        {
            return maximumSimultaneousObjects < 0 || activeEntries.Count < maximumSimultaneousObjects;
        }

        private GameObject PickInactiveObject()
        {
            if (objects == null || objects.Length == 0)
            {
                return null;
            }

            int start = Random.Range(0, objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject candidate = objects[(start + i) % objects.Length];
                if (candidate != null && !candidate.activeSelf)
                {
                    return candidate;
                }
            }

            return null;
        }

        private float GetNextDelay()
        {
            float min = Mathf.Min(currentMinimumDelay, currentMaximumDelay);
            float max = Mathf.Max(currentMinimumDelay, currentMaximumDelay);
            return Random.Range(min, max);
        }

        private void IncreaseDifficulty()
        {
            if (delayDecreasePerActivation <= 0f)
            {
                return;
            }

            currentMinimumDelay = Mathf.Max(fastestDelay, currentMinimumDelay - delayDecreasePerActivation);
            currentMaximumDelay = Mathf.Max(currentMinimumDelay, currentMaximumDelay - delayDecreasePerActivation);
        }

        private struct ActiveEntry
        {
            public GameObject Target;
            public float Remaining;
        }
    }
}
