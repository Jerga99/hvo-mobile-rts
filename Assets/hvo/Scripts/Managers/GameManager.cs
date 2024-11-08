
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : SingletonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_PointToClickPrefab;
    [SerializeField] private ActionBar m_ActionBar;

    public Unit ActiveUnit;

    private Vector2 m_InitialTouchPosition;

    public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    public bool IsLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    public bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    public bool HasActiveUnit => ActiveUnit != null;

    void Start()
    {
        ClearActionBarUI();
    }

    void Update()
    {
        if (IsLeftClickOrTapDown)
        {
            m_InitialTouchPosition = InputPosition;
        }

        if (IsLeftClickOrTapUp)
        {
            if (Vector2.Distance(m_InitialTouchPosition, InputPosition) < 5)
            {
                DetectClick(InputPosition);
            }
        }
    }

    void DetectClick(Vector2 inputPosition)
    {
        if (IsPointerOverUIElement())
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
            DisplayClickEffect(worldPoint);
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
        }

        SelectNewUnit(unit);
    }

    void SelectNewUnit(Unit unit)
    {
        if (HasActiveUnit)
        {
            ActiveUnit.Deselect();
        }

        ActiveUnit = unit;
        ActiveUnit.Select();
        ShowUnitActions();
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

    void DisplayClickEffect(Vector2 worldPoint)
    {
        Instantiate(m_PointToClickPrefab, (Vector3)worldPoint, Quaternion.identity);
    }

    void ShowUnitActions()
    {
        ClearActionBarUI();

        var hardcodedAtions = 2;

        for (int i = 0; i < hardcodedAtions; i++)
        {
            m_ActionBar.RegisterAction();
        }

        m_ActionBar.Show();
    }

    void ClearActionBarUI()
    {
        m_ActionBar.ClearActions();
        m_ActionBar.Hide();
    }

    bool IsPointerOverUIElement()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        else
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
