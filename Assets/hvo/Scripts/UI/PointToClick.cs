


using UnityEngine;

public class PointToClick : MonoBehaviour
{
    [SerializeField] private float m_Duration = 1f;

    private float m_Timer;

    void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_Duration)
        {
            Destroy(gameObject);
        }
    }
}
