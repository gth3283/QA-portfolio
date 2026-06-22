using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public PlayerManager player;
    public EnemyManager enemy;
    public BoardManager boardManager;
    public EnemySpawner enemySpawner;
    public PlayerAnimation ani;
    public RewardPanel rewardPanel;
    public GameObject Ending;

    public RectTransform handPanel;
    public Button endTurnButton;

    public Slider playerHpSlider;
    public Slider enemyHpSlider;

    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;

    public TMP_Text TurnCountText;
    private int TurnCount;

    public int drawCount = 5;
    public Vector2 handStoneSize = new Vector2(90f, 90f);
    public Vector2 handStoneSpacing = new Vector2(10f, 10f);

    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;

    private List<GoStone> drawPile = new List<GoStone>();
    private List<GoStone> hand = new List<GoStone>();
    private List<GoStone> discardPile = new List<GoStone>();
    private List<HandStone> handStoneUIs = new List<HandStone>();

    private int selectedHandIndex = -1;

    private bool isPlayerTurn = true;

    private void Start()
    {
        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.onClick.AddListener(EndPlayerTurn);
        }
        TurnCount = 1;
        if(enemySpawner != null)
        {
            if (GameManager.boss)
            {
                enemySpawner.SpawnBoss();
            }
            else
            {
                enemySpawner.SpawnNormal();
            }
        }
        InitBattle();
    }

    private void InitBattle()
    {
        drawPile.Clear();
        hand.Clear();
        discardPile.Clear();

        CreateDrawPile();
        Shuffle(drawPile);
        DrawStones(drawCount);
        RefreshHandUI();
        UpdateHpUI();

        TurnCountText.text = "Turn : " + TurnCount.ToString();
    }

    private void CreateDrawPile()
    {
        drawPile.Clear();

        foreach (GoStone stone in player.deck)
        {
            drawPile.Add(new GoStone(stone.GoType));
        }
    }

    private void DrawStones(int count)
    {
        enemy.ShowDetailAct();

        if (hand.Count > 0)
        {
            foreach (GoStone stone in hand)
            {
                discardPile.Add(new GoStone(stone.GoType));
            }

            hand.Clear();
        }

        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count <= 0)
            {
                if (discardPile.Count > 0)
                {
                    foreach (GoStone stone in discardPile)
                    {
                        drawPile.Add(new GoStone(stone.GoType));
                    }

                    discardPile.Clear();
                    Shuffle(drawPile);
                }
            }
            if (drawPile.Count <= 0)
            {
                break;
            }

            GoStone drawnStone = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(drawnStone);
        }

        selectedHandIndex = -1;
    }
    private void RefillDrawPileFromDiscard()
    {
        if (discardPile.Count <= 0)
        {
            return;
        }

        foreach (GoStone stone in discardPile)
        {
            drawPile.Add(new GoStone(stone.GoType));
        }

        discardPile.Clear();
        Shuffle(drawPile);
    }

    private void RefreshHandUI()
    {
        ClearHandPanel();
        SetupHandLayout();

        handStoneUIs.Clear();

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject stoneObject = CreateHandStoneObject(i);
            HandStone handStoneUI = stoneObject.AddComponent<HandStone>();
            handStoneUI.BoardIn(hand[i], i, this, blackStonePrefab, whiteStonePrefab);

            handStoneUIs.Add(handStoneUI);
        }
    }

    private void ClearHandPanel()
    {
        for (int i = handPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(handPanel.GetChild(i).gameObject);
        }
    }

    private void SetupHandLayout()
    {
        HorizontalLayoutGroup layout = handPanel.GetComponent<HorizontalLayoutGroup>();

        if (layout == null)
        {
            layout = handPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        layout.spacing = handStoneSpacing.x;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private GameObject CreateHandStoneObject(int index)
    {
        GameObject stoneObject = new GameObject($"HandStone_{index}", typeof(RectTransform));
        stoneObject.transform.SetParent(handPanel, false);

        RectTransform rect = stoneObject.GetComponent<RectTransform>();
        rect.sizeDelta = handStoneSize;

        Image image = stoneObject.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.01f);
        image.raycastTarget = true;

        Button button = stoneObject.AddComponent<Button>();

        return stoneObject;
    }

    public void SelectStone(int handIndex)
    {
        if (!isPlayerTurn)
        {
            return;
        }

        if (handIndex < 0 || handIndex >= hand.Count)
        {
            return;
        }

        selectedHandIndex = handIndex;

        for (int i = 0; i < handStoneUIs.Count; i++)
        {
            handStoneUIs[i].SetSelected(i == selectedHandIndex);
        }
    }

    public GoStone GetSelectedStone()
    {
        if (selectedHandIndex < 0 || selectedHandIndex >= hand.Count)
        {
            return null;
        }

        return hand[selectedHandIndex];
    }

    public void UseSelectedStone()
    {
        if (selectedHandIndex < 0 || selectedHandIndex >= hand.Count)
        {
            return;
        }

        hand.RemoveAt(selectedHandIndex);
        selectedHandIndex = -1;

        RefreshHandUI();
    }

    private int GetMultiplier(int maxLine)
    {
        if (maxLine >= 6)
        {
            return 5;
        }

        if (maxLine == 5)
        {
            return 4;
        }

        if (maxLine == 4)
        {
            return 3;
        }

        if (maxLine == 3)
        {
            return 2;
        }

        return 1;
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn)
        {
            return;
        }

        StartCoroutine(EndPlayerTurnRoutine());
    }

    private IEnumerator EndPlayerTurnRoutine()
    {
        isPlayerTurn = false;

        List<GoStone> placedStones = boardManager.GetPlacedStones();

        int damage = 0;
        int defense = 0;

        if (placedStones.Count > 0)
        {
            int blackCount = 0;
            int whiteCount = 0;

            foreach (GoStone stone in placedStones)
            {
                if (stone.GoType == GoType.Black)
                {
                    blackCount++;
                }
                else if (stone.GoType == GoType.White)
                {
                    whiteCount++;
                }
            }

            int blackMaxLine = boardManager.GetMaxConnectedLine(GoType.Black);
            int whiteMaxLine = boardManager.GetMaxConnectedLine(GoType.White);

            int blackMultiplier = GetMultiplier(blackMaxLine);
            int whiteMultiplier = GetMultiplier(whiteMaxLine);

            damage = blackCount * blackMultiplier;
            defense = whiteCount * whiteMultiplier;

            Debug.Log(
                $"검은돌 {blackCount}개 / 최대 연결 {blackMaxLine} / 배율 x{blackMultiplier} / 피해 {damage}\n" +
                $"흰돌 {whiteCount}개 / 최대 연결 {whiteMaxLine} / 배율 x{whiteMultiplier} / 방어도 {defense}"
            );
        }

        if (damage > 0)
        {
            yield return StartCoroutine(ani.atk());

            enemy.TakeDamage(damage);
            UpdateHpUI();
        }

        if(enemy.Hp <= 0)
        {
            GameManager.SavePlayerHp(player.HP, player.MaxHP);

            if (GameManager.boss)
            {
                Ending.SetActive(true);
            }

            if (rewardPanel != null)
            {
                rewardPanel.Open(player);
            }


            yield break;
        }

        enemy.DoEnemyAction();

        if (enemy.IsAttack)
        {
            yield return StartCoroutine(ani.def());

            int finalDamage = enemy.AttackPower - defense;

            if (finalDamage < 0)
            {
                finalDamage = 0;
            }

            player.TakeDamage(finalDamage);
            UpdateHpUI();
        }

        Debug.Log(
            $"적 공격 처리 완료 | 적 공격력 {enemy.AttackPower}, 방어도 {defense}"
        );

        FinishTurnAfterBattle(placedStones);
    }

    private void FinishTurnAfterBattle(List<GoStone> placedStones)
    {
        foreach (GoStone stone in placedStones)
        {
            discardPile.Add(new GoStone(stone.GoType));
        }

        foreach (GoStone stone in hand)
        {
            discardPile.Add(new GoStone(stone.GoType));
        }

        hand.Clear();
        selectedHandIndex = -1;

        boardManager.ClearBoard();

        DrawStones(drawCount);
        RefreshHandUI();

        isPlayerTurn = true;
        UpdateHpUI();

        TurnCount++;
        TurnCountText.text = "Turn : " + TurnCount.ToString();
    }
   
    private void Shuffle(List<GoStone> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            GoStone temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void UpdateHpUI()
    {
        playerHpSlider.maxValue = player.MaxHP;
        playerHpSlider.value = player.HP;

        enemyHpSlider.maxValue = enemy.MaxHp;
        enemyHpSlider.value = enemy.Hp;

        playerHpText.text = $"{player.HP} / {player.MaxHP}";


        enemyHpText.text = $"{enemy.Hp} / {enemy.MaxHp}";
    }
}
