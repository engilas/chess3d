using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _onlinePanel;
    [SerializeField] private GameObject _waitConnectPanel;
    [SerializeField] private GameObject _offlinePanel;
    [SerializeField] private GameObject _playAiPanel;
    [SerializeField] private Slider _difficultySlider;

    private GameObject[] _panels;

    private void Start()
    {
        _panels = new[] {_exitButton, _mainPanel, _onlinePanel, _waitConnectPanel, _offlinePanel, _playAiPanel};
        SetActivePanel(_mainPanel);

        var platformsWithExitButton = new[]
        {
            RuntimePlatform.LinuxPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.OSXPlayer,
            RuntimePlatform.OSXEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor
        };

        if (!platformsWithExitButton.Contains(Application.platform))
            _exitButton.SetActive(false);
        else _exitButton.SetActive(true);

        _difficultySlider.value = Settings.Difficulty;
    }
    
    public void PlayOfflineWithAi()
    {
        SetActivePanel(_playAiPanel);
    }

    public void StartPlayOfflineWithAi()
    {
        Settings.GameMode = GameMode.OfflineAi;
        SceneManager.LoadScene("MainScene");
    }

    public void StartPlayOfflineMp()
    {
        Settings.GameMode = GameMode.OfflineMp;
        SceneManager.LoadScene("MainScene");
    }

    public void StartPlayOnlineQuick()
    {
        Settings.GameMode = GameMode.OnlineQuick;
        SetActivePanel(_waitConnectPanel);
        StartCoroutine(ConnectCoroutine());
    }

    public void PlayOffline()
    {
        SetActivePanel(_offlinePanel);
    }

    public void PlayOnline()
    {
        SetActivePanel(_onlinePanel);
    }

    public void GoMainMenu()
    {
        SetActivePanel(_mainPanel);
    }

    public void PlayAiPanelBack()
    {
        SetActivePanel(_offlinePanel);
    }

    public void OnDifficultyChanged(float value)
    {
        Settings.Difficulty = (int) value;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void SetActivePanel(GameObject activePanel)
    {
        foreach (var panel in _panels)
        {
            panel.SetActive(false);
        }
        activePanel.SetActive(true);
    }

    private IEnumerator ConnectCoroutine()
    {
        OnlineManager.Connect();
        int timeoutSeconds = 100;
        int seconds = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
            if (OnlineManager.IsFailed())
            {
                var exn = OnlineManager.GetError();
                Debug.Log("Failed. " + exn?.Message);
                yield break;
            }

            if (OnlineManager.IsConnected())
            {
                Debug.Log("Connected");
                yield break;
            }

            if (seconds >= timeoutSeconds)
            {
                yield break;
            }
        }
    }
}
