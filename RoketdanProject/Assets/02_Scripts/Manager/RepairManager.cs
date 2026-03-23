using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairManager : MonoBehaviour
{
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private CommanderHealth commanderHealth;

    [Header("Repair")]
    [SerializeField] private int repairCost = 30;
    [SerializeField] private int repairAmount = 2;

    public bool TryRepair()
    {
        if (currencyManager == null || commanderHealth == null)
            return false;

        if (commanderHealth.IsDead)
            return false;

        if (commanderHealth.CurrentHealth >= commanderHealth.MaxHealth)
            return false;

        if (!currencyManager.Spend(repairCost))
        {
            Debug.Log("[RepairManager] 골드 부족으로 수리 실패");
            return false;
        }

        commanderHealth.Heal(repairAmount);
        Debug.Log($"[RepairManager] 수리 성공 / 비용 {repairCost} / 회복량 {repairAmount}");
        return true;
    }
}