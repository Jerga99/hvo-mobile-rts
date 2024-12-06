

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    [SerializeField] private float m_WoodGatherTickTime = 1f;
    [SerializeField] private int m_WoodPerTick = 1;
    [SerializeField] private float m_HitTreeFrequency = 0.5f;

    [SerializeField] private SpriteRenderer m_HoldingWoodSprite;
    [SerializeField] private SpriteRenderer m_HoldingGoldSprite;

    private float m_ChoppingTimer;
    private float m_HitTreeTimer;
    private int m_WoodCollected;
    private int m_GoldCollected;
    private int m_WoodCapacity = 5;
    private int m_GoldCapacity = 10;
    private Tree m_AssignedTree;
    private StructureUnit m_AssignedWoodStorage;

    public bool IsHoldingWood => m_WoodCollected > 0;
    public bool IsHoldingGold => m_GoldCollected > 0;
    public bool IsHoldingResource => IsHoldingWood || IsHoldingGold;

    protected override void UpdateBehaviour()
    {
        if (CurrentTask == UnitTask.Build && HasTarget)
        {
            CheckForConstruction();
        }
        else if (
            CurrentTask == UnitTask.Chop
            && m_AssignedTree != null
            && m_WoodCollected < m_WoodCapacity
        )
        {
            HandleChoppingTask();
        }
        else if (
            CurrentTask == UnitTask.ReturnResource
            && m_AssignedWoodStorage != null
            && IsHoldingWood
        )
        {
            var closesPointOnStorage = m_AssignedWoodStorage.Collider.ClosestPoint(transform.position);
            var distance = Vector3.Distance(closesPointOnStorage, transform.position);

            if (distance < 1f)
            {
                m_WoodCollected = 0;
                TryMoveToClosestTree();
            }
        }

        if (CurrentState == UnitState.Chopping && m_WoodCollected < m_WoodCapacity)
        {
            StartChopping();
        }

        HandleResourceDisplay();
    }

    protected override void OnSetDestination(DestinationSource source)
    {
        SetState(UnitState.Moving);
        ResetState();
    }

    public void OnBuildingFinished() => ResetState();

    public void SendToBuild(StructureUnit structure)
    {
        MoveTo(structure.transform.position);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    public void SendToChop(Tree tree)
    {
        if (tree.TryToClaim())
        {
            MoveTo(tree.GetBottomPosition());
            SetTask(UnitTask.Chop);
            m_AssignedTree = tree;
        }
    }

    protected override void Die()
    {
        base.Die();
        if (m_AssignedTree != null) m_AssignedTree.Release();

    }

    void HandleResourceDisplay()
    {
        if (IsHoldingResource)
        {
            if (IsHoldingGold)
            {
                m_HoldingGoldSprite.gameObject.SetActive(true);
                m_HoldingWoodSprite.gameObject.SetActive(false);
            }
            else
            {
                m_HoldingGoldSprite.gameObject.SetActive(false);
                m_HoldingWoodSprite.gameObject.SetActive(true);
            }

            m_Animator.SetFloat("IsHoldingResource", 1f);
        }
        else
        {
            m_HoldingGoldSprite.gameObject.SetActive(false);
            m_HoldingWoodSprite.gameObject.SetActive(false);
            m_Animator.SetFloat("IsHoldingResource", 0f);
        }
    }

    void HandleChoppingTask()
    {
        var treeBottomPosition = m_AssignedTree.GetBottomPosition();
        var workerClosestPoint = Collider.ClosestPoint(treeBottomPosition);

        var distance = Vector3.Distance(treeBottomPosition, workerClosestPoint);

        if (distance <= 0.1f)
        {
            StopMovement();
            SetState(UnitState.Chopping);
        }
    }

    void StartChopping()
    {
        m_Animator.SetBool("IsChopping", true);
        m_ChoppingTimer += Time.deltaTime;
        m_HitTreeTimer += Time.deltaTime;

        if (m_HitTreeTimer >= m_HitTreeFrequency)
        {
            m_HitTreeTimer = 0;
            m_AssignedTree.Hit();
        }

        if (m_ChoppingTimer >= m_WoodGatherTickTime)
        {
            m_WoodCollected += m_WoodPerTick;
            m_ChoppingTimer = 0;

            if (m_WoodCollected == m_WoodCapacity)
            {
                HandleChoppingFinished();
            }
        }
    }

    void HandleChoppingFinished()
    {
        m_Animator.SetBool("IsChopping", false);

        m_AssignedWoodStorage = m_GameManager.FindClosestWoodStorage(transform.position);

        if (m_AssignedWoodStorage != null)
        {
            var closestPointOnStorage = m_AssignedWoodStorage.Collider.ClosestPoint(transform.position);
            MoveTo(closestPointOnStorage);
        }

        SetState(UnitState.Idle);
        SetTask(UnitTask.ReturnResource);
    }

    void CheckForConstruction()
    {
        var distanceToConstruction = Vector3.Distance(transform.position, Target.transform.position);

        if (distanceToConstruction <= m_ObjectDetectionRadius && CurrentState == UnitState.Idle)
        {
            StartBuilding(Target as StructureUnit);
        }
    }

    void StartBuilding(StructureUnit structure)
    {
        SetState(UnitState.Building);
        m_Animator.SetBool("IsBuilding", true);
        structure.AssignWorkerToBuildProcess(this);
    }

    void TryMoveToClosestTree()
    {
        var closestTree = m_GameManager.FindClosestUnclaimedTree(transform.position);

        if (closestTree != null)
        {
            SendToChop(closestTree);
        }
    }

    void ResetState()
    {
        SetTask(UnitTask.None);

        if (HasTarget) CleanupTarget();

        m_Animator.SetBool("IsBuilding", false);
        m_Animator.SetBool("IsChopping", false);

        m_ChoppingTimer = 0;

        if (m_AssignedTree != null)
        {
            m_AssignedTree.Release();
            m_AssignedTree = null;
        }
    }

    void CleanupTarget()
    {
        if (Target is StructureUnit structure)
        {
            structure.UnassignWorkerFromBuildProcess();
        }

        SetTarget(null);
    }
}
