


using UnityEngine;

public class TextPopupController: MonoBehaviour
{
    [SerializeField] private TextPopup m_TextPopupPrefab;

    public void Spawn(string text, Vector3 position, Color color)
    {
        var textPopup = Instantiate(m_TextPopupPrefab);
        textPopup.transform.position = position;
        textPopup.SetText(text, color);
    }
}
