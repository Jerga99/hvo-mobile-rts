


using UnityEngine;

public class PointToClick : MonoBehaviour
{
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private float m_Timer;

    void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_Duration * 0.9f)
        {
            float fadeProgress = (m_Timer - m_Duration * 0.9f) / (m_Duration * 0.1f);
            m_SpriteRenderer.color = new Color(1, 1, 1, 1 - fadeProgress);
        }

        if (m_Timer >= m_Duration)
        {
            Destroy(gameObject);
        }
    }
}
