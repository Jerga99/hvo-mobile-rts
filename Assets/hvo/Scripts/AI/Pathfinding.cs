

using UnityEngine;

public class Pathfinding
{
    private Node[,] m_Grid;
    public Node[,] Grid => m_Grid;

    public Pathfinding
    (
        int width,
        int height
    )
    {
        m_Grid = new Node[width, height];
        Debug.Log(m_Grid.Length);
    }
}
