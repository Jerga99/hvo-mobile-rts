

using System.Collections.Generic;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    private Vector3? m_Destination;
    private List<Node> m_CurrentPath;
    private TilemapManager m_TilemapManager;

    public Vector3? Destination => m_Destination;

    void Start()
    {
        m_TilemapManager = TilemapManager.Get();
    }

    void Update()
    {
        if (m_Destination.HasValue)
        {
            var dir = m_Destination.Value - transform.position;
            transform.position += dir.normalized * Time.deltaTime * m_Speed;

            var distanceToDestination = Vector3.Distance(transform.position, m_Destination.Value);

            if (distanceToDestination < 0.1f)
            {
                m_Destination = null;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        m_CurrentPath = m_TilemapManager.FindPath(transform.position, destination);
        Debug.Log("Path: " + string.Join(", ", m_CurrentPath));
        m_Destination = destination;
    }
}
