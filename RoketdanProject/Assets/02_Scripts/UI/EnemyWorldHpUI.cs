using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWorldHpUI : MonoBehaviour
{
    [SerializeField] private EnemyInstance enemyInstance;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {
        if (hpText == null)
            hpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (enemyInstance == null)
            enemyInstance = GetComponentInParent<EnemyInstance>();

        if (enemyInstance != null)
        {
            enemyInstance.OnHpChanged += HandleHpChanged;

            if (enemyInstance.Data != null)
            {
                HandleHpChanged(enemyInstance.CurrentHp, enemyInstance.Data.maxHp);
            }
        }
        else
        {
            Debug.LogWarning("[EnemyWorldHpUI] EnemyInstance를 찾지 못했습니다.");
        }
    }

    private void OnDestroy()
    {
        if (enemyInstance != null)
            enemyInstance.OnHpChanged -= HandleHpChanged;
    }

    private void HandleHpChanged(int currentHp, int maxHp)
    {
        if (hpText != null)
            hpText.text = $"{currentHp}/{maxHp}";
    }
}
