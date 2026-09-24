using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Combat/Health")]
    public class Health : MonoBehaviour, IPointsSource
    {
        [Header("Health")]
        [Tooltip("Hit points at the start of the game.")]
        [SerializeField, Min(1)]
        private int maxHealth = 3;

        [Tooltip("If enabled, the object is destroyed when health reaches zero.")]
        [SerializeField]
        private bool destroyOnDeath = true;

        [Tooltip("Points awarded when this object dies. Use 0 for no score.")]
        [SerializeField]
        private int pointsOnDeath;

        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<int, int> HealthChanged;
        public event Action<Health> Died;
        public event Action<int> PointsAwarded;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            IsDead = false;
        }

        private void Start()
        {
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead || damage <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            if (pointsOnDeath != 0)
            {
                PointsAwarded?.Invoke(pointsOnDeath);
            }

            Died?.Invoke(this);

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
