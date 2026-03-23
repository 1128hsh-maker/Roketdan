using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyId = "BasicEnemy";
    public int maxHp = 10;
    public float moveSpeed = 1.5f;

    [Header("Attack")]
    public int contactDamage = 1;
    public float attackInterval = 1f;

    [Header("Reward")]
    public int killReward = 10;

    [Header("Prefab")]
    public GameObject prefab;
}
