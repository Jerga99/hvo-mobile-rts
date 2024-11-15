

using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO m_BuildAction;

    public BuildingProcess(
        BuildActionSO buildAction,
        Vector3 placementPosition,
        WorkerUnit worker
    )
    {
        m_BuildAction = buildAction;
        var structure = Object.Instantiate(buildAction.StructurePrefab);
        structure.Renderer.sprite = m_BuildAction.FoundationSprite;
        structure.transform.position = placementPosition;
        structure.RegisterProcess(this);

        worker.MoveTo(placementPosition);
        worker.SetTask(UnitTask.Build);
        worker.SetTarget(structure);
    }

    public void Update()
    {
        Debug.Log("Building is under construction");
    }
}
