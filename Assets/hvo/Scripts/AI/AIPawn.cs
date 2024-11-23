

using System.Collections.Generic;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    private Vector3? m_Destination;
    private List<Node> m_CurrentPath = new();
    private TilemapManager m_TilemapManager;
    private int m_CurrentNodeIndex;

    public Vector3? Destination => m_Destination;

    void Start()
    {
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            m_Destination = null;
            return;
        }

        Node currentNode = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 targetPosition = new Vector3(currentNode.centerX, currentNode.centerY);
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * m_Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            m_CurrentNodeIndex++;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        Debug.Log("Path: " + string.Join(", ", m_CurrentPath));
        m_Destination = destination;
        m_CurrentNodeIndex = 0;
    }

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}
