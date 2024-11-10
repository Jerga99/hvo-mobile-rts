

using UnityEngine;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;

    public PlacementProcess(BuildActionSO buildAction)
    {
        m_BuildAction = buildAction;
    }

    public void Update()
    {
        Vector2 screenPosition = Input.touchCount > 0 ?
            Input.GetTouch(0).position :
            Input.GetMouseButton(0) ? Input.mousePosition : Vector2.zero;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (screenPosition != Vector2.zero)
        {
            m_PlacementOutline.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        }
    }

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999;
        renderer.color = new Color(1, 1, 1, 0.5f);
        renderer.sprite = m_BuildAction.PlacementSprite;

    }
}
