


using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;
    public float gCost;
    public float hCost;
    public float fCost;
    public Node parent;

    public Node(Vector3Int position, Vector3 cellSize, bool isWalkable)
    {
        x = position.x;
        y = position.y;
        Vector3 halfCellSize = cellSize / 2f;
        var nodeCenterPosition = position + halfCellSize;
        centerX = nodeCenterPosition.x;
        centerY = nodeCenterPosition.y;

        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}
