
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

    public Unit ActiveUnit;

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

    public bool HasActiveUnit => ActiveUnit != null;

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
                    var closestPoint = unit.Collider.ClosestPoint(worker.transform.position);
                    worker.MoveTo(closestPoint, DestinationSource.PlayerClick);
                    worker.SetTask(UnitTask.ReturnResource);
                    worker.SetWoodStorage(unit as StructureUnit);
                    DisplayClickEffect(unit.transform.position, ClickType.Build);
                    return;
                }
            }
        }

        SelectNewUnit(unit);
    }

    bool WorkerClickedOnWoodStorage(Unit clickedUnit)
    {
        return
            (clickedUnit is StructureUnit structure)
            && structure.CanStoreWood;
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

    void CancelActiveUnit()
    {
        ActiveUnit.Deselect();
        ActiveUnit = null;

        ClearActionBarUI();
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
        m_ActionBar.ClearActions();
        m_ActionBar.Hide();
    }

    void ConfirmBuildPlacement()
    {
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
