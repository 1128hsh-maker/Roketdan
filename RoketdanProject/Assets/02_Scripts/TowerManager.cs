using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private BoardGridByBounds boardGrid;
    [SerializeField] private BoardRuntimeManager boardRuntime;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Default Build Towers")]
    [SerializeField] private TowerData defaultMeleeTower;
    [SerializeField] private TowerData defaultRangedTower;

    [Header("Spawn Parent")]
    [SerializeField] private Transform towerRoot;

    private readonly List<TowerInstance> activeTowers = new List<TowerInstance>();

    public IReadOnlyList<TowerInstance> ActiveTowers => activeTowers;

    public TowerData GetBuildDataForCell(CellRuntime cell)
    {
        if (cell == null)
            return null;

        return cell.isAdjacentToPath ? defaultMeleeTower : defaultRangedTower;
    }

    public bool TryBuildDefault(Vector2Int cellPos, out TowerInstance builtTower)
    {
        builtTower = null;

        CellRuntime cell = boardRuntime.GetCell(cellPos.x, cellPos.y);
        if (cell == null)
            return false;

        if (!cell.IsEmptyBuildable)
            return false;

        TowerData buildData = GetBuildDataForCell(cell);
        if (buildData == null || buildData.prefab == null)
        {
            Debug.LogWarning("[TowerManager] 설치할 기본 TowerData 또는 prefab이 없습니다.");
            return false;
        }

        if (!currencyManager.Spend(buildData.buildCost))
        {
            Debug.Log("[TowerManager] 골드가 부족합니다.");
            return false;
        }

        builtTower = SpawnTower(buildData, cellPos);
        return builtTower != null;
    }

    public bool CanMerge(TowerInstance selected)
    {
        if (selected == null)
            return false;

        if (selected.Data == null)
            return false;

        if (selected.Data.nextGradeTower == null)
            return false;

        return FindMergeTarget(selected) != null;
    }

    public bool CanTranscend(TowerInstance selected)
    {
        if (selected == null)
            return false;

        if (selected.Data == null)
            return false;

        return selected.Data.transcendTower != null;
    }

    public bool TryMerge(TowerInstance selected, out TowerInstance resultTower)
    {
        resultTower = null;

        if (!CanMerge(selected))
            return false;

        TowerInstance target = FindMergeTarget(selected);
        if (target == null)
            return false;

        TowerData nextData = selected.Data.nextGradeTower;
        if (nextData == null || nextData.prefab == null)
            return false;

        Vector2Int selectedCellPos = selected.CellPos;

        RemoveTower(target);
        RemoveTower(selected);

        resultTower = SpawnTower(nextData, selectedCellPos);
        return resultTower != null;
    }

    public bool TryTranscend(TowerInstance selected, out TowerInstance resultTower)
    {
        resultTower = null;

        if (!CanTranscend(selected))
            return false;

        TowerData transcendData = selected.Data.transcendTower;
        if (transcendData == null || transcendData.prefab == null)
            return false;

        Vector2Int selectedCellPos = selected.CellPos;

        RemoveTower(selected);

        resultTower = SpawnTower(transcendData, selectedCellPos);
        return resultTower != null;
    }

    private TowerInstance FindMergeTarget(TowerInstance selected)
    {
        for (int i = 0; i < activeTowers.Count; i++)
        {
            TowerInstance other = activeTowers[i];

            if (other == null || other == selected)
                continue;

            if (other.Data == null || selected.Data == null)
                continue;

            if (other.Data.towerId == selected.Data.towerId &&
                other.Data.grade == selected.Data.grade)
            {
                return other;
            }
        }

        return null;
    }

    private TowerInstance SpawnTower(TowerData data, Vector2Int cellPos)
    {
        if (data == null || data.prefab == null)
            return null;

        Vector3 spawnPos = boardGrid.GetCellCenter(cellPos.x, cellPos.y);
        GameObject towerObj = Instantiate(data.prefab, spawnPos, Quaternion.identity, towerRoot);

        TowerInstance instance = towerObj.GetComponent<TowerInstance>();
        if (instance == null)
            instance = towerObj.AddComponent<TowerInstance>();

        instance.Initialize(data, cellPos);

        activeTowers.Add(instance);
        boardRuntime.SetPlacedTower(cellPos.x, cellPos.y, instance);

        return instance;
    }

    private void RemoveTower(TowerInstance tower)
    {
        if (tower == null)
            return;

        activeTowers.Remove(tower);
        boardRuntime.ClearPlacedTower(tower.CellPos.x, tower.CellPos.y);

        Destroy(tower.gameObject);
    }
}
