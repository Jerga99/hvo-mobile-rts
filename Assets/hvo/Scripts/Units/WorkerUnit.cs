

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{

    protected override void UpdateBehaviour()
    {
        if (CurrentTask == UnitTask.Build && HasTarget)
        {
            CheckForConstruction();
        }
    }

    protected override void OnSetDestination(DestinationSource source) => ResetState();

    public void OnBuildingFinished() => ResetState();


    public void SendToBuild(StructureUnit structure)
    {
        MoveTo(structure.transform.position);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    void CheckForConstruction()
    {
        var distanceToConstruction = Vector3.Distance(transform.position, Target.transform.position);

        if (distanceToConstruction <= m_ObjectDetectionRadius && CurrentState == UnitState.Idle)
        {
            StartBuilding(Target as StructureUnit);
        }
    }

    void StartBuilding(StructureUnit structure)
    {
        SetState(UnitState.Building);
        m_Animator.SetBool("IsBuilding", true);
        structure.AssignWorkerToBuildProcess(this);
    }

    void ResetState()
    {
        SetTask(UnitTask.None);

        if (HasTarget) CleanupTarget();

        m_Animator.SetBool("IsBuilding", false);
    }

    void CleanupTarget()
    {
        if (Target is StructureUnit structure)
        {
            structure.UnassignWorkerFromBuildProcess();
        }

        SetTarget(null);
    }
}





// void CheckForCloseObjects()
//     {
//         Debug.Log("Checking!");
//         var hits = RunProximityObjectDetection();

//         foreach (var hit in hits)
//         {
//             if (hit.gameObject == this.gameObject) continue;

//             if (CurrentTask == UnitTask.Build && hit.gameObject == Target.gameObject)
//             {
//                 if (hit.TryGetComponent<StructureUnit>(out var unit))
//                 {
//                     StartBuilding(unit);
//                 }
//             }
//         }
//     }
