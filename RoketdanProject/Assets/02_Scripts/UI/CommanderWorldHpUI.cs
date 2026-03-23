using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommanderWorldHpUI : MonoBehaviour
{
    [SerializeField] private CommanderHealth commanderHealth;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {
        if (hpText == null)
            hpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (commanderHealth != null)
            commanderHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (commanderHealth != null)
            commanderHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        if (commanderHealth != null)
        {
            HandleHealthChanged(commanderHealth.CurrentHealth, commanderHealth.MaxHealth);
        }
    }

    private void HandleHealthChanged(int currentHp, int maxHp)
    {
        if (hpText != null)
            hpText.text = $"{currentHp}/{maxHp}";
    }
}
