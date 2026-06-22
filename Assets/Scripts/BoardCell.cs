using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BoardCell : MonoBehaviour
{
    public int row;
    public int col;

    public bool hasStone = false;
    public GoStone GoStone;

    private Button button;

    private BoardManager boardManager;

    private GameObject blackStonePrefab;
    private GameObject whiteStonePrefab;
    private GameObject currentStoneObject;

    public void boardIn(int row, int col, BoardManager boardManager, GameObject blackStonePrefab, GameObject whiteStonePrefab)//보드에 각각 셀 적용
    {
        this.row = row;
        this.col = col;
        this.boardManager = boardManager;
        this.blackStonePrefab = blackStonePrefab;
        this.whiteStonePrefab = whiteStonePrefab;

        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(Click);
        }
    }
    
    private void Click()
    {
        if(boardManager != null)
        {
            boardManager.PlaceStone(this);
        }
    }

    public void SetStone(GoStone stone)
    {
        GoStone = stone;
        hasStone = true;

        CreateStonePrefab(stone);
    }

    private void CreateStonePrefab(GoStone stone)
    {
        GameObject prefabToCreate = null;

        if (stone.GoType == GoType.Black)
        {
            prefabToCreate = blackStonePrefab;
        }
        else if (stone.GoType == GoType.White)
        {
            prefabToCreate = whiteStonePrefab;
        }

        if (prefabToCreate == null)
        {
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
            rect.localScale = Vector3.one*0.9f;
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
    public void ClearStone()
    {
        GoStone = null;
        hasStone = false;

        if (currentStoneObject != null)
        {
            Destroy(currentStoneObject);
            currentStoneObject = null;
        }
    }
}
