

using UnityEngine;

public class Projectile: MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_Damage = 10f;

    private Unit m_Target;
    private Unit m_Owner;

    public void Initialize(Unit owner, Unit target)
    {
        m_Owner = owner;
        m_Target = target;

        Debug.Log(m_Owner.gameObject.name);
        Debug.Log(target.gameObject.name);
    }


    void Update()
    {

    }
}
