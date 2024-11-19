
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ClickType
{
    Move, Attack, Build
}

public class GameManager : SingletonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToMovePrefab;
    [SerializeField] private PointToClick m_PointToBuildPrefab;
    [SerializeField] private ActionBar m_ActionBar;
    [SerializeField] private ConfirmationBar m_BuildConfirmationBar;

    [Header("VFX")]
    [SerializeField] private ParticleSystem m_ConstructionEffectPrefab;

    public Unit ActiveUnit;

    private PlacementProcess m_PlacementProcess;
    private int m_Gold = 1000;
    private int m_Wood = 1000;

    public int Gold => m_Gold;
    public int Wood => m_Wood;

    public bool HasActiveUnit => ActiveUnit != null;

    void Start()
    {
        ClearActionBarUI();
    }

    void Update()
    {
        if (m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else if (HvoUtils.TryGetShortClickPosition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }
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
    }

    void DetectClick(Vector2 inputPosition)
    {
        if (HvoUtils.IsPointerOverUIElement())
        {
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (HasClickedOnUnit(hit, out var unit))
        {
            HandleClickOnUnit(unit);
        }
        else
        {
            HandleClickOnGround(worldPoint);
        }
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
            ActiveUnit.MoveTo(worldPoint);
        }
    }

    void HandleClickOnUnit(Unit unit)
    {
        if (HasActiveUnit)
        {
            if (HasClickedOnActiveUnit(unit))
            {
                CancelActiveUnit();
                return;
            }
            else if (WorkerClickedOnUnfinishedBuild(unit))
            {
                DisplayClickEffect(unit.transform.position, ClickType.Build);
                ((WorkerUnit)ActiveUnit).SendToBuild(unit as StructureUnit);
                return;
            }
        }

        SelectNewUnit(unit);
    }

    bool WorkerClickedOnUnfinishedBuild(Unit clickedUnit)
    {
        return
            ActiveUnit is WorkerUnit &&
            clickedUnit is StructureUnit structure &&
            structure.IsUnderConstuction;
    }

    void SelectNewUnit(Unit unit)
    {
        if (HasActiveUnit)
        {
            ActiveUnit.Deselect();
        }

        ActiveUnit = unit;
        ActiveUnit.Select();
        ShowUnitActions(unit);
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
        }
        else
        {
            RevertResources(m_PlacementProcess.GoldCost, m_PlacementProcess.WoodCost);
        }
    }

    void RevertResources(int gold, int wood)
    {
        m_Gold += gold;
        m_Wood += wood;
    }

    void CancelBuildPlacement()
    {
        m_BuildConfirmationBar.Hide();
        m_PlacementProcess.Cleanup();
        m_PlacementProcess = null;
    }

    bool TryDeductResources(int goldCost, int woodCost)
    {
        if (m_Gold >= goldCost && m_Wood >= woodCost)
        {
            m_Gold -= goldCost;
            m_Wood -= woodCost;
            return true;
        }

        return false;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 40, 200, 20), "Gold: " + m_Gold.ToString(), new GUIStyle { fontSize = 30 });
        GUI.Label(new Rect(20, 80, 200, 20), "Wood: " + m_Wood.ToString(), new GUIStyle { fontSize = 30 });

        if (ActiveUnit != null)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + ActiveUnit.CurrentState.ToString(), new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 160, 200, 20), "Task: " + ActiveUnit.CurrentTask.ToString(), new GUIStyle { fontSize = 30 });
        }
    }
}
