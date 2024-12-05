

using UnityEngine;

public class Tree: MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;

    private bool m_Occupied = false;
    public bool Occupied => m_Occupied;

    public bool TryOccupy()
    {
        if (!m_Occupied)
        {
            m_Occupied = true;
            return true;
        }

        return false;
    }

    public Vector3 GetBottomPosition()
    {
        return m_Collider.bounds.min;
    }

}
