


using UnityEngine;

public class SoldierUnit: HumanoidUnit
{
    protected override void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        if (newTask == UnitTask.Attack && HasTarget)
        {
            MoveTo(Target.transform.position);
        }

        base.OnSetTask(oldTask, newTask);
    }

    protected override void OnSetDestination()
    {
        if (CurrentTask == UnitTask.Attack)
        {
            SetTask(UnitTask.None);
            SetTarget(null);
        }
    }

    protected override void UpdateBehaviour()
    {
        if (CurrentState == UnitState.Idle || CurrentState == UnitState.Moving)
        {
            if (HasTarget)
            {
                if (IsTargetInRange(Target.transform))
                {
                    StopMovement();
                    SetState(UnitState.Attacking);
                }
            }
            else
            {
                if (TryFindClosestFoe(out var foe))
                {
                    SetTarget(foe);
                    SetTask(UnitTask.Attack);
                }
            }
        }
        else if (CurrentState == UnitState.Attacking)
        {
            if (HasTarget)
            {
                if (IsTargetInRange(Target.transform))
                {
                    TryAttackCurrentTarget();
                }
                else
                {
                    SetState(UnitState.Idle);
                }
            }
            else
            {
                SetState(UnitState.Idle);
            }
        }
    }
}
