using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyGoalTrigger : MonoBehaviour
{
    private EnemyInstance enemyInstance;

    private void Start()
    {
        enemyInstance = GetComponent<EnemyInstance>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Commander"))
        {
            Debug.Log("[EnemyGoalTrigger] Commander와 접촉, 골인 처리");
            enemyInstance.ForceReachGoal();
        }
    }
}
