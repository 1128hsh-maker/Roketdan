using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Hero Data")]
public class HeroData : ScriptableObject
{
    public string heroId = "BasicHero";
    [Range(1, 4)] public int grade = 1;

    [Header("Summon")]
    public int summonCost = 100;
    public GameObject prefab;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackInterval = 1.0f;

    [Header("Promotion")]
    public int promoteCost = 100;
    public HeroData nextGradeHero;

    [Header("Transcend")]
    public int transcendCostMineral = 100;
    public HeroData transcendOptionA;
    public HeroData transcendOptionB;
    public HeroData transcendOptionC;
}