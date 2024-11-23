

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    private List<Node> m_CurrentPath = new();
    private TilemapManager m_TilemapManager;
    private int m_CurrentNodeIndex;

    void Start()
    {
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (!IsPathValid())
        {
            return;
        }

        Node currentNode = m_CurrentPath[m_CurrentNodeIndex];
        Vector3 targetPosition = new Vector3(currentNode.centerX, currentNode.centerY);
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * m_Speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            if (m_CurrentNodeIndex == m_CurrentPath.Count - 1)
            {
                Debug.Log("Destiantion Reached!");
                m_CurrentPath = new();
            }
            else
            {
                m_CurrentNodeIndex++;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        if (m_CurrentPath.Count > 0)
        {
            Node newEndNode = m_TilemapManager.FindNode(destination);

            if (newEndNode == m_CurrentPath[^1])
            {
                return;
            }
        }

        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        m_CurrentNodeIndex = 0;
    }

    bool IsPathValid()
    {
        return m_CurrentPath.Count > 0 && m_CurrentNodeIndex < m_CurrentPath.Count;
    }
}
