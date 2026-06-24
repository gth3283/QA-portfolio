using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyManager EnemyManager;

    public List<EnemyData> normal;
    public List<EnemyData> boss;

    public void SpawnNormal()
    {
        int index = Random.Range(0, normal.Count);
        EnemyManager.DataInput(normal[index]);
        QAEventLogger.LogEvent("적 조우 이벤트", $"{index}번 적 조우");
    }

    public void SpawnBoss()
    {
        int index = Random.Range(0, boss.Count);
        EnemyManager.DataInput(boss[index]);
        QAEventLogger.LogEvent($"{index}번 적 조우", "보스");
    }
}
