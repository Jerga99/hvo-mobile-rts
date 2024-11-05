

using System;
using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Vector2 m_Velocity;
    protected Vector3 m_LastPosition;

    public float CurrentSpeed => m_Velocity.magnitude;

    protected void Update()
    {
        m_Velocity = new Vector2(
            (transform.position.x - m_LastPosition.x),
            (transform.position.y - m_LastPosition.y)
        ) / Time.deltaTime;

        m_LastPosition = transform.position;
        IsMoving = m_Velocity.magnitude > 0;

        m_Animator.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
    }
}

