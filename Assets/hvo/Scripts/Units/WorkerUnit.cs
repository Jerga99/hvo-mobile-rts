

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    [SerializeField] private float m_WoodGatherTickTime = 1f;
    [SerializeField] private int m_WoodPerTick = 4;
    [SerializeField] private float m_HitTreeFrequency = 0.5f;

    [SerializeField] private SpriteRenderer m_HoldingWoodSprite;
    [SerializeField] private SpriteRenderer m_HoldingGoldSprite;
    [SerializeField] private AudioSettings m_ChopAudioSettings;
    [SerializeField] private AudioSettings m_BuildAudioSettings;

    private float m_ChoppingTimer;
    private float m_HitTreeTimer;
    private int m_WoodCollected;
    private int m_GoldCollected;
    private int m_WoodCapacity = 20;
    private int m_GoldCapacity = 10;
    private Tree m_AssignedTree;
    private GoldMine m_AssignedMine;
    private StructureUnit m_AssignedWoodStorage;
    private StructureUnit m_AssignedGoldStorage;

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
            CurrentTask == UnitTask.Mine
            && m_AssignedMine != null
            && !IsHoldingGold
        )
        {
            HandleMinningTask();
        }
        else if (CurrentTask == UnitTask.ReturnResource)
        {
            if (IsHoldingGold && TryToReturnResources(m_AssignedGoldStorage))
            {
                m_GameManager.ShowTextPopup(m_GoldCollected.ToString(), GetTopPosition(), Color.yellow);
                m_GameManager.AddResources(m_GoldCollected, 0);
                m_GoldCollected = 0;
                MoveTo(m_GameManager.ActiveGoldMine.GetBottomPosition());
                SetTask(UnitTask.Mine);
            }
            else if (IsHoldingWood && TryToReturnResources(m_AssignedWoodStorage, 1f))
            {
                m_GameManager.ShowTextPopup(m_WoodCollected.ToString(), GetTopPosition(), Color.green);
                m_GameManager.AddResources(0, m_WoodCollected);
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

    public void OnBuildingFinished() => ResetState();

    public void SetWoodStorage(StructureUnit storage)
    {
        m_AssignedWoodStorage = storage;
    }

    public void SetGoldStorage(StructureUnit storage)
    {
        m_AssignedGoldStorage = storage;
    }

    public void SendToBuild(StructureUnit structure, DestinationSource destinationSource = DestinationSource.CodeTriggered)
    {
        MoveTo(structure.transform.position, destinationSource);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    public void SendToChop(Tree tree, DestinationSource destinationSource = DestinationSource.CodeTriggered)
    {
        if (tree.TryToClaim())
        {
            MoveTo(tree.GetBottomPosition(), destinationSource);
            SetTask(UnitTask.Chop);
            m_AssignedTree = tree;
        }
    }

    public void SendToMine(GoldMine mine, DestinationSource destinationSource = DestinationSource.CodeTriggered)
    {
        MoveTo(mine.GetBottomPosition(), destinationSource);
        SetTask(UnitTask.Mine);
        m_AssignedMine = mine;
    }

    public void OnEnterMine()
    {
        Hide();
    }

    public void OnLeaveMine()
    {
        Show();
        m_GoldCollected = m_GoldCapacity;
        SetState(UnitState.Idle);

        m_AssignedGoldStorage = m_GameManager.FindClosestGoldStorage(transform.position);

        if (m_AssignedGoldStorage != null)
        {
            MoveTo(m_AssignedGoldStorage.transform.position);
            SetTask(UnitTask.ReturnResource);
        }
    }

    public void PlayBuildSound()
    {
        m_AudioManager.PlaySound(m_BuildAudioSettings, transform.position);
    }

    protected override void OnSetDestination(DestinationSource source)
    {
        base.OnSetDestination(source);
        if (CurrentState == UnitState.Minig) return;

        SetState(UnitState.Moving);
        ResetState();
    }

    protected override void Die()
    {
        base.Die();
        if (m_AssignedTree != null) m_AssignedTree.Release();

    }

    bool TryToReturnResources(StructureUnit storage, float distanceTreshold = 0.5f)
    {
        if (storage != null)
        {
            var closestPoint = storage.Collider.ClosestPoint(transform.position);
            var distance = Vector3.Distance(closestPoint, transform.position);
            return distance < distanceTreshold;
        }

        return false;
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

    void HandleMinningTask()
    {
        var mineBottomPosition = m_AssignedMine.GetBottomPosition();
        var workerClosestPoint = Collider.ClosestPoint(mineBottomPosition);
        var distance = Vector3.Distance(mineBottomPosition, workerClosestPoint);

        if (distance <= 0.20f)
        {
            if (m_AssignedMine.TryToEnterMine(this))
            {
                m_WoodCollected = 0;
                StopMovement();
                SetState(UnitState.Minig);
            }
        }
    }

    void HandleChoppingTask()
    {
        var treeBottomPosition = m_AssignedTree.GetBottomPosition();
        var workerClosestPoint = Collider.ClosestPoint(treeBottomPosition);

        var distance = Vector3.Distance(treeBottomPosition, workerClosestPoint);

        if (distance <= 0.1f)
        {
            m_GoldCollected = 0;
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
            m_AudioManager.PlaySound(m_ChopAudioSettings, transform.position);
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
