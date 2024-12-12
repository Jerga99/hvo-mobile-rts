


using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverLayout: MonoBehaviour
{
    [SerializeField] private Button m_RestartBtn;
    [SerializeField] private Button m_QuitBtn;
    [SerializeField] private TextMeshProUGUI m_GameOverText;
    [SerializeField] private Image m_BackDropImage;

    public UnityAction OnRestartClicked = delegate { };
    public UnityAction OnQuitClicked = delegate { };

    void OnEnable()
    {
        m_RestartBtn.onClick.AddListener(RestartGame);
        m_QuitBtn.onClick.AddListener(QuitGame);
    }

    void OnDisable()
    {
        m_RestartBtn.onClick.RemoveListener(RestartGame);
        m_QuitBtn.onClick.RemoveListener(QuitGame);
    }

    public void ShowGameOver(bool isVictory)
    {
        m_GameOverText.text = isVictory ? "Victory!" : "Defeat!";
        gameObject.SetActive(true);
        m_BackDropImage.color = new Color(0, 0, 0, 0.3f);
    }

    void RestartGame()
    {
        AudioManager.Get().PlayBtnClick();
        OnRestartClicked.Invoke();
    }

    void QuitGame()
    {
        AudioManager.Get().PlayBtnClick();
        OnQuitClicked.Invoke();
    }


}
