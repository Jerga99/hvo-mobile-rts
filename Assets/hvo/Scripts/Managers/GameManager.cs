
using System.Collections.Generic;
using UnityEngine;

public enum ClickType
{
    Move, Attack, Build, Chop
}

public class GameManager : SingletonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToMovePrefab;
    [SerializeField] private PointToClick m_PointToBuildPrefab;
    [SerializeField] private PointToClick m_PointToAttackPrefab;
    [SerializeField] private PointToClick m_PointToChopPrefab;
    [SerializeField] private ActionBar m_ActionBar;
    [SerializeField] private ConfirmationBar m_BuildConfirmationBar;
    [SerializeField] private TextPopupController m_TextPopupController;
    [SerializeField] private ResourceDataUI m_ResourceDataUI;

    [Header("Camera Settings")]
    [SerializeField] private float m_PanSpeed = 100;
    [SerializeField] private float m_MobilePanSpeed = 10;

    [Header("VFX")]
    [SerializeField] private ParticleSystem m_ConstructionEffectPrefab;

    [Header("Resources")]
    [SerializeField] private Transform m_TreeContainer;
    [SerializeField] private GoldMine m_ActiveGoldMine;

    public Unit ActiveUnit;

    private Unit m_KingUnit;
    private Tree[] m_Trees = new Tree[0];
    private List<Unit> m_PlayerUnits = new();
    private List<StructureUnit> m_PlayerBuildings = new();
    private List<Unit> m_Enemies = new();
    private CameraController m_CameraController;
    private PlacementProcess m_PlacementProcess;
    private int m_Gold = 0;
    private int m_Wood = 0;

    public int Gold => m_Gold;
    public int Wood => m_Wood;
    public GoldMine ActiveGoldMine => m_ActiveGoldMine;
    public bool HasActiveUnit => ActiveUnit != null;
    public Unit KingUnit => m_KingUnit;

    void Start()
    {
        m_CameraController = new CameraController(m_PanSpeed, m_MobilePanSpeed);
        ClearActionBarUI();
        AddResources(500, 500);
    }

    void Update()
    {
        m_CameraController.Update();

        if (m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else if (HvoUtils.TryGetShortClickPosition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit.IsPlayer)
        {
            if (unit.IsBuilding)
            {
                m_PlayerBuildings.Add(unit as StructureUnit);
            }
            else if (unit.IsKingUnit)
            {
                m_KingUnit = unit;
            }
            else
            {
                m_PlayerUnits.Add(unit);
            }
        }
        else
        {
            m_Enemies.Add(unit);
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        if (unit.IsPlayer)
        {
            if (m_PlacementProcess != null)
            {
                CancelBuildPlacement();
            }

            if (ActiveUnit == unit)
            {
                ClearActionBarUI();
                ActiveUnit.Deselect();
                ActiveUnit = null;
            }

            unit.StopMovement();

            if (unit.IsBuilding)
            {
                m_PlayerBuildings.Remove(unit as StructureUnit);
            }
            else if (unit.IsKingUnit)
            {
                m_KingUnit = null;
            }
            else
            {
                m_PlayerUnits.Remove(unit);
            }
        }
        else
        {
            m_Enemies.Remove(unit);
        }
    }

    public void AddResources(int gold, int wood)
    {
        m_Gold += gold;
        m_Wood += wood;

        m_ResourceDataUI.UpdateResourceDisplay(m_Gold, m_Wood);
    }

    public void ShowTextPopup(string text, Vector3 position, Color color)
    {
        m_TextPopupController.Spawn(text, position, color);
    }

    public Tree FindClosestUnclaimedTree(Vector3 originPosition)
    {
        Tree closestTree = null;
        float closestDistanceSqr = float.MaxValue;

        if (m_Trees.Length == 0)
        {
            m_Trees = new Tree[m_TreeContainer.childCount];

            for (int i = 0; i < m_TreeContainer.childCount; i++)
            {
                m_Trees[i] = m_TreeContainer.GetChild(i).GetComponent<Tree>();
            }
        }

        foreach (var tree in m_Trees)
        {
            if (tree.Claimed) continue;

            float sqrDistance = (tree.transform.position - originPosition).sqrMagnitude;

            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestTree = tree;
            }
        }

        return closestTree;
    }

    public Unit FindClosestUnit(Vector3 originPosition, float maxDistance, bool isPlayer)
    {
        List<Unit> units = isPlayer ? m_PlayerUnits : m_Enemies;
        float sqrMaxDistance = maxDistance * maxDistance;
        Unit closestUnit = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (Unit unit in units)
        {
            if (unit.CurrentState == UnitState.Dead) continue;

            float sqrDistance = (unit.transform.position - originPosition).sqrMagnitude;
            if (sqrDistance < sqrMaxDistance && sqrDistance < closestDistanceSqr)
            {
                closestUnit = unit;
                closestDistanceSqr = sqrDistance;
            }
        }

        return closestUnit;
    }

    public StructureUnit FindClosestWoodStorage(Vector3 originPoint)
    {
        float closestDistacenSqr = float.MaxValue;
        StructureUnit closestUnit = null;

        foreach (StructureUnit unit in m_PlayerBuildings)
        {
            if (unit.CurrentState == UnitState.Dead || !unit.CanStoreWood) continue;

            float sqrDistance = (unit.transform.position - originPoint).sqrMagnitude;
            if (sqrDistance < closestDistacenSqr)
            {
                closestUnit = unit;
                closestDistacenSqr = sqrDistance;
            }
        }

        return closestUnit;
    }

    public StructureUnit FindClosestGoldStorage(Vector3 originPoint)
    {
        float closestDistacenSqr = float.MaxValue;
        StructureUnit closestUnit = null;

        foreach (StructureUnit unit in m_PlayerBuildings)
        {
            if (unit.CurrentState == UnitState.Dead || !unit.CanStoreGold) continue;

            float sqrDistance = (unit.transform.position - originPoint).sqrMagnitude;
            if (sqrDistance < closestDistacenSqr)
            {
                closestUnit = unit;
                closestDistacenSqr = sqrDistance;
            }
        }

        return closestUnit;
    }

    public List<Unit> GetFriendlyUnits(bool isPlayer)
    {
        return isPlayer ? m_PlayerUnits : m_Enemies;
    }

    public void StartBuildProcess(BuildActionSO buildAction)
    {
        if (m_PlacementProcess != null) return;
        var tilemapManager = TilemapManager.Get();
        m_PlacementProcess = new PlacementProcess(
            buildAction,
            tilemapManager
        );
        m_PlacementProcess.ShowPlacementOutline();
        m_BuildConfirmationBar.Show(buildAction.GoldCost, buildAction.WoodCost);
        m_BuildConfirmationBar.SetupHooks(ConfirmBuildPlacement, CancelBuildPlacement);
        m_CameraController.LockCamera = true;
    }

    public void StartUnitTrainProcess(TrainUnitActionSO trainUnitAction)
    {
        m_BuildConfirmationBar.Show(
            trainUnitAction.GoldCost,
            trainUnitAction.WoodCost
        );

        m_BuildConfirmationBar.SetupHooks(
            () => FinalizeUnitTraining(trainUnitAction),
            CancelUnitTrainingConfirmation
        );
    }

    void FinalizeUnitTraining(TrainUnitActionSO trainUnitAction)
    {
        if (!TryDeductResources(trainUnitAction.GoldCost, trainUnitAction.WoodCost))
        {
            Debug.Log("Not Enough Resources!");
            return;
        }

        Collider2D castleCollider = ActiveUnit.Collider;
        float spawnRadius = castleCollider.bounds.extents.x;
        int maxSpawnAttemps = 20;

        for (int i = 0; i < maxSpawnAttemps; i++)
        {
            float angle = (360f / maxSpawnAttemps) * i;
            Vector2 spawnOffset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * spawnRadius;
            Vector2 spawnPosition = (Vector2)ActiveUnit.transform.position + spawnOffset;

            var spawnPositionOnCollider = castleCollider.ClosestPoint(spawnPosition);
            var opositeDirection = (spawnPositionOnCollider - (Vector2)castleCollider.transform.position).normalized;
            spawnPositionOnCollider += opositeDirection * 0.3f;

            var unitLayerMask = LayerMask.GetMask("Unit");
            var hits = Physics2D.OverlapCircleAll(spawnPositionOnCollider, 0.5f, unitLayerMask);
            bool isPositionOccupied = false;

            foreach (var hit in hits)
            {
                if (hit != castleCollider)
                {
                    isPositionOccupied = true;
                    break;
                }
            }

            if (!isPositionOccupied)
            {
                Instantiate(trainUnitAction.UnitPrefab, spawnPositionOnCollider, Quaternion.identity);
                m_BuildConfirmationBar.UpdateRequirementsUI(trainUnitAction.GoldCost, trainUnitAction.WoodCost);
                return;
            }
        }

        AddResources(trainUnitAction.GoldCost, trainUnitAction.WoodCost);
    }

    void CancelUnitTrainingConfirmation()
    {
        m_BuildConfirmationBar.Hide();
    }

    void DetectClick(Vector2 inputPosition)
    {
        if (HvoUtils.IsPointerOverUIElement())
        {
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (HasActiveUnit && ActiveUnit is WorkerUnit worker)
        {
            if (TryGetClickedResource(hit, out Tree tree))
            {
                worker.SendToChop(tree);
                DisplayClickEffect(tree.transform.position, ClickType.Chop);
                return;
            }
            else if (TryGetClickedResource(hit, out GoldMine mine))
            {
                worker.SendToMine(mine);
                DisplayClickEffect(mine.transform.position, ClickType.Build);
                return;
            }

        }

        if (HasClickedOnUnit(hit, out var unit))
        {
            if (unit.IsPlayer)
            {
                HandleClickOnPlayerUnit(unit);
            }
            else
            {
                HandleClickOnEnemy(unit);
            }
        }
        else
        {
            HandleClickOnGround(worldPoint);
        }
    }

    public void CancelActiveUnit()
    {
        ActiveUnit.Deselect();
        ActiveUnit = null;

        ClearActionBarUI();
    }

    public void FocusActionUI(int idx)
    {
        m_ActionBar.FocusAction(idx);
    }

    bool TryGetClickedResource<T>(RaycastHit2D hit, out T resource) where T : MonoBehaviour
    {
        resource = null;
        if (hit.collider == null) return false;
        return hit.collider.TryGetComponent(out resource);
    }

    bool HasClickedOnUnit(RaycastHit2D hit, out Unit unit)
    {
        if (hit.collider != null && hit.collider.TryGetComponent<Unit>(out var clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }

        unit = null;
        return false;
    }

    void HandleClickOnGround(Vector2 worldPoint)
    {
        if (HasActiveUnit && IsHumanoid(ActiveUnit))
        {
            if (ActiveUnit.CurrentState == UnitState.Minig) return;

            DisplayClickEffect(worldPoint, ClickType.Move);
            ActiveUnit.MoveTo(worldPoint, DestinationSource.PlayerClick);
        }
    }

    void HandleClickOnPlayerUnit(Unit unit)
    {
        if (HasActiveUnit)
        {
            if (HasClickedOnActiveUnit(unit))
            {
                CancelActiveUnit();
                return;
            }
            else if (ActiveUnit is WorkerUnit worker)
            {
                if (WorkerClickedOnUnfinishedBuild(unit))
                {
                    DisplayClickEffect(unit.transform.position, ClickType.Build);
                    worker.SendToBuild(unit as StructureUnit);
                    return;
                }
                else if (worker.IsHoldingWood && WorkerClickedOnWoodStorage(unit))
                {
                    HandleResourceReturn(worker, unit as StructureUnit);
                    return;
                }
                else if (worker.IsHoldingGold && WorkerClickedOnGoldStorage(unit))
                {
                    HandleResourceReturn(worker, unit as StructureUnit);
                    return;
                }
            }
        }

        SelectNewUnit(unit);
    }

    void HandleResourceReturn(WorkerUnit worker, StructureUnit structure)
    {
        var closestPoint = structure.Collider.ClosestPoint(worker.transform.position);
        worker.MoveTo(closestPoint, DestinationSource.PlayerClick);
        worker.SetTask(UnitTask.ReturnResource);

        if (worker.IsHoldingGold && structure.CanStoreGold)
        {
            worker.SetGoldStorage(structure);
        }
        else if (worker.IsHoldingWood && structure.CanStoreWood)
        {
            worker.SetWoodStorage(structure);
        }

        DisplayClickEffect(structure.transform.position, ClickType.Build);
    }

    bool WorkerClickedOnWoodStorage(Unit clickedUnit)
    {
        return
            (clickedUnit is StructureUnit structure)
            && structure.CanStoreWood;
    }

    bool WorkerClickedOnGoldStorage(Unit clickedUnit)
    {
        return
            (clickedUnit is StructureUnit structure)
            && structure.CanStoreGold;
    }

    void HandleClickOnEnemy(Unit enemyUnit)
    {
        if (HasActiveUnit)
        {
            ActiveUnit.SetTarget(enemyUnit);
            ActiveUnit.SetTask(UnitTask.Attack);
            DisplayClickEffect(enemyUnit.GetTopPosition(), ClickType.Attack);
        }
    }

    bool WorkerClickedOnUnfinishedBuild(Unit clickedUnit)
    {
        return
            clickedUnit is StructureUnit structure &&
            structure.IsUnderConstuction;
    }

    void SelectNewUnit(Unit unit)
    {
        if (unit.CurrentState == UnitState.Dead) return;

        if (HasActiveUnit)
        {
            ActiveUnit.Deselect();
        }

        ShowUnitActions(unit);
        ActiveUnit = unit;
        ActiveUnit.Select();
    }

    bool HasClickedOnActiveUnit(Unit clickedUnit)
    {
        return clickedUnit == ActiveUnit;
    }

    bool IsHumanoid(Unit unit)
    {
        return unit is HumanoidUnit;
    }

    void DisplayClickEffect(Vector2 worldPoint, ClickType clickType)
    {
        if (clickType == ClickType.Move)
        {
            Instantiate(m_PointToMovePrefab, (Vector3)worldPoint, Quaternion.identity);
        }
        else if (clickType == ClickType.Build)
        {
            Instantiate(m_PointToBuildPrefab, (Vector3)worldPoint, Quaternion.identity);
        }
        else if (clickType == ClickType.Attack)
        {
            Instantiate(m_PointToAttackPrefab, (Vector3)worldPoint, Quaternion.identity);
        }
        else if (clickType == ClickType.Chop)
        {
            Instantiate(m_PointToChopPrefab, (Vector3)worldPoint, Quaternion.identity);
        }
    }

    void ShowUnitActions(Unit unit)
    {
        ClearActionBarUI();

        if (unit.Actions.Length == 0)
        {
            return;
        }

        m_ActionBar.Show();

        foreach (var action in unit.Actions)
        {
            m_ActionBar.RegisterAction(
                action.Icon,
                () => action.Execute(this)
            );
        }
    }

    void ClearActionBarUI()
    {
        if (m_BuildConfirmationBar.enabled)
        {
            m_BuildConfirmationBar.Hide();
        }

        m_ActionBar.ClearActions();
        m_ActionBar.Hide();
    }

    void ConfirmBuildPlacement()
    {
        if (((WorkerUnit)ActiveUnit).CurrentState == UnitState.Minig)
        {
            Debug.Log("Worker is minning!");
            return;
        }

        if (!TryDeductResources(m_PlacementProcess.GoldCost, m_PlacementProcess.WoodCost))
        {
            Debug.Log("Not Enough Resources!");
            return;
        }

        if (m_PlacementProcess.TryFinalizePlacement(out Vector3 buildPosition))
        {
            DisplayClickEffect(buildPosition, ClickType.Build);
            m_BuildConfirmationBar.Hide();

            new BuildingProcess(
                m_PlacementProcess.BuildAction,
                buildPosition,
                (WorkerUnit)ActiveUnit,
                m_ConstructionEffectPrefab
            );

            m_PlacementProcess = null;
            m_CameraController.LockCamera = false;
        }
        else
        {
            AddResources(m_PlacementProcess.GoldCost, m_PlacementProcess.WoodCost);
        }
    }

    void CancelBuildPlacement()
    {
        m_BuildConfirmationBar.Hide();
        m_PlacementProcess.Cleanup();
        m_PlacementProcess = null;
        m_CameraController.LockCamera = false;
    }

    bool TryDeductResources(int goldCost, int woodCost)
    {
        if (m_Gold >= goldCost && m_Wood >= woodCost)
        {
            AddResources(-goldCost, -woodCost);
            return true;
        }

        return false;
    }

    void OnGUI()
    {
        if (ActiveUnit != null)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + ActiveUnit.CurrentState.ToString(), new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 160, 200, 20), "Task: " + ActiveUnit.CurrentTask.ToString(), new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 200, 200, 20), "Stance: " + ActiveUnit.CurrentStance.ToString(), new GUIStyle { fontSize = 30 });
        }
    }
}
