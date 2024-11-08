

using UnityEngine;

public abstract class ActionSO: ScriptableObject
{
    public Sprite Icon;
    public string ActionName;
    public string Guid = System.Guid.NewGuid().ToString();

    public abstract void Execute(GameManager manager);
}
