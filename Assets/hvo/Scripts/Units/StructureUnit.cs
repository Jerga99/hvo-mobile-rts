

using System.Numerics;
using UnityEngine;

public class StructureUnit: Unit
{
    private BuildingProcess m_BuildingProcess;

    public bool IsUnderConstuction => m_BuildingProcess != null;

    void Update()
    {
        if (IsUnderConstuction)
        {
            m_BuildingProcess.Update();
        }
    }

    void OnDestroy()
    {
        UpdateWalkability();
    }

    public void OnConstructionFinished()
    {
        m_BuildingProcess = null;
        UpdateWalkability();
    }

    public void RegisterProcess(BuildingProcess process)
    {
        m_BuildingProcess = process;
    }

    public void AssignWorkerToBuildProcess(WorkerUnit worker)
    {
        m_BuildingProcess?.AddWorker(worker);
    }

    public void UnassignWorkerFromBuildProcess()
    {
        m_BuildingProcess?.RemoveWorker();
    }

    void UpdateWalkability()
    {
        int buildingWidthInTiles = 6;
        int buildingHeightInTiles = 6;

        float halfWidth = buildingWidthInTiles / 2f;
        float halfHeight = buildingHeightInTiles / 2f;

        Vector3Int startPosition = new Vector3Int(
            Mathf.FloorToInt(transform.position.x - halfWidth),
            Mathf.FloorToInt(transform.position.y - halfHeight),
            0
        );

        TilemapManager.Get()
            .UpdateNodesInArea(startPosition, buildingWidthInTiles, buildingHeightInTiles);
    }
}
