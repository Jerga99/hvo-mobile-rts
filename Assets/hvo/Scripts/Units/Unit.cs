

using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public bool IsMoving;

    protected Animator m_Animator;

    protected void Awake()
    {
        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }
    }
}
