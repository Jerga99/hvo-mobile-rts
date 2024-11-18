
using UnityEngine;
using UnityEngine.Tilemaps;


public class PathfindingManager: SingletonManager<PathfindingManager>
{
    [SerializeField] private Tilemap m_WalkableTilemap;
    private Pathfinding m_Pathfinding;

    void Start()
    {
        var bounds = m_WalkableTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        m_Pathfinding = new Pathfinding(
            width,
            height
        );
    }
}
