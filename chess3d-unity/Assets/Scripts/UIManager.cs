using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverDesc;

    public event Action OnRestartClick;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(string description)
    {
        gameOverDesc.text = description;
        gameOverPanel.SetActive(true);
    }

    public void RestartClick()
    {
        gameOverPanel.SetActive(false);
        OnRestartClick?.Invoke();
    }
}
