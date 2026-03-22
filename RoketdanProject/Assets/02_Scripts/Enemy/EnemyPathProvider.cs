using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathProvider : MonoBehaviour
{
    [SerializeField] private BoardRuntimeManager boardRuntime;

    public IReadOnlyList<Vector3> GetPathWorldPositions()
    {
        return boardRuntime.GetEnemyPathWorldPositions();
    }
}
