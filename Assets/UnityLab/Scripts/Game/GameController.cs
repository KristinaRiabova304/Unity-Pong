using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Game Controller")]
    public class GameController : MonoBehaviour
    {
        [Header("Result UI")]
        [Tooltip("Panel shown when the player wins. Leave empty if you do not need a win screen.")]
        [SerializeField]
        private GameObject winPanel;

        [Tooltip("Panel shown when the player loses.")]
        [SerializeField]
        private GameObject resultPanel;

        [Tooltip("Optional text updated with a short win or lose message.")]
        [SerializeField]
        private TMP_Text resultText;

        [Header("Player")]
        [Tooltip("The player object. Used to decide which Obstacle hits count, and to find Health if it is not assigned.")]
        [SerializeField]
        private Transform playerTransform;

        [Tooltip("Player Health used for Damage obstacles and for game over when health reaches zero.")]
        [SerializeField]
        private Health health;

        [Tooltip("Text that shows current and maximum health, for example 3 / 3.")]
        [SerializeField]
        private TMP_Text healthText;

        [Header("Restart")]
        [Tooltip("Key that reloads the current scene.")]
        [SerializeField]
        private KeyCode restartKey = KeyCode.R;

        private bool gameEnded;
        private Obstacle[] obstacles;
        private GameTimer timer;

        private void Awake()
        {
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }

            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }

            if (health == null && playerTransform != null)
            {
                health = playerTransform.GetComponent<Health>();
            }
        }

        private void Start()
        {
            SubscribeHealth();
            SubscribeTimer();
            SubscribeObstacles();
            UpdateHealthText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(restartKey))
            {
                RestartScene();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeHealth();
            UnsubscribeTimer();
            UnsubscribeObstacles();
        }

        public void Win()
        {
            if (gameEnded)
            {
                return;
            }

            gameEnded = true;
            Time.timeScale = 0f;
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
            else if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }

            SetResultMessage("You win!");
        }

        public void GameOver()
        {
            if (gameEnded)
            {
                return;
            }

            gameEnded = true;
            Time.timeScale = 0f;
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }

            SetResultMessage("Game over");
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Respawn()
        {
            RestartScene();
        }

        private void SubscribeHealth()
        {
            if (health == null)
            {
                return;
            }

            health.HealthChanged += OnHealthChanged;
            health.Died += OnHealthDied;
        }

        private void UnsubscribeHealth()
        {
            if (health == null)
            {
                return;
            }

            health.HealthChanged -= OnHealthChanged;
            health.Died -= OnHealthDied;
        }

        private void SubscribeTimer()
        {
            timer = FindFirstObjectByType<GameTimer>();
            if (timer != null)
            {
                timer.TimerFinished += OnTimerFinished;
            }
        }

        private void UnsubscribeTimer()
        {
            if (timer != null)
            {
                timer.TimerFinished -= OnTimerFinished;
            }
        }

        private void SubscribeObstacles()
        {
            obstacles = FindObjectsByType<Obstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacles[i].Hit += OnObstacleHit;
            }
        }

        private void UnsubscribeObstacles()
        {
            if (obstacles == null)
            {
                return;
            }

            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i] != null)
                {
                    obstacles[i].Hit -= OnObstacleHit;
                }
            }
        }

        private void OnObstacleHit(ObstacleHit hit)
        {
            if (gameEnded || !IsPlayerTarget(hit.Target))
            {
                return;
            }

            switch (hit.Response)
            {
                case ObstacleResponse.Respawn:
                    Respawn();
                    break;
                case ObstacleResponse.Damage:
                    if (health != null)
                    {
                        health.TakeDamage(hit.DamageAmount);
                    }

                    break;
                default:
                    GameOver();
                    break;
            }
        }

        private bool IsPlayerTarget(GameObject target)
        {
            if (playerTransform == null || target == null)
            {
                return true;
            }

            Transform targetTransform = target.transform;
            return targetTransform == playerTransform || targetTransform.IsChildOf(playerTransform);
        }

        private void OnHealthChanged(int current, int max)
        {
            UpdateHealthText(current, max);
        }

        private void OnHealthDied(Health _)
        {
            GameOver();
        }

        private void OnTimerFinished()
        {
            GameOver();
        }

        private void UpdateHealthText()
        {
            if (health != null)
            {
                UpdateHealthText(health.CurrentHealth, health.MaxHealth);
            }
        }

        private void UpdateHealthText(int current, int max)
        {
            if (healthText != null)
            {
                healthText.text = current + " / " + max;
            }
        }

        private void SetResultMessage(string title)
        {
            if (resultText == null)
            {
                return;
            }

            TwoPlayerScore versus = FindFirstObjectByType<TwoPlayerScore>();
            if (versus != null)
            {
                string winner = versus.PlayerOneScore > versus.PlayerTwoScore ? "Player 1" : "Player 2";
                resultText.text = winner + " Wins!\n" + versus.PlayerOneScore + " : " + versus.PlayerTwoScore;
                return;
            }

            string extra = "";
            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                extra = "\nScore: " + scoreManager.Score;
            }

            resultText.text = title + extra;
        }
    }
}
