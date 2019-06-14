using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private TextMeshProUGUI gameOverDesc;

    public event Action OnRestartClick;

    public bool IsMenuActive => gameOverPanel.activeSelf || escapePanel.activeSelf;

    void Start()
    {
        gameOverPanel.SetActive(false);
        escapePanel.SetActive(false);
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
    }

    public void RestartClick()
    {
        gameOverPanel.SetActive(false);
        escapePanel.SetActive(false);
        OnRestartClick?.Invoke();
    }

    public void ExitMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }
}
