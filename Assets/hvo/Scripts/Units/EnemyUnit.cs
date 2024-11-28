

using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    public override bool IsPlayer => false;

    protected override void UpdateBehaviour()
    {
        if (TryFindClosestFoe(out var foe))
        {
            Debug.Log(foe.gameObject.name);
        }
    }
}
