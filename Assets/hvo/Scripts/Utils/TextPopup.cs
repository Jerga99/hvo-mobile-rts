


using TMPro;
using UnityEngine;

public class TextPopup: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private AnimationCurve m_FontSizeCurve;
    [SerializeField] private AnimationCurve m_XOffsetCurve;
    [SerializeField] private AnimationCurve m_YOffsetCurve;
    [SerializeField] private AnimationCurve m_AlphaCurve;

    private float m_ElapsedTime;
    private int m_RandomXDirection = 1;

    void Start()
    {
        m_RandomXDirection = Random.Range(-1, 2);
    }

    public void SetText(string text, Color color)
    {
        m_Text.color = color;
        m_Text.text = text;
    }

    void Update()
    {
        m_ElapsedTime += Time.deltaTime;
        var normalizedTime = m_ElapsedTime / m_Duration;

        if (normalizedTime >= 1)
        {
            Destroy(gameObject);
            return;
        }

        var alpha = m_AlphaCurve.Evaluate(normalizedTime);
        m_Text.fontSize += m_FontSizeCurve.Evaluate(normalizedTime) / 5;
        m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);
        float xOffset = m_XOffsetCurve.Evaluate(normalizedTime) * 1.1f * m_RandomXDirection;
        float yOffset = m_YOffsetCurve.Evaluate(normalizedTime) * 1.1f;

        transform.position += new Vector3(xOffset, yOffset, 0) * Time.deltaTime;
    }
}
