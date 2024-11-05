


using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameManager[] managers = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static GameManager Get()
    {
        var tag = nameof(GameManager);
        GameObject managerObject = GameObject.FindWithTag(tag);
        if (managerObject != null)
        {
            return managerObject.GetComponent<GameManager>();
        }

        GameObject go = new(tag);
        go.tag = tag;
        return go.AddComponent<GameManager>();
    }

    public void Test()
    {
        Debug.Log("Hello World!");
    }
}
