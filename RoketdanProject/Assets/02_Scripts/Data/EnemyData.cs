using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyId = "BasicEnemy";
    public int maxHp = 10;
    public float moveSpeed = 1.5f;
    public int contactDamage = 1;
    public int killReward = 10;
    public GameObject prefab;
}
