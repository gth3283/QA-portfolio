using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HandStone : MonoBehaviour
{
    public GoStone stone;
    public int handIndex;

    private BattleManager battleManager;
    private Button button;

    private GameObject BlackStone;
    private GameObject WhiteStone;
    private GameObject currentStoneObject;

    public void BoardIn(GoStone stone, int handIndex, BattleManager battleManager, GameObject BlackStone, GameObject WhiteStone)
    {
        this.stone = stone;
        this.handIndex = handIndex;
        this.battleManager = battleManager;
        this.BlackStone = BlackStone;
        this.WhiteStone = WhiteStone;

        button = GetComponent<Button>();

        ClearStone();
        CreateStonePrefab();

        if (button != null)
        {
            button.onClick.AddListener(OnClickHandStone);
        }
    }

    private void CreateStonePrefab()
    {
        GameObject prefabToCreate = null;

        if (stone.GoType == GoType.Black)
        {
            prefabToCreate = BlackStone;
        }
        else if (stone.GoType == GoType.White)
        {
            prefabToCreate = WhiteStone;
        }

        if (prefabToCreate == null)
        {
            Debug.LogError($"{stone.GoType} 프리팹이 연결되지 않았습니다.");
            return;
        }

        currentStoneObject = Instantiate(prefabToCreate, transform);

        RectTransform rect = currentStoneObject.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
        else
        {
            currentStoneObject.transform.localPosition = Vector3.zero;
            currentStoneObject.transform.localScale = Vector3.one;
        }

        Graphic[] graphics = currentStoneObject.GetComponentsInChildren<Graphic>();

        foreach (Graphic graphic in graphics)
        {
            graphic.raycastTarget = false;
        }
    }

    private void ClearStone()
    {
        if (currentStoneObject != null)
        {
            Destroy(currentStoneObject);
            currentStoneObject = null;
        }
    }

    private void OnClickHandStone()
    {
        if (battleManager != null)
        {
            battleManager.SelectStone(handIndex);
        }
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? Vector3.one * 1.15f : Vector3.one;
    }
}
