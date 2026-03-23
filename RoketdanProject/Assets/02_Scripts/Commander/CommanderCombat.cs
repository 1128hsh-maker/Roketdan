using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderCombat : MonoBehaviour
{
    [SerializeField] private CommanderHealth commanderHealth;
    [SerializeField] private EnemyManager enemyManager;

    [Header("Commander Combat")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackInterval = 0.7f;

    private float attackTimer;

    private void Update()
    {
        if (commanderHealth == null || commanderHealth.IsDead)
            return;

        if (enemyManager == null)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer < attackInterval)
            return;

        EnemyInstance target = enemyManager.FindNearestEnemyInRange(transform.position, attackRange);
        if (target == null)
            return;

        attackTimer = 0f;
        target.TakeDamage(attackDamage);

        Debug.Log($"[CommanderCombat] Commander가 {target.name} 공격 / 데미지 {attackDamage}");
    }
}