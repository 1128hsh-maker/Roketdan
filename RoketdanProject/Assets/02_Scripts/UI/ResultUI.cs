using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;

    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnVictory += ShowVictory;
            waveManager.OnDefeat += ShowDefeat;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnVictory -= ShowVictory;
            waveManager.OnDefeat -= ShowDefeat;
        }
    }

    private void Start()
    {
        HideAll();
    }

    public void HideAll()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (defeatPanel != null)
            defeatPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        HideAll();

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Debug.Log("[ResultUI] 승리 패널 표시");
    }

    public void ShowDefeat()
    {
        HideAll();

        if (defeatPanel != null)
            defeatPanel.SetActive(true);

        Debug.Log("[ResultUI] 패배 패널 표시");
    }

    public void RestartCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
