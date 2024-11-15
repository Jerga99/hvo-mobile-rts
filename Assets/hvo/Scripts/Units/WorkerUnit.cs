

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{

    protected override void UpdateBehaviour()
    {
        if (CurrentTask != UnitTask.None)
        {
            CheckForCloseObjects();
        }
    }

    void CheckForCloseObjects()
    {
        Debug.Log("Checking!");
        var hits = RunProximityObjectDetection();

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            if (CurrentTask == UnitTask.Build && hit.gameObject == Target.gameObject)
            {
                if (hit.TryGetComponent<StructureUnit>(out var unit))
                {
                    StartBuilding(unit);
                }
            }
        }
    }

    void StartBuilding(StructureUnit unit)
    {
        Debug.Log("Starting building: " + unit.gameObject.name);
    }
}
