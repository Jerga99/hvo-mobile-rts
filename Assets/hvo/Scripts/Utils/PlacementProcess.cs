

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPositions;
    private Tilemap m_WalkableTilemap;

    public PlacementProcess(BuildActionSO buildAction, Tilemap walkableTilemap)
    {
        m_BuildAction = buildAction;
        m_WalkableTilemap = walkableTilemap;
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
        Vector2Int buildingSize = new Vector2Int(2, 3);
        m_HighlightPositions = new Vector3Int[buildingSize.x * buildingSize.y];

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_HighlightPositions[x + y * buildingSize.x] = new Vector3Int((int)outlinePosition.x + x, (int)outlinePosition.y + y, 0);
            }
        }

        foreach (var tilePosition in m_HighlightPositions)
        {
            m_WalkableTilemap.SetTileFlags(tilePosition, TileFlags.None);
            m_WalkableTilemap.SetColor(tilePosition, Color.green);
        }
    }
}
