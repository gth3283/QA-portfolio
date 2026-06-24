using System.Collections;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform SpawnPoint;
    private EnemyData enemyData;
    private EnemyAnimation enemyAnimation;
    public GameObject BuffParticle;
    public GameObject HealParticle;

    private GameObject enemy;

    public int MaxHp;
    public int Hp;
    public int AttackPower;
    public bool IsAttack = false;
    public TMP_Text actDetail;

    private int actionIndex;

    public void DataInput(EnemyData data)
    {
        enemyData = data;

        MaxHp = data.maxHp;
        Hp = MaxHp;
        AttackPower = data.baseAttackPower;

        actionIndex = 0;

        SpawnEnemy(data.enemyPrefab);
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemy != null)
        {
            Destroy(enemy);
        }

        enemy = Instantiate(enemyPrefab, SpawnPoint.position, Quaternion.identity, SpawnPoint);
        enemyAnimation = enemy.GetComponent<EnemyAnimation>();
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;

        if (Hp < 0)
        {
            Hp = 0;
        }

        if (Hp == 0)
        {
            StartCoroutine(enemyAnimation.dead());
            QAEventLogger.LogEvent("Enemy_Dead","Àû Ã³Ä¡");
        }
        else
        {
            StartCoroutine(enemyAnimation.def());
        }
    }

    public void ShowDetailAct()
    {
        switch (enemyData.actionPattern[actionIndex].action)
        {
            case EnemyAction.Attack:
                actDetail.text = $"Attack : {AttackPower}";
                break;
            case EnemyAction.Buff:
                actDetail.text = $"Buff : {enemyData.actionPattern[actionIndex].value}";
                break;
            case EnemyAction.Heal:
                actDetail.text = $"Heal : {enemyData.actionPattern[actionIndex].value}";
                break;
        }
    }

    private void nextAction()
    {
        if(actionIndex >= (enemyData.actionPattern.Count)-1) 
        {
            actionIndex = 0;
        }
        else
        {
            actionIndex++;
        }
    }

    public void DoEnemyAction()
    {
        IsAttack = false;
        switch (enemyData.actionPattern[actionIndex].action)
        {
            case EnemyAction.Attack:
                Attack();
                break;
            case EnemyAction.Buff:
                AttackPower += enemyData.actionPattern[actionIndex].value;
                StartCoroutine(buff());
                break;
            case EnemyAction.Heal:
                Hp += enemyData.actionPattern[actionIndex].value;
                if (Hp > MaxHp)
                {
                    Hp = MaxHp;
                }
                StartCoroutine(heal());
                break;
        }

        nextAction();
    }

    private void Attack()
    {
        StartCoroutine(enemyAnimation.atk());
        IsAttack = true;
    }

    private IEnumerator heal()
    {
        HealParticle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        HealParticle.SetActive(false);
    }

    private IEnumerator buff()
    {
        BuffParticle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        BuffParticle.SetActive(false);
    }
}
