using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _onlinePanel;
        [SerializeField] private GameObject _waitConnectPanel;
        [SerializeField] private GameObject _offlinePanel;
        [SerializeField] private GameObject _playAiPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private GameObject _errorPanel;

        [SerializeField] private GameObject _exitButton;
        [SerializeField] private Slider _difficultySlider;
        [SerializeField] private TMP_InputField _serverIpInput;
        [SerializeField] private TextMeshProUGUI _errorTitle;
        [SerializeField] private TextMeshProUGUI _errorDesc;

        private GameObject[] _panels;

        private void Start()
        {
            _panels = new[]
            {
                _mainPanel, _onlinePanel, _waitConnectPanel, _offlinePanel, _playAiPanel, _settingsPanel,
                _errorPanel
            };
            SetActivePanel(_mainPanel);

            var platformsWithExitButton = new[]
            {
                RuntimePlatform.LinuxPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.OSXPlayer,
                RuntimePlatform.OSXEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor
            };

            Debug.Log("Running platform: " + Application.platform);

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

        public void StopConnect()
        {
            OnlineManager.StopConnection();
            SetActivePanel(_onlinePanel);
        }

        public void SettingsClick()
        {
            _serverIpInput.text = Settings.ServerIp;
            SetActivePanel(_settingsPanel);
        }

        public void SettingsSave()
        {
            var ip = _serverIpInput.text;
            if (Uri.TryCreate(ip, UriKind.Absolute, out _))
            {
                Settings.ServerIp = ip;
            }
            else
            {
                Debug.LogError("Invalid ip " + ip);
            }

            GoMainMenu();
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
        }

        private void ShowErrorPanel(string title, string desc)
        {
            _errorTitle.text = title;
            _errorDesc.text = desc;
            SetActivePanel(_errorPanel);
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
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (OnlineManager.IsConnectionFailed())
                {
                    var exn = OnlineManager.GetConnectionError();
                    var errorTitle = "Connection failed";
                    if (exn == null)
                    {
                        ShowErrorPanel(errorTitle, "Something goes wrong");
                    }
                    else
                    {
                        var errorDesc = exn is HttpRequestException ? "Can't connect to the server" : exn.Message;
                        ShowErrorPanel(errorTitle, errorDesc);
                    }
                
                    Debug.LogError("Connection failed. \n" + exn);
                    yield break;
                }

                if (OnlineManager.IsConnected())
                {
                    Debug.Log("Connected");

                    SceneManager.LoadScene("MainScene");
                    yield break;
                }
            }
        }
    }
}
