using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDefeated;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        NotifyChanged();
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        NotifyChanged();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        Debug.Log($"[CommanderHealth] 지휘관 피해 {amount}, 현재 체력: {CurrentHealth}/{MaxHealth}");

        NotifyChanged();

        if (CurrentHealth <= 0)
        {
            Debug.Log("[CommanderHealth] 지휘관 체력이 0이 되어 패배했습니다.");
            OnDefeated?.Invoke();
        }
    }

    private void NotifyChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
