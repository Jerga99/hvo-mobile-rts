


using UnityEngine;

public enum UnitStance
{
    Defensive, Offensive
}

[CreateAssetMenu(fileName = "StanceAction", menuName = "HvO/Actions/UnitStanceAction")]
public class UnitStanceActionSO : ActionSO
{
    [SerializeField] private UnitStance m_UnitStance;

    public UnitStance UnitStance => m_UnitStance;

    public override void Execute(GameManager manager)
    {
        if (manager.ActiveUnit != null)
        {
            Debug.Log("Change state to: " + m_UnitStance.ToString());
        }
    }
}
