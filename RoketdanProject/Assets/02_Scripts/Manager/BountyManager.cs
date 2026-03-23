using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyManager : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Bounty")]
    [SerializeField] private EnemyData bountyEnemyData;
    [SerializeField] private int bountySpawnCost = 100;
    [SerializeField] private float cooldown = 15f;

    private float nextAvailableTime;

    public bool TrySpawnBounty()
    {
        if (currencyManager == null || enemySpawner == null || bountyEnemyData == null)
            return false;

        if (Time.time < nextAvailableTime)
        {
            Debug.Log("[BountyManager] 현상금 쿨타임 중");
            return false;
        }

        if (!currencyManager.Spend(bountySpawnCost))
        {
            Debug.Log("[BountyManager] 골드 부족으로 현상금 생성 실패");
            return false;
        }

        enemySpawner.Spawn(bountyEnemyData);
        nextAvailableTime = Time.time + cooldown;

        Debug.Log($"[BountyManager] 현상금 적 생성 / 비용 {bountySpawnCost}");
        return true;
    }
}
