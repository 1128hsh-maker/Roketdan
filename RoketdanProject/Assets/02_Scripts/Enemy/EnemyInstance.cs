using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private List<Vector3> pathPoints;
    private int currentPathIndex;

    private EnemyManager enemyManager;
    private CommanderHealth commanderHealth;
    private CurrencyManager currencyManager;

    private Vector3 goalPoint;
    private bool useGoalPoint;

    public void Initialize(
        EnemyData data,
        List<Vector3> path,
        EnemyManager manager,
        CommanderHealth commander,
        CurrencyManager currency,
        Vector3? customGoalPoint = null)
    {
        Data = data;
        pathPoints = new List<Vector3>(path);
        enemyManager = manager;
        commanderHealth = commander;
        currencyManager = currency;

        CurrentHp = data.maxHp;
        IsDead = false;
        currentPathIndex = 1;

        useGoalPoint = customGoalPoint.HasValue;
        if (useGoalPoint)
        {
            goalPoint = customGoalPoint.Value;
        }

        gameObject.name = $"{data.enemyId}_Enemy";

        if (pathPoints.Count > 0)
        {
            transform.position = pathPoints[0];
        }

        enemyManager?.Register(this);
    }

    private void Update()
    {
        if (IsDead)
            return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (Data == null || pathPoints == null || pathPoints.Count == 0)
            return;

        if (currentPathIndex >= pathPoints.Count)
        {
            ReachGoal();
            return;
        }

        Vector3 target = pathPoints[currentPathIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            Data.moveSpeed * Time.deltaTime
        );

        if (useGoalPoint && Vector3.Distance(transform.position, goalPoint) <= 0.05f)
        {
            ReachGoal();
            return;
        }

        if (Vector3.Distance(transform.position, target) <= 0.01f)
        {
            currentPathIndex++;

            if (currentPathIndex >= pathPoints.Count)
            {
                ReachGoal();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        Debug.Log($"[EnemyInstance] {Data.enemyId} 피해 {amount}, 현재 HP: {CurrentHp}");

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void ReachGoal()
    {
        if (IsDead)
            return;

        IsDead = true;

        Debug.Log($"[EnemyInstance] {Data.enemyId} 골인, 지휘관에게 {Data.contactDamage} 피해");
        commanderHealth?.TakeDamage(Data.contactDamage);
        enemyManager?.Unregister(this);

        Destroy(gameObject);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        Debug.Log($"[EnemyInstance] {Data.enemyId} 사망");

        if (currencyManager != null && Data.killReward > 0)
        {
            currencyManager.Add(Data.killReward);
        }

        enemyManager?.Unregister(this);
        Destroy(gameObject);
    }
    public void ForceReachGoal()
    {
        ReachGoal();
    }
}