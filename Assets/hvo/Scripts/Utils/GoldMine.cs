


using UnityEngine;

public class GoldMine: MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_Collider;

    public void EnterMine()
    {

    }

    public Vector3 GetBottomPosition()
    {
        return m_Collider.bounds.min;
    }
}
