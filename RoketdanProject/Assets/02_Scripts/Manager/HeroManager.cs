using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [SerializeField] private BoardGridByBounds boardGrid;
    [SerializeField] private BoardRuntimeManager boardRuntime;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private EnemyManager enemyManager;

    [Header("Default Heroes")]
    [SerializeField] private HeroData defaultMeleeHero;
    [SerializeField] private HeroData defaultRangedHero;

    [Header("Spawn Parent")]
    [SerializeField] private Transform heroRoot;

    [Header("Visual")]
    [SerializeField] private string heroSortingLayerName = "Hero";
    [SerializeField] private int heroSortingOrder = 100;
    [SerializeField] private float heroCellFitRatio = 0.65f;

    private readonly List<HeroInstance> activeHeroes = new List<HeroInstance>();

    public IReadOnlyList<HeroInstance> ActiveHeroes => activeHeroes;

    public HeroData GetSummonDataForCell(CellRuntime cell)
    {
        if (cell == null)
            return null;

        return cell.isAdjacentToPath ? defaultMeleeHero : defaultRangedHero;
    }

    public bool TrySummonDefault(Vector2Int cellPos, out HeroInstance summonedHero)
    {
        summonedHero = null;

        if (boardRuntime == null)
        {
            Debug.LogError("[HeroManager] BoardRuntimeManager가 연결되지 않았습니다.");
            return false;
        }

        CellRuntime cell = boardRuntime.GetCell(cellPos.x, cellPos.y);
        if (cell == null)
            return false;

        if (!cell.IsEmptyBuildable)
            return false;

        HeroData heroData = GetSummonDataForCell(cell);
        if (heroData == null)
        {
            Debug.LogWarning("[HeroManager] 소환할 HeroData가 없습니다.");
            return false;
        }

        if (heroData.prefab == null)
        {
            Debug.LogWarning($"[HeroManager] {heroData.heroId} prefab이 비어 있습니다.");
            return false;
        }

        if (currencyManager == null)
        {
            Debug.LogError("[HeroManager] CurrencyManager가 연결되지 않았습니다.");
            return false;
        }

        if (!currencyManager.Spend(heroData.summonCost))
        {
            Debug.Log("[HeroManager] 골드가 부족해서 소환 실패");
            return false;
        }

        summonedHero = SpawnHero(heroData, cellPos);
        return summonedHero != null;
    }

    public bool CanPromote(HeroInstance selected)
    {
        if (selected == null || selected.Data == null)
            return false;

        if (selected.Data.nextGradeHero == null)
            return false;

        return FindPromotionMaterial(selected) != null;
    }

    public bool TryPromote(HeroInstance selected, out HeroInstance promotedHero)
    {
        promotedHero = null;

        if (!CanPromote(selected))
            return false;

        HeroInstance material = FindPromotionMaterial(selected);
        if (material == null)
            return false;

        HeroData nextData = selected.Data.nextGradeHero;
        if (nextData == null)
            return false;

        if (nextData.prefab == null)
        {
            Debug.LogWarning($"[HeroManager] {nextData.heroId} prefab이 비어 있습니다.");
            return false;
        }

        Vector2Int selectedCell = selected.CellPos;

        RemoveHero(material);
        RemoveHero(selected);

        promotedHero = SpawnHero(nextData, selectedCell);
        return promotedHero != null;
    }

    private HeroInstance FindPromotionMaterial(HeroInstance selected)
    {
        for (int i = 0; i < activeHeroes.Count; i++)
        {
            HeroInstance other = activeHeroes[i];

            if (other == null || other == selected)
                continue;

            if (other.Data == null || selected.Data == null)
                continue;

            if (other.Data.heroId == selected.Data.heroId &&
                other.Data.grade == selected.Data.grade)
            {
                return other;
            }
        }

        return null;
    }

    private HeroInstance SpawnHero(HeroData data, Vector2Int cellPos)
    {
        if (data == null || data.prefab == null)
            return null;

        if (boardGrid == null)
        {
            Debug.LogError("[HeroManager] BoardGridByBounds가 연결되지 않았습니다.");
            return null;
        }

        if (boardRuntime == null)
        {
            Debug.LogError("[HeroManager] BoardRuntimeManager가 연결되지 않았습니다.");
            return null;
        }

        Vector3 spawnPos = boardGrid.GetCellCenter(cellPos.x, cellPos.y);
        spawnPos.z = 0f;

        GameObject heroObj = Instantiate(data.prefab, spawnPos, Quaternion.identity, heroRoot);

        HeroInstance instance = heroObj.GetComponent<HeroInstance>();
        if (instance == null)
            instance = heroObj.AddComponent<HeroInstance>();

        instance.Initialize(data, cellPos, enemyManager);

        SetupHeroVisual(heroObj);

        activeHeroes.Add(instance);

        CellRuntime cell = boardRuntime.GetCell(cellPos.x, cellPos.y);
        if (cell != null)
        {
            cell.placedHero = instance;
        }

        Debug.Log($"[HeroManager] Hero 생성 완료: {data.heroId} / 위치 {spawnPos}");
        return instance;
    }

    private void SetupHeroVisual(GameObject heroObj)
    {
        if (heroObj == null)
            return;

        SpriteRenderer sr = heroObj.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("[HeroManager] Hero 프리팹에 SpriteRenderer가 없습니다.");
            return;
        }

        sr.sortingLayerName = heroSortingLayerName;
        sr.sortingOrder = heroSortingOrder;

        Vector2 cellSize = boardGrid.GetCellSize();
        Vector2 spriteSize = sr.sprite != null ? sr.sprite.bounds.size : Vector2.one;

        if (spriteSize.x > 0f && spriteSize.y > 0f)
        {
            float scaleX = (cellSize.x / spriteSize.x) * heroCellFitRatio;
            float scaleY = (cellSize.y / spriteSize.y) * heroCellFitRatio;
            heroObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        Vector3 pos = heroObj.transform.position;
        pos.z = 0f;
        heroObj.transform.position = pos;
    }

    public void RemoveHero(HeroInstance hero)
    {
        if (hero == null)
            return;

        activeHeroes.Remove(hero);

        if (boardRuntime != null)
        {
            CellRuntime cell = boardRuntime.GetCell(hero.CellPos.x, hero.CellPos.y);
            if (cell != null)
            {
                cell.placedHero = null;
            }
        }

        Destroy(hero.gameObject);
    }
}