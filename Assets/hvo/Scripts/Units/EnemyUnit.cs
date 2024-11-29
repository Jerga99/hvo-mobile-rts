

using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    private float m_AttackCommitmentTime = 1f;
    private float m_CurrentAttackCommitmentTime = 0;
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
                        SetState(UnitState.Attacking);
                        StopMovement();
                    }
                    else
                    {
                        MoveTo(Target.transform.position);
                    }
                }
                else
                {
                    if (TryFindClosestFoe(out var foe))
                    {
                        SetTarget(foe);
                        MoveTo(foe.transform.position);
                    }
                }

                break;
            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target.transform))
                    {
                        m_CurrentAttackCommitmentTime = m_AttackCommitmentTime;
                        TryAttackCurrentTarget();
                    }
                    else
                    {
                        m_CurrentAttackCommitmentTime -= Time.deltaTime;
                        if (m_CurrentAttackCommitmentTime <= 0)
                        {
                            SetState(UnitState.Moving);
                        }
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }
                break;
        }
    }
}
