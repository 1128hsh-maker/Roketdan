using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private CommanderHealth commanderHealth;

    [Header("Wave List")]
    [SerializeField] private List<WaveData> waves = new List<WaveData>();

    [Header("Timing")]
    [SerializeField] private float gameStartDelay = 1f;
    [SerializeField] private float waveInterval = 2f;
    [SerializeField] private bool autoStart = true;

    public int CurrentWaveIndex { get; private set; } = -1;
    public int CurrentWaveNumber => CurrentWaveIndex + 1;
    public int TotalWaveCount => waves != null ? waves.Count : 0;
    public bool IsBattleRunning { get; private set; }
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
        if (IsBattleRunning || IsGameEnded)
            return;

        if (enemySpawner == null)
        {
            Debug.LogError("[WaveManager] EnemySpawner가 연결되지 않았습니다.");
            return;
        }

        if (enemyManager == null)
        {
            Debug.LogError("[WaveManager] EnemyManager가 연결되지 않았습니다.");
            return;
        }

        if (commanderHealth == null)
        {
            Debug.LogError("[WaveManager] CommanderHealth가 연결되지 않았습니다.");
            return;
        }

        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("[WaveManager] WaveData가 하나도 없습니다.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        IsBattleRunning = true;
        IsGameEnded = false;
        CurrentWaveIndex = -1;

        yield return new WaitForSeconds(gameStartDelay);

        for (int i = 0; i < waves.Count; i++)
        {
            if (IsGameEnded)
                yield break;

            CurrentWaveIndex = i;

            Debug.Log($"[WaveManager] Wave {CurrentWaveNumber} 시작");
            OnWaveStarted?.Invoke(CurrentWaveNumber);

            yield return StartCoroutine(SpawnWave(waves[i]));

            // 이번 웨이브 적 생성이 끝난 뒤,
            // 맵 위 적이 전부 사라질 때까지 기다림
            yield return new WaitUntil(() => IsGameEnded || enemyManager.AliveCount == 0);

            if (IsGameEnded)
                yield break;

            Debug.Log($"[WaveManager] Wave {CurrentWaveNumber} 종료");
            OnWaveCleared?.Invoke(CurrentWaveNumber);

            bool isLastWave = (i == waves.Count - 1);

            // 마지막 웨이브면 여기서 승리 체크
            if (isLastWave)
            {
                if (!commanderHealth.IsDead && enemyManager.AliveCount == 0)
                {
                    HandleVictory();
                    yield break;
                }
            }
            else
            {
                yield return new WaitForSeconds(waveInterval);
            }
        }
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

            int spawnCount = Mathf.Max(1, entry.count);
            float spawnInterval = Mathf.Max(0.01f, entry.interval);

            for (int j = 0; j < spawnCount; j++)
            {
                if (IsGameEnded)
                    yield break;

                enemySpawner.Spawn(entry.enemyData);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void HandleVictory()
    {
        if (IsGameEnded)
            return;

        IsGameEnded = true;
        IsBattleRunning = false;

        StopAllCoroutines();

        Debug.Log("[WaveManager] 모든 웨이브 종료 + Commander 생존 -> 승리");
        OnVictory?.Invoke();
    }

    private void HandleDefeat()
    {
        if (IsGameEnded)
            return;

        IsGameEnded = true;
        IsBattleRunning = false;

        StopAllCoroutines();

        Debug.Log("[WaveManager] Commander 사망 -> 패배");
        OnDefeat?.Invoke();
    }
}