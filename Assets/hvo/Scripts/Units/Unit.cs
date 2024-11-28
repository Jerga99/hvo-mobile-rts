

using UnityEngine;

public enum UnitState
{
    Idle, Moving, Attacking, Chopping, Minig, Building
}

public enum UnitTask
{
    None, Build, Chop, Mine, Attack
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_ObjectDetectionRadius = 3f;
    [SerializeField] protected float m_UnitDetectionCheckRate = 0.5f;
    [SerializeField] protected float m_AttackRange = 1.0f;
    [SerializeField] protected float m_AutoAttackFrequency = 1.5f;

    public bool IsTargeted;
    protected GameManager m_GameManager;
    protected Animator m_Animator;
    protected AIPawn m_AIPawn;
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightMaterial;
    protected float m_NextUnitDetectionTime;
    protected float m_NextAutoAttackTime;

    public UnitState CurrentState { get; protected set; } = UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;
    public Unit Target { get; protected set; }

    public virtual bool IsPlayer => true;
    public virtual bool IsBuilding => false;
    public ActionSO[] Actions => m_Actions;
    public SpriteRenderer Renderer => m_SpriteRenderer;
    public bool HasTarget => Target != null;

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
        }

        m_GameManager = GameManager.Get();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalMaterial = m_SpriteRenderer.material;
        m_HighlightMaterial = Resources.Load<Material>("Materials/Outline");
    }

    void OnDestroy()
    {
        if (m_AIPawn != null)
        {
            m_AIPawn.OnNewPositionSelected -= TurnToPosition;
        }

        UnregisterUnit();
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

    public void MoveTo(Vector3 destination)
    {
        var direction = (destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;

        m_AIPawn.SetDestination(destination);
        OnSetDestination();
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

    protected virtual void OnSetDestination() { }

    protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

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
        if (Time.time >= m_NextAutoAttackTime)
        {
            Debug.Log("Attack!");
            m_NextAutoAttackTime = Time.time + m_AutoAttackFrequency;
            return true;
        }

        Debug.Log("Attack is on CD");
        return false;
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
