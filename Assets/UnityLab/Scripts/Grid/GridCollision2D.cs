using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Grid/Grid Collision 2D")]
    [RequireComponent(typeof(GridMove2D))]
    public class GridCollision2D : MonoBehaviour
    {
        [Header("Obstacles")]
        [Tooltip("Layers that block grid movement, such as walls.")]
        [SerializeField]
        private LayerMask obstacleLayer;

        [Tooltip("How large the overlap box is compared to a cell. 0.8 is a little smaller than the cell so edges are less likely to false-hit.")]
        [SerializeField, Min(0.01f)]
        private float checkSizeMultiplier = 0.8f;

        private GridMove2D gridMove;

        private void Awake()
        {
            gridMove = GetComponent<GridMove2D>();
        }

        private void OnEnable()
        {
            if (gridMove != null)
            {
                gridMove.CellMoveRequested += OnCellMoveRequested;
            }
        }

        private void OnDisable()
        {
            if (gridMove != null)
            {
                gridMove.CellMoveRequested -= OnCellMoveRequested;
            }
        }

        private void OnCellMoveRequested(Vector2Int currentCell, Vector2Int targetCell)
        {
            GridBuilder2D grid = gridMove.Grid;
            if (grid == null)
            {
                return;
            }

            Vector3 center = grid.CellToWorld(targetCell);
            Vector2 size = Vector2.one * grid.CellSize * checkSizeMultiplier;
            Collider2D hit = Physics2D.OverlapBox(center, size, 0f, obstacleLayer);
            if (hit != null && hit.transform != transform && !hit.transform.IsChildOf(transform))
            {
                gridMove.CancelMove();
            }
        }
    }
}
