using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Interaction/Interactable Target")]
    public class InteractableTarget : MonoBehaviour, IInteractable, IPointsSource
    {
        public enum InteractionResult
        {
            Destroy,
            Hide
        }

        [Header("Interaction")]
        [Tooltip("Points awarded when this object is clicked or otherwise interacted with. Use 0 for no score.")]
        [SerializeField]
        private int points = 1;

        [Tooltip("Destroy removes the object. Hide turns it off so it can be used again later.")]
        [SerializeField]
        private InteractionResult interactionResult = InteractionResult.Destroy;

        public event Action<int> PointsAwarded;

        public void Interact()
        {
            if (points != 0)
            {
                PointsAwarded?.Invoke(points);
            }

            if (interactionResult == InteractionResult.Hide)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
