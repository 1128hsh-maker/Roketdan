using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralManager : MonoBehaviour
{
    [SerializeField] private int startMineral = 0;

    public int Mineral { get; private set; }

    public event Action<int> OnMineralChanged;

    private void Awake()
    {
        Mineral = startMineral;
        NotifyChanged();
    }

    public bool HasMineral(int amount)
    {
        return Mineral >= amount;
    }

    public bool Spend(int amount)
    {
        if (amount < 0)
            return false;

        if (Mineral < amount)
            return false;

        Mineral -= amount;
        NotifyChanged();
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        Mineral += amount;
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        OnMineralChanged?.Invoke(Mineral);
        Debug.Log($"[Mineral] = {Mineral}");
    }
}
