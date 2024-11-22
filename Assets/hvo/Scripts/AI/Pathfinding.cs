

using UnityEngine;

public class Pathfinding
{
    private int m_Width;
    private int m_Height;
    private Vector3Int m_GridOffset;
    private Node[,] m_Grid;
    private TilemapManager m_TilemapManager;
    public Node[,] Grid => m_Grid;

    public Pathfinding(TilemapManager tilemapManager)
    {
        m_TilemapManager = tilemapManager;
        tilemapManager.PathfindingTilemap.CompressBounds();
        var bounds = tilemapManager.PathfindingTilemap.cellBounds;
        m_Width = bounds.size.x;
        m_Height = bounds.size.y;
        m_Grid = new Node[m_Width, m_Height];
        m_GridOffset = m_TilemapManager.PathfindingTilemap.cellBounds.min;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        Vector3 cellSize = m_TilemapManager.PathfindingTilemap.cellSize;

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                Vector3Int nodeLeftBottomPosition = new Vector3Int(x + m_GridOffset.x, y + m_GridOffset.y);
                bool isWalkable = m_TilemapManager.CanWalkAtTile(nodeLeftBottomPosition);
                var node = new Node(
                    nodeLeftBottomPosition,
                    cellSize,
                    isWalkable
                );
                m_Grid[x, y] = node;
            }
        }
    }

    public void FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Node startNode = FindNode(startPosition);
        Node endNode = FindNode(endPosition);

        Debug.Log("Start Node: " + startNode);
        Debug.Log("End Node: " + endNode);
    }

    Node FindNode(Vector3 position)
    {
        Vector3Int flooredPosition = new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

        int gridX = flooredPosition.x - m_GridOffset.x;
        int gridY = flooredPosition.y - m_GridOffset.y;

        if (gridX >= 0 && gridX < m_Width && gridY >= 0 && gridY < m_Height)
        {
            return m_Grid[gridX, gridY];
        }

        Debug.Log($"Node not found at position: {position}");
        return null;
    }
}
