

using UnityEngine;
using TMPro;

public class ResourceDataUI: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_GoldText;
    [SerializeField] private TextMeshProUGUI m_WoodText;

    public void UpdateResourceDisplay(int gold, int wood)
    {
        m_GoldText.text = gold.ToString();
        m_WoodText.text = wood.ToString();
    }
}
