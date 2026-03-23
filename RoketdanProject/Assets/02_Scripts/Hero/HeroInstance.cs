using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInstance : MonoBehaviour
{
    public HeroData Data { get; private set; }
    public Vector2Int CellPos { get; private set; }

    private EnemyManager enemyManager;
    private float attackTimer;

    public void Initialize(HeroData data, Vector2Int cellPos, EnemyManager manager)
    {
        Data = data;
        CellPos = cellPos;
        enemyManager = manager;
        attackTimer = 0f;

        if (data != null)
        {
            gameObject.name = $"{data.heroId}_G{data.grade}_{cellPos.x}_{cellPos.y}";
        }
    }

    private void Update()
    {
        if (Data == null || enemyManager == null)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer < Data.attackInterval)
            return;

        EnemyInstance target = enemyManager.FindNearestEnemyInRange(transform.position, Data.attackRange);
        if (target == null)
            return;

        attackTimer = 0f;
        target.TakeDamage(Data.attackDamage);

        Debug.Log($"[HeroInstance] {Data.heroId} 공격 → {target.name} / 데미지 {Data.attackDamage}");
    }
}
