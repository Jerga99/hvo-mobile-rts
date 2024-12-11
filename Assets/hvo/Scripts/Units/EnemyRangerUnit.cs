

using System.Collections;
using UnityEngine;

public class EnemyRangerUnit : EnemyUnit
{
    [SerializeField] private Projectile m_ProjectilePrefab;

    protected override void OnAttackReady(Unit target)
    {
        OnPlayAttackSound();
        PerformAttackAnimation();
        StartCoroutine(ShootProjectile(0.4f, target));
    }

    protected override void PerformAttackAnimation()
    {
        m_Animator.SetTrigger("Attack");
    }

    private IEnumerator ShootProjectile(float delay, Unit target)
    {
        yield return new WaitForSeconds(delay);

        if (CurrentState == UnitState.Dead) yield return null;

        if (target != null && target.CurrentState != UnitState.Dead)
        {
            var projectile = Instantiate(m_ProjectilePrefab, transform.position, Quaternion.identity);
            projectile.Initialize(this, target, m_AutoAttackDamage);
        }
    }
}
