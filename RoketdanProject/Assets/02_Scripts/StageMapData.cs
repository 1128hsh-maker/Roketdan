using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Stage Map Data")]
public class StageMapData : ScriptableObject
{
    [Min(1)] public int columns = 6;
    [Min(1)] public int rows = 8;

    [Header("Layout Text")]
    [TextArea(8, 20)]
    public string layoutText =
@"XXXXXX
XBBBBX
XBLLBX
XBPPBX
XBPPBX
XBBBBX
XBBBBX
XXXXXX";

    [Header("Locked Cell Default Cost")]
    public int defaultUnlockCost = 100;

    [Header("Enemy Path (bottom-left origin)")]
    public List<Vector2Int> enemyPath = new List<Vector2Int>();

    [SerializeField, HideInInspector] private CellType[] parsedCells;

    public CellType GetCellType(int x, int y)
    {
        EnsureLayoutBuilt();

        if (x < 0 || x >= columns || y < 0 || y >= rows)
            return CellType.Blocked;

        return parsedCells[y * columns + x];
    }

    public void EnsureLayoutBuilt()
    {
        if (parsedCells == null || parsedCells.Length != columns * rows)
        {
            RebuildLayout();
        }
    }

    [ContextMenu("Rebuild Layout")]
    public void RebuildLayout()
    {
        parsedCells = new CellType[columns * rows];

        for (int i = 0; i < parsedCells.Length; i++)
            parsedCells[i] = CellType.Blocked;

        string[] rawLines = layoutText.Replace("\r", "").Split('\n');
        List<string> lines = new List<string>();

        for (int i = 0; i < rawLines.Length; i++)
        {
            string cleaned = RemoveWhitespace(rawLines[i]);
            if (!string.IsNullOrEmpty(cleaned))
                lines.Add(cleaned);
        }

        int usableRowCount = Mathf.Min(rows, lines.Count);

        for (int topRowIndex = 0; topRowIndex < usableRowCount; topRowIndex++)
        {
            string line = lines[topRowIndex];
            int y = rows - 1 - topRowIndex;

            int usableColCount = Mathf.Min(columns, line.Length);

            for (int x = 0; x < usableColCount; x++)
            {
                parsedCells[y * columns + x] = CharToCellType(line[x]);
            }
        }
    }

    private void OnValidate()
    {
        RebuildLayout();
    }

    private static string RemoveWhitespace(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        System.Text.StringBuilder sb = new System.Text.StringBuilder(input.Length);

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!char.IsWhiteSpace(c))
                sb.Append(c);
        }

        return sb.ToString();
    }

    private static CellType CharToCellType(char c)
    {
        switch (char.ToUpperInvariant(c))
        {
            case 'B': return CellType.Buildable;
            case 'L': return CellType.Locked;
            case 'P': return CellType.Path;
            case 'X':
            default:
                return CellType.Blocked;
        }
    }
}
