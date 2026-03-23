using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private MineralManager mineralManager;
    [SerializeField] private WaveManager waveManager;

    [Header("Texts")]
    [SerializeField] private TMP_Text currentGoldText;
    [SerializeField] private TMP_Text currentMineralText;
    [SerializeField] private TMP_Text currentWaveText;

    private void OnEnable()
    {
        if (currencyManager != null)
            currencyManager.OnGoldChanged += HandleGoldChanged;

        if (mineralManager != null)
            mineralManager.OnMineralChanged += HandleMineralChanged;

        if (waveManager != null)
        {
            waveManager.OnWaveStarted += HandleWaveStarted;
            waveManager.OnWaveCleared += HandleWaveCleared;
        }
    }

    private void OnDisable()
    {
        if (currencyManager != null)
            currencyManager.OnGoldChanged -= HandleGoldChanged;

        if (mineralManager != null)
            mineralManager.OnMineralChanged -= HandleMineralChanged;

        if (waveManager != null)
        {
            waveManager.OnWaveStarted -= HandleWaveStarted;
            waveManager.OnWaveCleared -= HandleWaveCleared;
        }
    }

    private void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        if (currencyManager != null)
            HandleGoldChanged(currencyManager.Gold);

        if (mineralManager != null)
            HandleMineralChanged(mineralManager.Mineral);

        RefreshWaveText();
    }

    private void HandleGoldChanged(int currentGold)
    {
        if (currentGoldText != null)
            currentGoldText.text = currentGold.ToString();
    }

    private void HandleMineralChanged(int currentMineral)
    {
        if (currentMineralText != null)
            currentMineralText.text = currentMineral.ToString();
    }

    private void HandleWaveStarted(int waveNumber)
    {
        RefreshWaveText();
    }

    private void HandleWaveCleared(int waveNumber)
    {
        RefreshWaveText();
    }

    private void RefreshWaveText()
    {
        if (currentWaveText == null || waveManager == null)
            return;

        int currentWave = Mathf.Max(0, waveManager.CurrentWaveNumber);
        int totalWave = Mathf.Max(0, waveManager.TotalWaveCount);

        currentWaveText.text = $"{currentWave}/{totalWave}";
    }
}