

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen: MonoBehaviour
{
    [SerializeField] private AudioSettings m_MenuBgAudioSettings;
    [SerializeField] private Button m_PlayBtn;
    [SerializeField] private Button m_ExitBtn;

    void OnEnable()
    {
        m_PlayBtn.onClick.AddListener(OnPlayBtnClicked);
        m_ExitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    void OnDisable()
    {
        m_PlayBtn.onClick.RemoveListener(OnPlayBtnClicked);
        m_ExitBtn.onClick.RemoveListener(OnExitBtnClicked);
    }

    void Start()
    {
        AudioManager.Get().PlayMusic(m_MenuBgAudioSettings);
    }

    void OnPlayBtnClicked()
    {
        SceneManager.LoadScene("PlayScene");
    }

    void OnExitBtnClicked()
    {
        Application.Quit();
    }
}
