

using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 5f;

    private Vector3? m_Destination;

    public Vector3? Destination => m_Destination;

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
        m_Destination = destination;
    }
}
