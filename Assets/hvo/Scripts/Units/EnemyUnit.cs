

using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    public override bool IsPlayer => false;

    protected override void UpdateBehaviour()
    {

        switch (CurrentState)
        {
            case UnitState.Idle:
            case UnitState.Moving:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        Debug.Log("Changing to Attacking State");
                        SetState(UnitState.Attacking);
                        // Stop Movement!
                    }
                    else
                    {
                        Debug.Log("Move to Target!");
                        MoveTo(Target.transform.position);
                    }
                }
                else
                {
                    if (TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        MoveTo(foe.transform.position);
                        Debug.Log("Target Detected - Move to target!");
                    }
                }

                break;
            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        TryAttackCurrentTarget();
                    }
                    else
                    {
                        Debug.Log("Back to moving state!");
                        SetState(UnitState.Moving);
                    }
                }
                else
                {
                    Debug.Log("Back to idle state!");
                    SetState(UnitState.Idle);
                }
                break;
        }
    }
}
