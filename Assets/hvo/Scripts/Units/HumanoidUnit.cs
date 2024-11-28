

using System;
using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Vector2 m_Velocity;
    protected Vector3 m_LastPosition;

    protected float m_SmoothFactor = 50;
    protected float m_SmoothedSpeed;

    public float CurrentSpeed => m_Velocity.magnitude;

    protected override void Start()
    {
        base.Start();
        m_LastPosition = transform.position;
    }

    protected void Update()
    {
        UpdateVelocity();
        UpdateBehaviour();
        UpdateMovementAnimation();
    }

    protected virtual void UpdateBehaviour() { }

    protected virtual void UpdateVelocity()
    {
        m_Velocity = new Vector2(
            (transform.position.x - m_LastPosition.x),
            (transform.position.y - m_LastPosition.y)
        ) / Time.deltaTime;

        m_LastPosition = transform.position;
        m_SmoothedSpeed = Mathf.Lerp(m_SmoothedSpeed, CurrentSpeed, Time.deltaTime * m_SmoothFactor);

        if (CurrentState != UnitState.Attacking)
        {
            var state = m_SmoothedSpeed > 0.1f ? UnitState.Moving : UnitState.Idle;
            SetState(state);
        }
    }

    protected virtual void UpdateMovementAnimation()
    {
        m_Animator?.SetFloat("Speed", Mathf.Clamp01(m_SmoothedSpeed));
    }
}

