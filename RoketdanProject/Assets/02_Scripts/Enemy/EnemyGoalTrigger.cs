using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyGoalTrigger : MonoBehaviour
{
    private EnemyInstance enemyInstance;

    private void Start()
    {
        enemyInstance = GetComponentInParent<EnemyInstance>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyInstance == null)
            return;

        if (enemyInstance.IsDead)
            return;

        if (other.CompareTag("Commander"))
        {
            Debug.Log("[EnemyGoalTrigger] Commander 접촉 -> Enemy 근접 공격 시작");
            enemyInstance.BeginCommanderAttack();
        }
    }
}