using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int MaxHP;
    public int HP;

    public List<GoStone> deck = new List<GoStone>();
    public TMP_Text StoneCount;
    public SpriteRenderer spriteRenderer;
    public GameObject Retry;

    private int tocount;
    public void Exit()
    {
        Application.Quit();
    }
    private void Awake()
    {
        Retry.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();

        LoadHp();

        if (deck.Count == 0)
        {
            CreateDeck();
        }
        tocount = deck.Count;
        StoneCount.text = "Stone: "+ tocount.ToString();
    }

    private void LoadHp()
    {
        MaxHP = GameManager.PlayerMaxHp;
        HP = GameManager.PlayerCurrentHp;
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
        if (HP < 0)
        {
            HP = 0;
        }
    }

    private void CreateDeck()
    {
        deck.Clear();

        for (int i = 0; i < GameManager.WhiteStoneCount; i++)
        {
            deck.Add(new GoStone(GoType.White));
        }

        for (int i = 0; i < GameManager.BlackStoneCount; i++)
        {
            deck.Add(new GoStone(GoType.Black));
        }
    }

    public void TakeDamage(int Damage)
    {
        HP -= Damage;
        if (HP < 0)
        {
            HP = 0;
        }

        if (HP <= 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            spriteRenderer.flipY = true;
            StopAllCoroutines();
            QAEventLogger.LogEvent("Player_Dead","플레이어 사망");
            Retry.SetActive(true);
        }
    }

    public void Heal(int Heal)
    {
        HP += Heal;

        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
    }

    public void AddStone(GoType stone)
    {
        deck.Add(new GoStone(stone));

        if (stone == GoType.Black)
        {
            GameManager.BlackStoneCount++;
        }
        else if (stone == GoType.White)
        {
            GameManager.WhiteStoneCount++;
        }

        tocount = deck.Count;

        if (StoneCount != null)
        {
            StoneCount.text = "Stone: " + tocount.ToString();
        }
    }

    public void RemoveStone(GoStone stone)
    {
        if (deck.Contains(stone))
        {
            deck.Remove(stone);
            tocount--;
            StoneCount.text = "Stone: " + tocount.ToString();
        }
    }
}
