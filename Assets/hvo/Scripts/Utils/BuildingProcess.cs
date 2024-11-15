

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
        var structure = Object.Instantiate(buildAction.StructurePrefab);
        structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        structure.transform.position = placementPosition;
    }
}
