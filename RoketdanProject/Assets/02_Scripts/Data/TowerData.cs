using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerId;
    public TowerInstallType installType;
    [Range(1, 4)] public int grade = 1;

    public int buildCost = 100;
    public GameObject prefab;

    [Header("Upgrade")]
    public TowerData nextGradeTower;

    [Header("Transcend")]
    public TowerData transcendTower;
}
