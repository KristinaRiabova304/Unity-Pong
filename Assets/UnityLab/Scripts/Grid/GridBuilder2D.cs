using UnityEngine;

namespace UnityLab.Components
{
    [AddComponentMenu("Unity Lab/Grid/Grid Builder 2D")]
    public class GridBuilder2D : MonoBehaviour
    {
        [Header("Grid")]
        [Tooltip("Number of cells along the X axis.")]
        [SerializeField, Min(1)]
        private int width = 10;

        [Tooltip("Number of cells along the Y axis.")]
        [SerializeField, Min(1)]
        private int height = 10;

        [Tooltip("World size of one cell.")]
        [SerializeField, Min(0.01f)]
        private float cellSize = 1f;

        [Tooltip("World position of cell (0,0), at the bottom-left corner of the grid. Leave empty to use this object.")]
        [SerializeField]
        private Transform originTransform;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;

        public Transform Origin => originTransform != null ? originTransform : transform;

        public Vector3 CellToWorld(Vector2Int cell)
        {
            Vector3 origin = Origin.position;
            return origin + new Vector3((cell.x + 0.5f) * cellSize, (cell.y + 0.5f) * cellSize, 0f);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            Vector3 local = worldPosition - Origin.position;
            int x = Mathf.FloorToInt(local.x / cellSize);
            int y = Mathf.FloorToInt(local.y / cellSize);
            return new Vector2Int(x, y);
        }

        public bool IsInsideGrid(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
        }

        private void OnDrawGizmos()
        {
            Transform origin = Origin;
            if (origin == null || cellSize <= 0f || width <= 0 || height <= 0)
            {
                return;
            }

            Vector3 originPosition = origin.position;

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 center = originPosition + new Vector3((x + 0.5f) * cellSize, (y + 0.5f) * cellSize, 0f);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0f));
                }
            }

            Gizmos.color = Color.yellow;
            Vector3 originCellCenter = originPosition + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0f);
            Gizmos.DrawWireCube(originCellCenter, new Vector3(cellSize, cellSize, 0f));
            Gizmos.DrawSphere(originPosition, cellSize * 0.08f);

            Gizmos.color = Color.cyan;
            Vector3 gridSize = new Vector3(width * cellSize, height * cellSize, 0f);
            Vector3 gridCenter = originPosition + gridSize * 0.5f;
            Gizmos.DrawWireCube(gridCenter, gridSize);
        }
    }
}
