

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{

    protected override void UpdateBehaviour()
    {
        CheckForCloseObjects();
    }

    private void CheckForCloseObjects()
    {
        var hits = RunProximityObjectDetection();

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;
            Debug.Log(hit.gameObject.name);
        }
    }
}
