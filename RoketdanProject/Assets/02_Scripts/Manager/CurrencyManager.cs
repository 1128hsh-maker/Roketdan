using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private int startGold = 500;

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        Gold = startGold;
        NotifyChanged();
    }

    public bool HasGold(int amount)
    {
        return Gold >= amount;
    }

    public bool Spend(int amount)
    {
        if (amount < 0)
            return false;

        if (Gold < amount)
            return false;

        Gold -= amount;
        NotifyChanged();
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        Gold += amount;
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnGoldChanged?.Invoke(Gold);
        Debug.Log($"[Currency] Gold = {Gold}");
    }
}
