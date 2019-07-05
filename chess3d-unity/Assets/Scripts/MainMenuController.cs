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
    [SerializeField] private GameObject _offlinePanel;
    [SerializeField] private GameObject _playAiPanel;
    [SerializeField] private Slider _difficultySlider;

    private const string DifficultyPrefKey = "DifficultyPrefKey";

    private void Start()
    {
        var platformsWithExitButton = new[] { RuntimePlatform.LinuxPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.OSXPlayer, RuntimePlatform.OSXEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor };

        if (!platformsWithExitButton.Contains(Application.platform))
            _exitButton.SetActive(false);
        else _exitButton.SetActive(true);

        _mainPanel.SetActive(true);
        _offlinePanel.SetActive(false);
        _playAiPanel.SetActive(false);

        if (PlayerPrefs.HasKey(DifficultyPrefKey))
        {
            var diff = PlayerPrefs.GetInt(DifficultyPrefKey);
            Settings.Difficulty = diff;
            _difficultySlider.value = diff;
        } else
        {
            var diff = Settings.DefaultDifficulty;
            SetDifficulty(diff);
            _difficultySlider.value = diff;
        }
    }
    
    public void PlayWithAi()
    {
        _offlinePanel.SetActive(false);
        _playAiPanel.SetActive(true);
    }

    public void StartPlayWithAi()
    {
        Settings.GameMode = GameMode.OfflineAi;
        SceneManager.LoadScene("MainScene");
    }

    public void StartPlayOfflineMp()
    {
        Settings.GameMode = GameMode.OfflineMp;
        SceneManager.LoadScene("MainScene");
    }

    public void PlayOffline()
    {
        _mainPanel.SetActive(false);
        _offlinePanel.SetActive(true);
    }

    public void OfflinePanelBack()
    {
        _mainPanel.SetActive(true);
        _offlinePanel.SetActive(false);
    }

    public void PlayAiPanelBack()
    {
        _playAiPanel.SetActive(false);
        _offlinePanel.SetActive(true);
    }

    public void OnDifficultyChanged(float value)
    {
        SetDifficulty((int)value);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void SetDifficulty(int value)
    {
        Settings.Difficulty = value;
        PlayerPrefs.SetInt(DifficultyPrefKey, value);
    }
}
