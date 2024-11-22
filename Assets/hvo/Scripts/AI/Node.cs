


using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;

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
}
