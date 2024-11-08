
using UnityEngine;

[CreateAssetMenu(fileName = "BuildAction", menuName = "HvO/Actions/BuildAction")]
public class BuildActionSO: ActionSO
{
    [SerializeField] private Sprite m_PlacementSprite;
    [SerializeField] private Sprite m_FoundationSprite;
    [SerializeField] private Sprite m_CompletionSprite;

    [SerializeField] private int m_GoldCost;
    [SerializeField] private int m_WoodCost;

    public Sprite PlacementSprite => m_PlacementSprite;
    public Sprite FoundationSprite => m_FoundationSprite;
    public Sprite CompletionSprite => m_CompletionSprite;

    public int GoldCost => m_GoldCost;
    public int WoodCost => m_WoodCost;


    public override void Execute(GameManager manager)
    {
    }
}
