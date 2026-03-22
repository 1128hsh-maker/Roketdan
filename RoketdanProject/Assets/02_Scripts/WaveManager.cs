using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private CommanderHealth commanderHealth;

    [Header("Wave List")]
    [SerializeField] private List<WaveData> waves = new List<WaveData>();

    [Header("Timing")]
    [SerializeField] private float gameStartDelay = 1f;
    [SerializeField] private float waveInterval = 2f;
    [SerializeField] private bool autoStart = true;

    public int CurrentWaveNumber { get; private set; }
    public bool IsGameEnded { get; private set; }

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCleared;
    public event Action OnVictory;
    public event Action OnDefeat;

    private void OnEnable()
    {
        if (commanderHealth != null)
        {
            commanderHealth.OnDefeated += HandleDefeat;
        }
    }

    private void OnDisable()
    {
        if (commanderHealth != null)
        {
            commanderHealth.OnDefeated -= HandleDefeat;
        }
    }

    private void Start()
    {
        if (autoStart)
        {
            StartBattle();
        }
    }

    [ContextMenu("Start Battle")]
    public void StartBattle()
    {
        if (IsGameEnded)
            return;

        StopAllCoroutines();
        StartCoroutine(RunBattleLoop());
    }

    private IEnumerator RunBattleLoop()
    {
        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("[WaveManager] WaveData가 하나도 없습니다.");
            yield break;
        }

        yield return new WaitForSeconds(gameStartDelay);

        for (int i = 0; i < waves.Count; i++)
        {
            if (IsGameEnded)
                yield break;

            CurrentWaveNumber = i + 1;
            Debug.Log($"[WaveManager] Wave {CurrentWaveNumber} 시작");
            OnWaveStarted?.Invoke(CurrentWaveNumber);

            yield return StartCoroutine(SpawnWave(waves[i]));

            yield return new WaitUntil(() => IsGameEnded || enemyManager.AliveCount == 0);

            if (IsGameEnded)
                yield break;

            Debug.Log($"[WaveManager] Wave {CurrentWaveNumber} 종료");
            OnWaveCleared?.Invoke(CurrentWaveNumber);

            if (i < waves.Count - 1)
            {
                yield return new WaitForSeconds(waveInterval);
            }
        }

        if (IsGameEnded)
            yield break;

        IsGameEnded = true;
        Debug.Log("[WaveManager] 모든 웨이브 종료, 승리");
        OnVictory?.Invoke();
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        if (wave == null)
        {
            Debug.LogWarning($"[WaveManager] Wave {CurrentWaveNumber} 데이터가 비어 있습니다.");
            yield break;
        }

        for (int i = 0; i < wave.spawnEntries.Count; i++)
        {
            WaveSpawnEntry entry = wave.spawnEntries[i];

            if (entry == null || entry.enemyData == null)
                continue;

            for (int count = 0; count < entry.count; count++)
            {
                if (IsGameEnded)
                    yield break;

                enemySpawner.Spawn(entry.enemyData);

                float interval = Mathf.Max(0.01f, entry.interval);
                yield return new WaitForSeconds(interval);
            }
        }
    }

    private void HandleDefeat()
    {
        if (IsGameEnded)
            return;

        IsGameEnded = true;
        StopAllCoroutines();

        Debug.Log("[WaveManager] 지휘관 체력 0, 패배");
        OnDefeat?.Invoke();
    }
}
