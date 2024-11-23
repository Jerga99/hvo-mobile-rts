

using System.Collections.Generic;
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

        if (startNode == null || endNode == null)
        {
            Debug.Log("Cannot find the path!");
            return;
        }

        List<Node> openList = new();
        HashSet<Node> closedList = new();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                Debug.Log("Path Found!");
                return;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            Debug.Log("OL: " + string.Join(", ", openList));
            Debug.Log("CL: " + string.Join(", ", closedList));
        }

    }

    Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];

        foreach (Node node in openList)
        {
            if (
                node.fCost < lowestFCostNode.fCost ||
                (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost)
            )
            {
                lowestFCostNode = node;
            }
        }

        return lowestFCostNode;
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

        return null;
    }
}
