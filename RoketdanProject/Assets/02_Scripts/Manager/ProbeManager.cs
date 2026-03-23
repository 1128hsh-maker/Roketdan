using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeManager : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private MineralManager mineralManager;

    [Header("Probe")]
    [SerializeField] private int summonCostGold = 150;
    [SerializeField] private int maxProbeCount = 3;
    [SerializeField] private int mineralPerTickPerProbe = 20;
    [SerializeField] private float incomeInterval = 5f;

    public int CurrentProbeCount { get; private set; }

    private Coroutine incomeRoutine;

    public bool TrySummonProbe()
    {
        if (currencyManager == null || mineralManager == null)
            return false;

        if (CurrentProbeCount >= maxProbeCount)
        {
            Debug.Log("[ProbeManager] 탐사정 최대 수 도달");
            return false;
        }

        if (!currencyManager.Spend(summonCostGold))
        {
            Debug.Log("[ProbeManager] 골드 부족으로 탐사정 소환 실패");
            return false;
        }

        CurrentProbeCount++;

        if (incomeRoutine == null)
            incomeRoutine = StartCoroutine(IncomeLoop());

        Debug.Log($"[ProbeManager] 탐사정 소환 성공 / 현재 수: {CurrentProbeCount}");
        return true;
    }

    private IEnumerator IncomeLoop()
    {
        while (CurrentProbeCount > 0)
        {
            yield return new WaitForSeconds(incomeInterval);

            int income = CurrentProbeCount * mineralPerTickPerProbe;
            mineralManager.Add(income);

            Debug.Log($"[ProbeManager] 탐사정 채굴 / +{income} Mineral");
        }

        incomeRoutine = null;
    }
}