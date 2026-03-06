using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;   // matches your floorMap width
    [SerializeField] private int gridHeight = 3;   // matches your floorMap height
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Color gridColor = Color.green;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;

    // Array to store positions of each grid cell
    private Vector3[,] gridPositions;

    // Manual floor map (0 = empty, 1 = floor)
    private int[,] floorMap = new int[,]
    {
        // Top rows are empty for falling/jumping space
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 9 (top)
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 8
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 7
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 6
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 5
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 4
        {0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0}, // row 3 – small platforms
        {0,0,0,0,0,0,1,1,1,0,0,0,1,1,0,0,0,0,0,0}, // row 2 – larger floor + jumps
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, // row 1 – empty for testing jump height
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}  // row 0 – base floor
    };

    private void Awake()
    {
        CreateGrid();
        SpawnFloors();
    }

    /// <summary>
    /// Create the grid positions
    /// </summary>
    private void CreateGrid()
    {
        gridPositions = new Vector3[gridWidth, gridHeight];

        Vector3 origin = transform.position - new Vector3(gridWidth * cellSize / 2f, gridHeight * cellSize / 2f, 0);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridPositions[x, y] = origin + new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, 0);
            }
        }
    }

    /// <summary>
    /// Spawn floor prefabs according to manual floorMap
    /// </summary>
    private void SpawnFloors()
    {
        if (floorPrefab == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Flip y to match bottom-left origin in world space
                int flippedY = gridHeight - 1 - y;

                if (floorMap[y, x] == 1)
                {
                    Vector3 spawnPos = gridPositions[x, flippedY];
                    Instantiate(floorPrefab, spawnPos, Quaternion.identity, transform);
                }
            }
        }
    }

    /// <summary>
    /// Draw the grid in the editor
    /// </summary>
    private void OnDrawGizmos()
    {
        if (gridWidth <= 0 || gridHeight <= 0) return;

        Gizmos.color = gridColor;

        Vector3 origin = transform.position - new Vector3(gridWidth * cellSize / 2f, gridHeight * cellSize / 2f, 0);

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = origin + new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = origin + new Vector3(0, y * cellSize, 0);
            Vector3 end = origin + new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw cell centers
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellCenter = origin + new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, 0);
                Gizmos.DrawSphere(cellCenter, 0.05f);
            }
        }
    }

    /// <summary>
    /// Returns the world position of a grid cell
    /// </summary>
    public Vector3 GetCellPosition(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning("GridManager: Cell index out of bounds!");
            return Vector3.zero;
        }
        return gridPositions[x, y];
    }
}