

using UnityEngine;
using UnityEngine.UI;

public class ActionButton: MonoBehaviour
{
    [SerializeField] private Image m_IconImage;
    [SerializeField] private Image m_ButtonImage;

    public void Init(Sprite icon)
    {
        m_IconImage.sprite = icon;
    }
}
