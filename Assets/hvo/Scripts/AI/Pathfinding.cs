

using UnityEngine;

public class Pathfinding
{
    private int m_Width;
    private int m_Height;
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
        InitializeGrid(bounds.min);
    }

    void InitializeGrid(Vector3Int offset)
    {
        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                var nodePosition = new Vector3(x + offset.x, y + offset.y);
                var node = new Node(nodePosition.x, nodePosition.y, true);
                m_Grid[x, y] = node;

                Debug.Log($"Node x: {x}, y: {y} | Position: Vector2({node.x}, {node.y}) | W: {node.isWalkable}");
            }
        }
    }
}
