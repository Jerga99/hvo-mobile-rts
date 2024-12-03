
using UnityEngine;

public class TowerUnit: StructureUnit
{
    [SerializeField] private Projectile m_ProjectilePrefab;

    public override bool IsPlayer => true;

    public override void OnConstructionFinished()
    {
        base.OnConstructionFinished();
    }

    protected override void AfterConstructionUpdate()
    {
        if (TryFindClosestFoe(out var foe))
        {
            SetTarget(foe);
            TryAttackCurrentTarget();
        }
    }

    protected override void OnAttackReady(Unit target)
    {
        var projectile = Instantiate(m_ProjectilePrefab, transform.position, Quaternion.identity);
        projectile.Initialize(this, target, m_AutoAttackDamage);
    }
}
