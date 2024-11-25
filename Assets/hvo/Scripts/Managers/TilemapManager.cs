
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapManager: SingletonManager<TilemapManager>
{
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemaps;

    public Tilemap PathfindingTilemap => m_WalkableTilemap;

    private Pathfinding m_Pathfinding;

    void Start()
    {
        m_Pathfinding = new Pathfinding(
            this
        );
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        return m_Pathfinding.FindPath(startPosition, endPosition);
    }

    public Node FindNode(Vector3 position)
    {
        return m_Pathfinding.FindNode(position);
    }

    public bool CanWalkAtTile(Vector3Int tilePosition)
    {
        return
            m_WalkableTilemap.HasTile(tilePosition) &&
            !IsInUnreachableTilemap(tilePosition) &&
            !IsBlockedByBuilding(tilePosition);
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

    public bool IsBlockedByBuilding(Vector3Int tilePosition)
    {
        Vector3 worldPosition = m_WalkableTilemap.CellToWorld(tilePosition) + m_WalkableTilemap.cellSize / 2;
        int buildingLayerMask = 1 << LayerMask.NameToLayer("Building");

        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition, buildingLayerMask);
        return colliders.Length > 0;
    }

    public bool IsBlockedByGameobject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize * 0.5f, 0);

        foreach (var collider in colliders)
        {
            var layer = collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Player") || layer == LayerMask.NameToLayer("Building"))
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
