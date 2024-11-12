

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPositions;
    private Tilemap m_WalkableTilemap;
    private Tilemap m_OverlayTilemap;
    private Sprite m_PlaceholderTileSprite;

    public PlacementProcess(
        BuildActionSO buildAction,
        Tilemap walkableTilemap,
        Tilemap overlayTilemap
    )
    {
        m_PlaceholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");
        m_BuildAction = buildAction;
        m_WalkableTilemap = walkableTilemap;
        m_OverlayTilemap = overlayTilemap;
    }

    public void Update()
    {
        if (m_PlacementOutline != null)
        {
            HighlightTiles(m_PlacementOutline.transform.position);
        }

        if (HvoUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutline.transform.position = SnapToGrid(worldPosition);
        }
    }

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999;
        renderer.color = new Color(1, 1, 1, 0.5f);
        renderer.sprite = m_BuildAction.PlacementSprite;

    }

    Vector3 SnapToGrid(Vector3 worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), 0);
    }

    void HighlightTiles(Vector3 outlinePosition)
    {
        Vector3Int buildingSize = m_BuildAction.BuildingSize;
        Vector3 pivotPosition = outlinePosition + m_BuildAction.OriginOffset;

        ClearHighlights();
        m_HighlightPositions = new Vector3Int[buildingSize.x * buildingSize.y];

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_HighlightPositions[x + y * buildingSize.x] = new Vector3Int((int)pivotPosition.x + x, (int)pivotPosition.y + y, 0);
            }
        }

        foreach (var tilePosition in m_HighlightPositions)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_PlaceholderTileSprite;
            tile.color = new Color(0, 0.8f, 1, 0.4f);
            m_OverlayTilemap.SetTile(tilePosition, tile);
        }
    }

    void ClearHighlights()
    {
        if (m_HighlightPositions == null) return;

        foreach (var tilePosition in m_HighlightPositions)
        {
            m_OverlayTilemap.SetTile(tilePosition, null);
        }
    }
}
