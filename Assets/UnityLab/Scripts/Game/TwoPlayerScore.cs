using TMPro;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Two Player Score")]
    public class TwoPlayerScore : MonoBehaviour
    {
        [Header("UI")]
        [Tooltip("Text that shows player 1 score.")]
        [SerializeField]
        private TMP_Text playerOneText;

        [Tooltip("Text that shows player 2 score.")]
        [SerializeField]
        private TMP_Text playerTwoText;

        [Tooltip("Text placed in front of each score number.")]
        [SerializeField]
        private string scorePrefix = "";

        [Header("Win")]
        [Tooltip("Win when either player reaches this score. Use 0 to disable automatic winning.")]
        [SerializeField, Min(0)]
        private int winScore = 5;

        public int PlayerOneScore { get; private set; }
        public int PlayerTwoScore { get; private set; }

        private ScoreZone2D[] zones;
        private bool hasWon;

        private void Start()
        {
            PlayerOneScore = 0;
            PlayerTwoScore = 0;
            UpdateScoreText();
            zones = FindObjectsByType<ScoreZone2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < zones.Length; i++)
            {
                zones[i].Scored += OnScored;
            }
        }

        private void OnDestroy()
        {
            if (zones == null)
            {
                return;
            }

            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i] != null)
                {
                    zones[i].Scored -= OnScored;
                }
            }
        }

        private void OnScored(ScoreSide side, int points)
        {
            if (hasWon)
            {
                return;
            }

            if (side == ScoreSide.PlayerTwo)
            {
                PlayerTwoScore += points;
            }
            else
            {
                PlayerOneScore += points;
            }

            UpdateScoreText();
            TryWin();
        }

        private void TryWin()
        {
            if (winScore <= 0)
            {
                return;
            }

            if (PlayerOneScore < winScore && PlayerTwoScore < winScore)
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

        private void UpdateScoreText()
        {
            if (playerOneText != null)
            {
                playerOneText.text = scorePrefix + PlayerOneScore;
            }

            if (playerTwoText != null)
            {
                playerTwoText.text = scorePrefix + PlayerTwoScore;
            }
        }
    }
}
