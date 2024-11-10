

using UnityEngine;

public class PlacementProcess
{
    private BuildActionSO m_BuildAction;

    public PlacementProcess(BuildActionSO buildAction)
    {
        m_BuildAction = buildAction;
    }

    public void Update()
    {
        Debug.Log("PlacementProcess Update()");
    }

    public void ShowPlacementOutline()
    {
        Debug.Log("ShowPlacementOutline");
    }
}
