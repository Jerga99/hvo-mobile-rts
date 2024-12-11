
using System.Collections;
using UnityEngine;

public class TowerUnit : StructureUnit
{
    [SerializeField] private Projectile m_ProjectilePrefab;

    public override bool IsPlayer => true;

    public override void OnConstructionFinished()
    {
        base.OnConstructionFinished();
    }

    protected override void AfterConstructionUpdate()
    {
        if (CurrentState == UnitState.Dead) return;

        if (HasTarget)
        {
            TryAttackCurrentTarget();
        }
        else
        {
            if (TryFindClosestFoe(out var foe))
            {
                SetTarget(foe);
            }
        }
    }

    protected override void OnAttackReady(Unit target)
    {
        OnPlayAttackSound();
        var projectile = Instantiate(m_ProjectilePrefab, transform.position, Quaternion.identity);
        projectile.Initialize(this, target, m_AutoAttackDamage);
    }

    protected override void RunDeadEffect()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            m_SpriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
