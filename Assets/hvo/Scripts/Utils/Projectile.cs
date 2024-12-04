

using Unity.VisualScripting;
using UnityEngine;

public class Projectile: MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private bool m_EnableDynamiteRotation = false;

    private int m_Damage = 10;

    private Unit m_Target;
    private Unit m_Owner;

    public void Initialize(Unit owner, Unit target, int damage)
    {
        m_Owner = owner;
        m_Target = target;
        m_Damage = damage;
    }

    void Update()
    {
        if (m_Target == null || m_Target.CurrentState == UnitState.Dead)
        {
            Destroy(gameObject);
            return;
        }

        var direction = (m_Target.transform.position - transform.position).normalized;

        float angle;

        if (m_EnableDynamiteRotation)
        {
            float currentRotation = transform.eulerAngles.z;
            angle = currentRotation + 720 * Time.deltaTime;
        }
        else
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position += direction * m_Speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Unit>(out var targetUnit))
        {
            if (targetUnit == m_Target)
            {
                targetUnit.TakeDamage(m_Damage, m_Owner);
                Destroy(gameObject);
            }
        }
    }
}
