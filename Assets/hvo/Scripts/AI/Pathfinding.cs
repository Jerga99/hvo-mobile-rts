

using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    public Node FindNode(Vector3 position)
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

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Node startNode = FindNode(startPosition);
        Node endNode = FindNode(endPosition);

        if (startNode == null || endNode == null)
        {
            Debug.Log("Cannot find the path!");
            return new List<Vector3>();
        }

        List<Node> openList = new();
        HashSet<Node> closedList = new();

        openList.Add(startNode);

        Node closestNode = startNode;
        var closestDistanceToEnd = GetDistance(closestNode, endNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                var path = RetracePath(startNode, endNode, startPosition);
                ResetNodes(openList, closedList);
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedList.Contains(neighbor)) continue;

                float tentativeG = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (tentativeG < neighbor.gCost || !openList.Contains(neighbor))
                {
                    var distance = GetDistance(neighbor, endNode);
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = distance;
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.parent = currentNode;

                    if (distance < closestDistanceToEnd)
                    {
                        closestNode = neighbor;
                        closestDistanceToEnd = distance;
                    }

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        var unfinshedPath = RetracePath(startNode, closestNode, startPosition);
        ResetNodes(openList, closedList);
        return unfinshedPath;
    }

    public void UpdateNodesInArea(Vector3Int startPosition, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int xPosition = startPosition.x + x;
                int yPosition = startPosition.y + y;

                int gridX = xPosition - m_GridOffset.x;
                int gridY = yPosition - m_GridOffset.y;

                if (gridX >= 0 && gridX < m_Width && gridY >= 0 && gridY < m_Height)
                {
                    Node node = m_Grid[gridX, gridY];
                    Vector3Int cellPosition = new Vector3Int(node.x, node.y, 0);
                    node.isWalkable = m_TilemapManager.CanWalkAtTile(cellPosition);
                }
            }
        }
    }

    void ResetNodes(List<Node> openList, HashSet<Node> closedList)
    {
        foreach(Node node in openList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.parent = null;
        }

        foreach(Node node in closedList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.parent = null;
        }

        openList.Clear();
        closedList.Clear();
    }

    List<Vector3> RetracePath(Node startNode, Node endNode, Vector3 startPosition)
    {
        List<Vector3> path = new();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(new Vector3(currentNode.centerX, currentNode.centerY));
            currentNode = currentNode.parent;
        }

        path.Add(startPosition);
        path.Reverse();

        return path;
    }

    float GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.x + x - m_GridOffset.x;
                int checkY = node.y + y - m_GridOffset.y;

                if (checkX >= 0 && checkX < m_Width && checkY >= 0 && checkY < m_Height)
                {
                    var neighbor = m_Grid[checkX, checkY];
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
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
}
