using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsAttackingCommander { get; private set; }

    public event Action<int, int> OnHpChanged;

    private List<Vector3> pathPoints;
    private int currentPathIndex;

    private EnemyManager enemyManager;
    private CommanderHealth commanderHealth;
    private CurrencyManager currencyManager;

    private float attackTimer;
    private bool reachedPathEnd;

    public void Initialize(
        EnemyData data,
        List<Vector3> path,
        EnemyManager manager,
        CommanderHealth commander,
        CurrencyManager currency)
    {
        Data = data;
        pathPoints = new List<Vector3>(path);
        enemyManager = manager;
        commanderHealth = commander;
        currencyManager = currency;

        CurrentHp = data.maxHp;
        IsDead = false;
        IsAttackingCommander = false;
        reachedPathEnd = false;
        attackTimer = 0f;
        currentPathIndex = 1;

        gameObject.name = $"{data.enemyId}_Enemy";

        if (pathPoints.Count > 0)
        {
            transform.position = pathPoints[0];
        }

        enemyManager?.Register(this);
        NotifyHpChanged();
    }

    private void Update()
    {
        if (IsDead)
            return;

        if (IsAttackingCommander)
        {
            AttackCommander();
            return;
        }

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (Data == null || pathPoints == null || pathPoints.Count == 0)
            return;

        if (reachedPathEnd)
            return;

        if (currentPathIndex >= pathPoints.Count)
        {
            reachedPathEnd = true;
            return;
        }

        Vector3 target = pathPoints[currentPathIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            Data.moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= 0.01f)
        {
            currentPathIndex++;

            if (currentPathIndex >= pathPoints.Count)
            {
                reachedPathEnd = true;
            }
        }
    }

    public void BeginCommanderAttack()
    {
        if (IsDead)
            return;

        if (commanderHealth == null || commanderHealth.IsDead)
            return;

        if (IsAttackingCommander)
            return;

        IsAttackingCommander = true;
        attackTimer = 0f;

        Debug.Log($"[EnemyInstance] {Data.enemyId} 가 Commander와 접촉, 근접 공격 시작");
    }

    private void AttackCommander()
    {
        if (commanderHealth == null || commanderHealth.IsDead)
            return;

        attackTimer += Time.deltaTime;

        float interval = Mathf.Max(0.05f, Data.attackInterval);

        if (attackTimer < interval)
            return;

        attackTimer = 0f;

        Debug.Log($"[EnemyInstance] {Data.enemyId} 가 Commander 공격 / 데미지 {Data.contactDamage}");
        commanderHealth.TakeDamage(Data.contactDamage);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        Debug.Log($"[EnemyInstance] {Data.enemyId} 피해 {amount}, 현재 HP: {CurrentHp}");

        NotifyHpChanged();

        if (CurrentHp <= 0)
        {
            Die();
        }
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

    private void NotifyHpChanged()
    {
        if (Data != null)
        {
            OnHpChanged?.Invoke(CurrentHp, Data.maxHp);
        }
    }
}