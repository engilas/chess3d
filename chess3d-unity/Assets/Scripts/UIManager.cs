using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject escapePanel;
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private GameObject reconnectingPanel;

        [SerializeField] private GameObject gameOverRestartButton;
        [SerializeField] private GameObject escapeRestartButton;

        [SerializeField] private TextMeshProUGUI gameOverDesc;
        [SerializeField] private TextMeshProUGUI confirmDesc;

        [SerializeField] private Toggle frontViewToggle;
        [SerializeField] private Toggle menuToggle;

        private GameObject[] _panels;

        private CameraManager cameraManager;
        private Action _confirmAction;

        public event Action OnRestartClick;
        public event Action OnExitClick;

        private bool _frontToggleEnabled = false;

        void Start()
        {
            _panels = new[] { gameOverPanel, escapePanel, confirmPanel, reconnectingPanel };

            cameraManager = FindObjectOfType<CameraManager>();
            ResetUi();

            frontViewToggle.isOn = !Settings.FrontView;
            _frontToggleEnabled = true;
        }

        void Update()
        {
            if (Input.GetKeyDown("escape") && menuToggle.interactable)
            {
                menuToggle.isOn = !menuToggle.isOn;
            }

            if (OnlineManager.IsReconnecting) ActivateMenu(reconnectingPanel);
            if (!OnlineManager.IsReconnecting && reconnectingPanel.activeSelf) 
                DeactivateMenu(reconnectingPanel);
            if (OnlineManager.IsClosed) ShowGameOver("Connection lost");
        }

        public void OnToggleMenu()
        {
            confirmPanel.SetActive(false);
            SetActiveMenu(escapePanel, !menuToggle.isOn, false);
        }

        public void CloseMenu()
        {
            if (!menuToggle.isOn)
            {
                menuToggle.isOn = true;
            }
        }

        public void ShowGameOver(string description)
        {
            gameOverDesc.text = description;
            ActivateMenu(gameOverPanel);
        }

        public void RestartClick()
        {
            SetupConfirm("Restart", RestartAction);
        }

        public void ExitMenuClick()
        {
            SetupConfirm("Quit", ExitAction);
        }

        public void RestartGameOverClick()
        {
            RestartAction();
        }

        public void ExitGameOverClick()
        {
            ExitAction();
        }

        public void ConfirmClick()
        {
            _confirmAction?.Invoke();
        }

        public void RejectClick()
        {
            confirmPanel.SetActive(false);
            escapePanel.SetActive(true);
        }

        public void TopViewClick()
        {
            if (_frontToggleEnabled)
            {
                if (!PlayerLock.CameraLock)
                    cameraManager.ToggleFrontView();
                else
                {
                    _frontToggleEnabled = false;
                    frontViewToggle.isOn = !frontViewToggle.isOn;
                    _frontToggleEnabled = true;
                }
            }
        }

        public void EnableRestartButtons(bool enabled)
        {
            gameOverRestartButton.SetActive(enabled);
            escapeRestartButton.SetActive(enabled);
        }

        private void ResetUi()
        {
            CloseMenu();
            gameOverPanel.SetActive(false);
            escapePanel.SetActive(false);
            confirmPanel.SetActive(false);
            reconnectingPanel.SetActive(false);
            menuToggle.interactable = true;
        }

        private void SetupConfirm(string title, Action confirmAction)
        {
            escapePanel.SetActive(false);
            confirmPanel.SetActive(true);
            confirmDesc.text = $"{title} the game?";
            _confirmAction = confirmAction;
        }

        private void ExitAction()
        {
            OnExitClick?.Invoke();
            SceneManager.LoadScene("Menu");
        }

        private void RestartAction()
        {
            ResetUi();
            PlayerLock.MenuLock = false;
            OnRestartClick?.Invoke();
        }

        private void ActivateMenu(GameObject menu) => SetActiveMenu(menu, true);

        private void DeactivateMenu(GameObject menu) => SetActiveMenu(menu, false);

        private void SetActiveMenu(GameObject menu, bool active, bool blockEscapeMenu = true)
        {
            foreach (var panel in _panels)
            {
                panel.SetActive(false);
            }

            PlayerLock.MenuLock = active;
            if (blockEscapeMenu)
            {
                menuToggle.interactable = !active;
            }
            menu.SetActive(active);
        }
    }
}
