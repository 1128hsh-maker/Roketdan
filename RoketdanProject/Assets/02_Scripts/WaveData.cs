using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveSpawnEntry
{
    public EnemyData enemyData;
    [Min(1)] public int count = 5;
    [Min(0.05f)] public float interval = 0.5f;
}

[CreateAssetMenu(menuName = "TD/Wave Data")]
public class WaveData : ScriptableObject
{
    public List<WaveSpawnEntry> spawnEntries = new List<WaveSpawnEntry>();
}
