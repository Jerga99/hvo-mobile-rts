


using TMPro;
using UnityEngine;

public class ResourceRequirementsDisplay: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_GoldText;
    [SerializeField] private TextMeshProUGUI m_WoodText;

    public void Show(int reqGold, int reqWood)
    {
        m_GoldText.text = reqGold.ToString();
        m_WoodText.text = reqWood.ToString();
        UpdateColorRequirements(reqGold, reqWood);
    }

    void UpdateColorRequirements(int reqGold, int reqWood)
    {
        var manager = GameManager.Get();
        var greenColor = new Color(0, 0.8f, 0, 1f);
        m_GoldText.color = manager.Gold >= reqGold ? greenColor : Color.red;
        m_WoodText.color = manager.Wood >= reqWood ? greenColor : Color.red;
    }
}
