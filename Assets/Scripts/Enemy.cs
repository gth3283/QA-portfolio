using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Normal, Boss
}
public enum EnemyAction
{
    Attack,
    Buff,
    Heal
}

[System.Serializable]
public class EnemyActionData
{
    public EnemyAction action;
    public int value;
}

[System.Serializable]
public class EnemyData
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;

    public int maxHp;
    public int baseAttackPower;

    public List<EnemyActionData> actionPattern;
}