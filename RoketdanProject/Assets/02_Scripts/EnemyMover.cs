using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private EnemyPathProvider pathProvider;
    [SerializeField] private float moveSpeed = 2f;

    private List<Vector3> path = new List<Vector3>();
    private int currentIndex = 0;

    private void Start()
    {
        IReadOnlyList<Vector3> source = pathProvider.GetPathWorldPositions();

        for (int i = 0; i < source.Count; i++)
            path.Add(source[i]);

        if (path.Count > 0)
        {
            transform.position = path[0];
            currentIndex = 1;
        }
    }

    private void Update()
    {
        if (path.Count == 0)
            return;

        if (currentIndex >= path.Count)
            return;

        Vector3 target = path[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            currentIndex++;
        }
    }
}
