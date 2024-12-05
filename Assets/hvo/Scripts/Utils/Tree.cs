

using UnityEngine;

public class Tree: MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;
    [SerializeField] private Animator m_Animator;

    public bool m_Claimed = false;
    public bool Claimed => m_Claimed;

    public bool TryToClaim()
    {
        if (!m_Claimed)
        {
            m_Claimed = true;
            return true;
        }

        return false;
    }

    public void Release()
    {
        m_Claimed = false;
    }

    public void Hit()
    {
        m_Animator.SetTrigger("Hit");
    }

    public Vector3 GetBottomPosition()
    {
        return m_Collider.bounds.min;
    }

}
