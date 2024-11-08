


using UnityEngine;
using UnityEngine.UI;

public class ActionBar: MonoBehaviour
{
    [SerializeField] private Image m_BackgroundImage;

    private Color m_OriginalBackgroundColor;

    void Awake()
    {
        m_OriginalBackgroundColor = m_BackgroundImage.color;
        Hide();
    }

    public void Show()
    {
        m_BackgroundImage.color = m_OriginalBackgroundColor;
    }

    public void Hide()
    {
        m_BackgroundImage.color = new Color(0, 0, 0, 0);
    }
}
