using System;
using TMPro;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Game/Game Timer")]
    public class GameTimer : MonoBehaviour
    {
        [Header("Timer")]
        [Tooltip("Countdown length in seconds.")]
        [SerializeField, Min(0.01f)]
        private float duration = 60f;

        [Tooltip("Text that shows the time remaining.")]
        [SerializeField]
        private TMP_Text timerText;

        public event Action TimerFinished;

        private float remaining;
        private bool finished;

        private void Start()
        {
            remaining = duration;
            UpdateTimerText();
        }

        private void Update()
        {
            if (finished)
            {
                return;
            }

            remaining -= Time.deltaTime;
            if (remaining <= 0f)
            {
                remaining = 0f;
                finished = true;
                UpdateTimerText();
                TimerFinished?.Invoke();
                return;
            }

            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(remaining).ToString();
            }
        }
    }
}
