




using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar: MonoBehaviour
{
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    void OnDisable()
    {
        m_ConfirmButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.RemoveAllListeners();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetupHooks(UnityAction onConfirm, UnityAction onCancel)
    {
        m_ConfirmButton.onClick.AddListener(onConfirm);
        m_CancelButton.onClick.AddListener(onCancel);
    }
}
