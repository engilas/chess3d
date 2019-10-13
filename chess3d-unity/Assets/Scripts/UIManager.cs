using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject gameOverRestartButton;
    [SerializeField] private GameObject escapeRestartButton;
    [SerializeField] private TextMeshProUGUI gameOverDesc;
    [SerializeField] private TextMeshProUGUI confirmDesc;
    [SerializeField] private Toggle frontViewToggle;
    [SerializeField] private Toggle menuToggle;

    private CameraManager cameraManager;
    private Action _confirmAction;

    public event Action OnRestartClick;
    public event Action OnExitClick;

    //public bool IsMenuActive => gameOverPanel.activeSelf || escapePanel.activeSelf;

    private bool _frontToggleEnabled = false;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        ResetUi();

        frontViewToggle.isOn = !Settings.FrontView;
        _frontToggleEnabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            menuToggle.isOn = !menuToggle.isOn;
        }
    }

    public void OnToggleMenu()
    {
        confirmPanel.SetActive(false);
        escapePanel.SetActive(!menuToggle.isOn);
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
        gameOverPanel.SetActive(true);
        PlayerLock.MenuLock = true;
    }

    public void RestartClick()
    {
        SetupConfirm("Restart", () =>
        {
            ResetUi();
            PlayerLock.MenuLock = false;
            OnRestartClick?.Invoke();
        });
    }

    public void ExitMenuClick()
    {
        SetupConfirm("Quit", ExitAction);
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
}
