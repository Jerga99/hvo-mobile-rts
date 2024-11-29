


using TMPro;
using UnityEngine;

public class TextPopup: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;

    public void SetText(string text, Color color)
    {
        m_Text.color = color;
        m_Text.text = text;
    }
}
