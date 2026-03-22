using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGridByBounds : MonoBehaviour
{
    [Header("Board Corners")]
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform bottomRight;

    [Header("Grid Size")]
    [SerializeField] private int columns = 6;
    [SerializeField] private int rows = 8;

    public int Columns => columns;
    public int Rows => rows;

    public Vector3 GetCellCenter(int x, int y)
    {
        Vector3 boardRight = topRight.position - topLeft.position;
        Vector3 boardUp = topLeft.position - bottomLeft.position;

        float cellWidth01 = 1f / columns;
        float cellHeight01 = 1f / rows;

        Vector3 origin = bottomLeft.position;

        Vector3 rightOffset = boardRight * ((x + 0.5f) * cellWidth01);
        Vector3 upOffset = boardUp * ((y + 0.5f) * cellHeight01);

        return origin + rightOffset + upOffset;
    }

    public bool TryWorldToCell(Vector3 worldPos, out Vector2Int cell)
    {
        cell = new Vector2Int(-1, -1);

        Vector3 boardRight = bottomRight.position - bottomLeft.position;
        Vector3 boardUp = topLeft.position - bottomLeft.position;

        float boardWidth = boardRight.magnitude;
        float boardHeight = boardUp.magnitude;

        if (boardWidth <= 0.0001f || boardHeight <= 0.0001f)
            return false;

        Vector3 rightDir = boardRight.normalized;
        Vector3 upDir = boardUp.normalized;

        Vector3 local = worldPos - bottomLeft.position;

        float xProj = Vector3.Dot(local, rightDir);
        float yProj = Vector3.Dot(local, upDir);

        if (xProj < 0f || xProj > boardWidth || yProj < 0f || yProj > boardHeight)
            return false;

        float normalizedX = xProj / boardWidth;
        float normalizedY = yProj / boardHeight;

        int x = Mathf.FloorToInt(normalizedX * columns);
        int y = Mathf.FloorToInt(normalizedY * rows);

        x = Mathf.Clamp(x, 0, columns - 1);
        y = Mathf.Clamp(y, 0, rows - 1);

        cell = new Vector2Int(x, y);
        return true;
    }

    public Vector2 GetCellSize()
    {
        float boardWidth = Vector3.Distance(bottomLeft.position, bottomRight.position);
        float boardHeight = Vector3.Distance(bottomLeft.position, topLeft.position);

        return new Vector2(boardWidth / columns, boardHeight / rows);
    }

    private void OnDrawGizmos()
    {
        if (topLeft == null || topRight == null || bottomLeft == null || bottomRight == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(topLeft.position, topRight.position);
        Gizmos.DrawLine(topRight.position, bottomRight.position);
        Gizmos.DrawLine(bottomRight.position, bottomLeft.position);
        Gizmos.DrawLine(bottomLeft.position, topLeft.position);

        Gizmos.color = Color.green;

        Vector3 boardRight = bottomRight.position - bottomLeft.position;
        Vector3 boardUp = topLeft.position - bottomLeft.position;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 center = GetCellCenter(x, y);

                Vector3 size = new Vector3(
                    boardRight.magnitude / columns,
                    boardUp.magnitude / rows,
                    0.01f
                );

                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
