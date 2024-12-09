




using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar: MonoBehaviour
{
    [SerializeField] private ResourceRequirementsDisplay m_ResourceDisplay;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    void OnDisable()
    {
        UnsubscribeAll();
    }

    public void Show(int gold, int wood)
    {
        gameObject.SetActive(true);
        m_ResourceDisplay.Show(gold, wood);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateRequirementsUI(int gold, int wood)
    {
        m_ResourceDisplay.UpdateColorRequirements(gold, wood);
    }

    public void SetupHooks(UnityAction onConfirm, UnityAction onCancel)
    {
        UnsubscribeAll();

        m_ConfirmButton.onClick.AddListener(onConfirm);
        m_CancelButton.onClick.AddListener(onCancel);
    }

    void UnsubscribeAll()
    {
        m_ConfirmButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.RemoveAllListeners();
    }
}
