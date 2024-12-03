


using UnityEngine;

public class SoldierUnit : HumanoidUnit
{
    private bool m_IsRetreating = false;

    public override void SetStance(UnitStanceActionSO stanceActionSO)
    {
        base.SetStance(stanceActionSO);

        if (CurrentStance == UnitStance.Defensive)
        {
            SetState(UnitState.Idle);
            StopMovement();
            m_IsRetreating = false;
        }
    }

    protected override void OnSetState(UnitState oldState, UnitState newState)
    {
        if (newState == UnitState.Attacking)
        {
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency / 2f;
        }

        base.OnSetState(oldState, newState);
    }

    protected override void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        if (newTask == UnitTask.Attack && HasTarget)
        {
            MoveTo(Target.transform.position);
        }

        base.OnSetTask(oldTask, newTask);
    }

    protected override void OnSetDestination(DestinationSource source)
    {
        if (
            HasTarget
            && source == DestinationSource.PlayerClick
            && (CurrentTask == UnitTask.Attack || CurrentState == UnitState.Attacking))
        {
            m_IsRetreating = true;
            SetTarget(null);
            SetTask(UnitTask.None);
            Debug.Log("Retreating!");
        }
    }

    protected override void OnDestinationReached()
    {
        if (m_IsRetreating)
        {
            m_IsRetreating = false;
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
                else if (CurrentStance == UnitStance.Offensive)
                {
                    MoveTo(Target.transform.position);
                }
            }
            else
            {
                if (CurrentStance == UnitStance.Offensive)
                {
                    if (!m_IsRetreating && TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        SetTask(UnitTask.Attack);
                    }
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
                    if (CurrentStance == UnitStance.Defensive)
                    {
                        SetTarget(null);
                        SetState(UnitState.Idle);
                    }
                    else
                    {
                        MoveTo(Target.transform.position);
                    }
                }
            }
            else
            {
                SetState(UnitState.Idle);
            }
        }
    }
}
