




using UnityEngine;
using UnityEngine.UI;

public class ConfirmationBar: MonoBehaviour
{
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
