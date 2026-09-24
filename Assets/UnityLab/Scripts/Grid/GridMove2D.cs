using System;
using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Grid/Grid Move 2D")]
    public class GridMove2D : MonoBehaviour
    {
        [Header("Grid")]
        [Tooltip("The grid this object walks on. Drag the object that has Grid Builder 2D.")]
        [SerializeField]
        private GridBuilder2D gridBuilder;

        [Tooltip("WASD or Arrows. Diagonals are ignored; only one direction is used at a time.")]
        [SerializeField]
        private InputScheme inputScheme = InputScheme.WASD;

        [Tooltip("Seconds taken to slide from one cell to the next. 0 means instant.")]
        [SerializeField, Min(0f)]
        private float moveDuration = 0.12f;

        [Tooltip("If enabled, the object snaps to the center of its starting cell when the game starts.")]
        [SerializeField]
        private bool snapToCellOnStart = true;

        public Vector2Int PreviousCell { get; private set; }
        public Vector2Int CurrentCell { get; private set; }
        public GridBuilder2D Grid => gridBuilder;

        public event Action<Vector2Int, Vector2Int> CellMoveRequested;
        public event Action<Vector2Int, Vector2Int> CellMoved;

        private bool moving;
        private bool moveCancelled;
        private Vector3 moveStart;
        private Vector3 moveEnd;
        private float moveTime;
        private Vector2Int pendingCell;

        private void Start()
        {
            if (gridBuilder == null)
            {
                return;
            }

            CurrentCell = gridBuilder.WorldToCell(transform.position);
            PreviousCell = CurrentCell;
            if (snapToCellOnStart && gridBuilder.IsInsideGrid(CurrentCell))
            {
                transform.position = gridBuilder.CellToWorld(CurrentCell);
            }
        }

        private void Update()
        {
            if (gridBuilder == null)
            {
                return;
            }

            if (moving)
            {
                UpdateMove();
                return;
            }

            TryStartMove();
        }

        public void CancelMove()
        {
            moveCancelled = true;
        }

        private void TryStartMove()
        {
            Vector2Int delta = ReadDirection();
            if (delta == Vector2Int.zero)
            {
                return;
            }

            Vector2Int targetCell = CurrentCell + delta;
            if (!gridBuilder.IsInsideGrid(targetCell))
            {
                return;
            }

            moveCancelled = false;
            CellMoveRequested?.Invoke(CurrentCell, targetCell);
            if (moveCancelled)
            {
                return;
            }

            pendingCell = targetCell;
            moveStart = transform.position;
            moveEnd = gridBuilder.CellToWorld(targetCell);
            moveTime = 0f;
            moving = true;

            if (moveDuration <= 0f)
            {
                CompleteMove();
            }
        }

        private void UpdateMove()
        {
            moveTime += Time.deltaTime;
            float t = moveDuration <= 0f ? 1f : Mathf.Clamp01(moveTime / moveDuration);
            transform.position = Vector3.Lerp(moveStart, moveEnd, t);
            if (t >= 1f)
            {
                CompleteMove();
            }
        }

        private void CompleteMove()
        {
            moving = false;
            transform.position = moveEnd;
            PreviousCell = CurrentCell;
            CurrentCell = pendingCell;
            CellMoved?.Invoke(PreviousCell, CurrentCell);
        }

        private Vector2Int ReadDirection()
        {
            Vector2 input = LabInput.GetAxes(inputScheme);
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y) && Mathf.Abs(input.x) > 0.1f)
            {
                return new Vector2Int(input.x > 0f ? 1 : -1, 0);
            }

            if (Mathf.Abs(input.y) > 0.1f)
            {
                return new Vector2Int(0, input.y > 0f ? 1 : -1);
            }

            return Vector2Int.zero;
        }
    }
}
