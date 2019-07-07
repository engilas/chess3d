﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private TextMeshProUGUI gameOverDesc;
    [SerializeField] private Toggle frontViewToggle;

    private CameraManager cameraManager;

    public event Action OnRestartClick;

    //public bool IsMenuActive => gameOverPanel.activeSelf || escapePanel.activeSelf;

    private bool _frontToggleEnabled = false;

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        gameOverPanel.SetActive(false);
        escapePanel.SetActive(false);

        frontViewToggle.isOn = !Settings.FrontView;
        _frontToggleEnabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            escapePanel.SetActive(!escapePanel.activeSelf);
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
        gameOverPanel.SetActive(false);
        escapePanel.SetActive(false);
        PlayerLock.MenuLock = false;
        OnRestartClick?.Invoke();
    }

    public void ExitMenuClick()
    {
        SceneManager.LoadScene("Menu");
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
}
