

using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    [SerializeField] private float m_WoodGatherTickTime = 1f;
    [SerializeField] private int m_WoodPerTick = 1;
    [SerializeField] private float m_HitTreeFrequency = 0.5f;

    private float m_ChoppingTimer;
    private float m_HitTreeTimer;
    private int m_WoodCollected;
    private int m_GoldCollected;
    private int m_WoodCapacity = 5;
    private int m_GoldCapacity = 10;
    private Tree m_AssignedTree;

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


        if (CurrentState == UnitState.Chopping && m_WoodCollected < m_WoodCapacity)
        {
            StartChopping();
        }

        Debug.Log(m_WoodCollected);
    }

    protected override void OnSetDestination(DestinationSource source){
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
                m_Animator.SetBool("IsChopping", false);
                SetState(UnitState.Idle);
            }
        }

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
