


using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{

    private Vector2 m_InitialTouchPosition;

    void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            m_InitialTouchPosition = inputPosition;
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10)
            {
                DeteckClick(inputPosition);
            }
        }
    }

    void DeteckClick(Vector2 inputPosition)
    {
        Debug.Log(inputPosition);
    }
}
