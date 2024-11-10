

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton: MonoBehaviour
{
    [SerializeField] private Image m_IconImage;
    [SerializeField] private Button m_Button;

    public void Init(Sprite icon, UnityAction action)
    {
        m_IconImage.sprite = icon;
        m_Button.onClick.AddListener(action);
    }
}
