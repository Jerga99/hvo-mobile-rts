


using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{



    void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            DeteckClick(inputPosition);
        }
    }

    void DeteckClick(Vector2 inputPosition)
    {
        Debug.Log(inputPosition);
    }
}
