using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private BoardRuntimeManager boardRuntime;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private CommanderHealth commanderHealth;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private Transform enemyRoot;


    public EnemyInstance Spawn(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            Debug.LogError("[EnemySpawner] EnemyData가 비어 있습니다.");
            return null;
        }

        if (enemyData.prefab == null)
        {
            Debug.LogError($"[EnemySpawner] {enemyData.enemyId}의 prefab이 비어 있습니다.");
            return null;
        }

        if (boardRuntime == null)
        {
            Debug.LogError("[EnemySpawner] BoardRuntimeManager가 연결되지 않았습니다.");
            return null;
        }

        List<Vector3> path = boardRuntime.GetEnemyPathWorldPositions();

        if (path == null || path.Count == 0)
        {
            Debug.LogError("[EnemySpawner] enemyPath가 비어 있어서 적을 생성할 수 없습니다.");
            return null;
        }

        Vector3 goalPoint = path[path.Count - 1];

        GameObject enemyObj = Instantiate(enemyData.prefab, path[0], Quaternion.identity, enemyRoot);

        EnemyInstance instance = enemyObj.GetComponent<EnemyInstance>();
        if (instance == null)
        {
            instance = enemyObj.AddComponent<EnemyInstance>();
        }

        instance.Initialize(enemyData, path, enemyManager, commanderHealth, currencyManager, goalPoint);
        return instance;

    }
}
