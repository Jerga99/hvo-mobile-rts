

using UnityEngine;

public class BuildingProcess
{
    private BuildActionSO m_BuildAction;
    private WorkerUnit m_Worker;

    public bool HasActiveWorker => m_Worker != null;

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
        if (HasActiveWorker)
        {
            Debug.Log("Building is under construction");
        }
    }

    public void AddWorker(WorkerUnit worker)
    {
        if (HasActiveWorker) return;
        Debug.Log("Adding Worker");
        m_Worker = worker;
    }

    public void RemoveWorker()
    {
        if (!HasActiveWorker) return;
        Debug.Log("Removing Worker");
        m_Worker = null;
    }
}
