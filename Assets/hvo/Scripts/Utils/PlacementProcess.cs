

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Vector3Int[] m_HighlightPositions;
    private Sprite m_PlaceholderTileSprite;
    private TilemapManager m_TilemapManager;

    private Color m_HighlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_BlockedColor = new Color(1f, 0.2f, 0, 0.8f);

    public BuildActionSO BuildAction => m_BuildAction;

    public int GoldCost => m_BuildAction.GoldCost;
    public int WoodCost => m_BuildAction.WoodCost;

    public PlacementProcess(
        BuildActionSO buildAction,
        TilemapManager tilemapManager
    )
    {
        m_PlaceholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");
        m_BuildAction = buildAction;
        m_TilemapManager = tilemapManager;
    }

    public void Update()
    {
        if (m_PlacementOutline != null)
        {
            HighlightTiles(m_PlacementOutline.transform.position);
        }

        if (HvoUtils.IsPointerOverUIElement()) return;

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

    public void Cleanup()
    {
        Object.Destroy(m_PlacementOutline);
        ClearHighlights();
    }

    public bool TryFinalizePlacement(out Vector3 buildPosition)
    {
        if (IsPlacementAreaValid())
        {
            ClearHighlights();
            buildPosition = m_PlacementOutline.transform.position;
            Object.Destroy(m_PlacementOutline);
            return true;
        }

        Debug.Log("Invalid Placement Area");
        buildPosition = Vector3.zero;
        return false;
    }

    bool IsPlacementAreaValid()
    {
        foreach (var tilePosition in m_HighlightPositions)
        {
            if (!m_TilemapManager.CanPlaceTile(tilePosition)) return false;
        }

        return true;
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

            if (m_TilemapManager.CanPlaceTile(tilePosition))
            {
                tile.color = m_HighlightColor;
            }
            else
            {
                tile.color = m_BlockedColor;
            }

            m_TilemapManager.SetTileOverlay(tilePosition, tile);
        }
    }

    void ClearHighlights()
    {
        if (m_HighlightPositions == null) return;

        foreach (var tilePosition in m_HighlightPositions)
        {
            m_TilemapManager.SetTileOverlay(tilePosition, null);
        }
    }
}
