

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    private Vector3? m_CurrentDestination;
    private List<Vector3> m_CurrentPath = new();
    private TilemapManager m_TilemapManager;
    private int m_CurrentNodeIndex;

    public UnityAction<Vector3> OnNewPositionSelected = delegate { };
    public UnityAction OnDestinationReached = delegate { };

    void Start()
    {
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            m_CurrentDestination = null;
            return;
        }

        Vector3 targetPosition = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * m_Speed * Time.deltaTime;

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

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}
