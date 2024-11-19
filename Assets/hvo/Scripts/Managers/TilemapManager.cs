
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapManager: SingletonManager<TilemapManager>
{
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemaps;

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

    public bool CanPlaceTile(Vector3Int tilePosition)
    {
        return
            m_WalkableTilemap.HasTile(tilePosition) &&
            !IsInUnreachableTilemap(tilePosition) &&
            !IsBlockedByGameobject(tilePosition);
    }

    public bool IsInUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach (var tilemap in m_UnreachableTilemaps)
        {
            if (tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    public bool IsBlockedByGameobject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.5f, 0);

        foreach (var collider in colliders)
        {
            var layer = collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void SetTileOverlay(Vector3Int tilePosition, Tile tile)
    {
        m_OverlayTilemap.SetTile(tilePosition, tile);
    }
}
