

using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    private float m_AttackCommitmentTime = 1f;
    private float m_CurrentAttackCommitmentTime = 0;
    public override bool IsPlayer => false;
    public Unit KingUnit => m_GameManager.KingUnit;

    protected override void UpdateBehaviour()
    {
        switch (CurrentState)
        {
            case UnitState.Idle:
            case UnitState.Moving:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target))
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
                    else if (KingUnit != null && KingUnit.CurrentState != UnitState.Dead)
                    {
                        var distance = Vector3.Distance(transform.position, KingUnit.transform.position);

                        if (distance < m_ObjectDetectionRadius)
                        {
                            SetTarget(KingUnit);
                        }

                        MoveTo(KingUnit.transform.position);
                    }
                }

                break;
            case UnitState.Attacking:
                if (HasTarget)
                {
                    if (IsTargetInRange(Target))
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
