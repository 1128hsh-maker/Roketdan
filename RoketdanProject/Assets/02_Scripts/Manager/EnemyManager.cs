using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private readonly List<EnemyInstance> activeEnemies = new List<EnemyInstance>();

    public int AliveCount => activeEnemies.Count;
    public IReadOnlyList<EnemyInstance> ActiveEnemies => activeEnemies;

    public event Action<int> OnAliveCountChanged;

    public void Register(EnemyInstance enemy)
    {
        if (enemy == null || activeEnemies.Contains(enemy))
            return;

        activeEnemies.Add(enemy);
        NotifyChanged();
    }

    public void Unregister(EnemyInstance enemy)
    {
        if (enemy == null)
            return;

        if (activeEnemies.Remove(enemy))
        {
            NotifyChanged();
        }
    }

    public EnemyInstance FindNearestEnemyInRange(Vector3 origin, float range)
    {
        float bestSqrDistance = range * range;
        EnemyInstance bestEnemy = null;

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            EnemyInstance enemy = activeEnemies[i];

            if (enemy == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;

            if (sqrDistance <= bestSqrDistance)
            {
                bestSqrDistance = sqrDistance;
                bestEnemy = enemy;
            }
        }

        return bestEnemy;
    }

    private void NotifyChanged()
    {
        OnAliveCountChanged?.Invoke(activeEnemies.Count);
        Debug.Log($"[EnemyManager] 현재 살아있는 적 수: {activeEnemies.Count}");
    }
}
