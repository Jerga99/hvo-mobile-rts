

using System.Collections;
using UnityEngine;

public enum UnitState
{
    Idle, Moving, Attacking, Chopping, Minig, Building, Dead
}

public enum UnitTask
{
    None, Build, Chop, Mine, Attack
}

public enum DestinationSource
{
    CodeTriggered, PlayerClick
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_ObjectDetectionRadius = 3f;
    [SerializeField] protected float m_UnitDetectionCheckRate = 0.5f;
    [SerializeField] protected float m_AttackRange = 1.0f;
    [SerializeField] protected float m_AutoAttackFrequency = 1.5f;
    [SerializeField] protected float m_AutoAttackDamageDelay = 0.5f;
    [SerializeField] protected int m_AutoAttackDamage = 7;
    [SerializeField] protected int m_Health = 100;
    [SerializeField] protected Color m_DamageFlashColor = new Color(1f, 0.65f, 0.65f, 1f);

    public bool IsTargeted;
    protected GameManager m_GameManager;
    protected Animator m_Animator;
    protected AIPawn m_AIPawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightMaterial;
    protected CapsuleCollider2D m_Collider;
    protected float m_NextUnitDetectionTime;
    protected float m_NextAutoAttackTime;
    protected int m_CurrentHealth;
    protected UnitStance m_CurrentStance = UnitStance.Offensive;

    public UnitState CurrentState { get; protected set; } = UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
    public Unit Target { get; protected set; }

    public virtual bool IsPlayer => true;
    public virtual bool IsBuilding => false;
    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;
    public bool HasTarget => Target != null;
    public int CurrentHealth => m_CurrentHealth;
    public UnitStance CurrentStance => m_CurrentStance;

    protected virtual void Start()
    {
        RegisterUnit();
    }

    protected void Awake()
    {
        if (TryGetComponent<Animator>(out var animator))
        {
            m_Animator = animator;
        }

        if (TryGetComponent<AIPawn>(out var aiPawn))
        {
            m_AIPawn = aiPawn;
            m_AIPawn.OnNewPositionSelected += TurnToPosition;
            m_AIPawn.OnDestinationReached += OnDestinationReached;
        }

        m_Collider = GetComponent<CapsuleCollider2D>();
        m_GameManager = GameManager.Get();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/Outline");

        m_CurrentHealth = m_Health;
    }

    void OnDestroy()
    {
        if (m_AIPawn != null)
        {
            m_AIPawn.OnNewPositionSelected -= TurnToPosition;
            m_AIPawn.OnDestinationReached -= OnDestinationReached;
        }
    }

    public void SetTask(UnitTask task)
    {
        OnSetTask(CurrentTask, task);
    }

    public void SetState(UnitState state)
    {
        OnSetState(CurrentState, state);
    }

    public void SetTarget(Unit target)
    {
        Target = target;
    }

    public virtual void SetStance(UnitStanceActionSO stanceActionSO)
    {
        m_CurrentStance = stanceActionSO.UnitStance;

        for (int i = 0; i < m_Actions.Length; i++)
        {
            if (m_Actions[i] == stanceActionSO)
            {
                m_GameManager.FocusActionUI(i);
                return;
            }
        }
    }

    public void MoveTo(Vector3 destination, DestinationSource source = DestinationSource.CodeTriggered)
    {
        var direction = (destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;

        m_AIPawn.SetDestination(destination);
        OnSetDestination(source);
    }

    public void Select()
    {
        Highlight();
        IsTargeted = true;
    }

    public void Deselect()
    {
        UnHighlight();
        IsTargeted = false;
    }

    public void StopMovement()
    {
        m_AIPawn.Stop();
    }

    public Vector3 GetTopPosition()
    {
        if (m_Collider == null) return transform.position;

        return transform.position + Vector3.up * m_Collider.size.y / 2;
    }

    protected virtual void OnSetDestination(DestinationSource source) { }

    protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    protected virtual void OnDestinationReached(){}

    protected virtual void RegisterUnit()
    {
        m_GameManager.RegisterUnit(this);
    }

    protected virtual void UnregisterUnit()
    {
        m_GameManager.UnregisterUnit(this);
    }

    protected virtual bool TryFindClosestFoe(out Unit foe)
    {
        if (Time.time >= m_NextUnitDetectionTime)
        {
            m_NextUnitDetectionTime = Time.time + m_UnitDetectionCheckRate;
            foe = m_GameManager.FindClosestUnit(transform.position, m_ObjectDetectionRadius, !IsPlayer);
            return foe != null;
        }
        else
        {
            foe = null;
            return false;
        }
    }

    protected virtual bool TryAttackCurrentTarget()
    {
        if (Target.CurrentState == UnitState.Dead) return false;

        if (Time.time >= m_NextAutoAttackTime)
        {
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency;
            PerformAttackAnimation();
            StartCoroutine(DelayDamage(m_AutoAttackDamageDelay, m_AutoAttackDamage, Target));
            return true;
        }

        return false;
    }

    protected virtual void PerformAttackAnimation(){}
    protected virtual void RunDeadEffect(){}
    protected virtual void Die()
    {
        SetState(UnitState.Dead);
        RunDeadEffect();
        UnregisterUnit();
    }


    protected virtual void TakeDamage(int damage, Unit damager)
    {
        if (CurrentState == UnitState.Dead) return;

        m_CurrentHealth -= damage;

        if (!HasTarget)
        {
            SetTarget(damager);
        }

        m_GameManager.ShowTextPopup(
            damage.ToString(),
            GetTopPosition(),
            Color.red
        );
        StartCoroutine(FlashEffect(0.2f, 2, m_DamageFlashColor));

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected IEnumerator FlashEffect(float duration, int flashCount, Color color)
    {
        Color originalColor = m_SpriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            m_SpriteRenderer.color = color;
            yield return new WaitForSeconds(duration / 2f);

            m_SpriteRenderer.color = originalColor;
            yield return new WaitForSeconds(duration / 2f);
        }
    }

    protected IEnumerator DelayDamage(float delay, int damage, Unit target)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            if (target.CurrentState == UnitState.Dead)
            {
                SetTarget(null);
            }
            else
            {
                target.TakeDamage(damage, this);
            }
        }
    }

    protected bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= m_AttackRange;
    }

    protected Collider2D[] RunProximityObjectDetection()
    {
        return Physics2D.OverlapCircleAll(transform.position, m_ObjectDetectionRadius);
    }

    void TurnToPosition(Vector3 newPosition)
    {
        var direction = (newPosition - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;
    }

    void Highlight()
    {
        m_SpriteRenderer.material = m_HighlightMaterial;
    }

    void UnHighlight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, m_ObjectDetectionRadius);

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawSphere(transform.position, m_AttackRange);
    }
}
