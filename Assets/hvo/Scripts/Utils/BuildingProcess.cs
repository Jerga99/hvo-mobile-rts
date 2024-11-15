

using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO m_BuildAction;

    public BuildingProcess(
        BuildActionSO buildAction,
        Vector3 placementPosition
    )
    {
        m_BuildAction = buildAction;
        var structureGo = new GameObject(m_BuildAction.ActionName);
        var renderer = structureGo.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 25;
        renderer.sprite = m_BuildAction.FoundationSprite;
        structureGo.transform.position = placementPosition;
    }
}
