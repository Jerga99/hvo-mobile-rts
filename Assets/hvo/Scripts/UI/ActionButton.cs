

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton: MonoBehaviour
{
    [SerializeField] private Image m_IconImage;
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private Sprite m_ButtonFocusedSprite;

    [SerializeField] private Sprite m_ButtonDefaultSprite;

    void OnDestroy()
    {
        m_Button.onClick.RemoveAllListeners();
    }

    public void Init(Sprite icon, UnityAction action)
    {
        m_IconImage.sprite = icon;
        m_Button.onClick.AddListener(action);
    }

    public void Focus()
    {
        m_ButtonImage.sprite = m_ButtonFocusedSprite;
    }

    public void Unfocus()
    {
        m_ButtonImage.sprite = m_ButtonDefaultSprite;
    }
}
