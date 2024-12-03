

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    [Header("Separation")]
    [SerializeField] private float m_SeparationRadius = 1f;
    [SerializeField] private float m_SeparationForce = 0.5f;
    [SerializeField] private bool m_ApplySeparation = true;

    private Vector3? m_CurrentDestination;
    private List<Vector3> m_CurrentPath = new();
    private TilemapManager m_TilemapManager;
    private int m_CurrentNodeIndex;
    private GameManager m_GameManager;

    public UnityAction<Vector3> OnNewPositionSelected = delegate { };
    public UnityAction OnDestinationReached = delegate { };

    void Start()
    {
        m_GameManager = GameManager.Get();
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            m_CurrentDestination = null;
            return;
        }


        Vector3 separationVector = m_ApplySeparation ? CalculateSeparation() : Vector3.zero;
        Vector3 targetPosition = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 combinedDirection = direction + separationVector;

        if (combinedDirection.magnitude > 1f)
        {
            combinedDirection.Normalize();
        }

        transform.position += combinedDirection * m_Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
            {
                OnDestinationReached.Invoke();
                m_CurrentPath = new();
            }
            else
            {
                m_CurrentNodeIndex++;
                OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        if (m_CurrentDestination.HasValue && Vector3.Distance(m_CurrentDestination.Value, destination) < 0.1f)
        {
            return;
        }

        m_CurrentDestination = destination;
        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        m_CurrentNodeIndex = 0;
        OnNewPositionSelected.Invoke(m_CurrentPath[m_CurrentNodeIndex]);
    }

    public void Stop()
    {
        m_CurrentPath.Clear();
        m_CurrentNodeIndex = 0;
    }

    private Unit m_Unit;
    protected virtual bool GetPlayerStatus()
    {
        if (m_Unit != null)
        {
            return m_Unit.IsPlayer;
        }

        m_Unit = GetComponent<Unit>();
        return m_Unit.IsPlayer;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separationVector = Vector3.zero;
        float separationRadiusSqr = m_SeparationRadius * m_SeparationRadius;
        List<Unit> units = m_GameManager.GetFriendlyUnits(GetPlayerStatus());

        foreach(var unit in units)
        {
            if (unit.gameObject == gameObject) continue;

            Vector3 opositeDirection = transform.position - unit.transform.position;
            float sqrDistance = opositeDirection.sqrMagnitude;

            if (sqrDistance < separationRadiusSqr && sqrDistance > 0)
            {
                separationVector += opositeDirection.normalized / sqrDistance;
            }
        }

        return separationVector * m_SeparationForce;
    }

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}
