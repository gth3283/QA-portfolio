using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class BoardManager : MonoBehaviour
{
    public int rows = 5;
    public int cols = 8;

    public RectTransform boardPanel;

    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;


    public Vector2 cellSize = new Vector2(80f, 80f);
    public Vector2 cellSpacing = new Vector2(5f, 5f);

    private Color cellBackgroundColor = new Color(0.85f, 0.65f, 0.4f, 1f);

    private BoardCell[,] boardCells;

    private BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    private void Start()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        ClearBoardPanel();
        SetupGridLayout();

        boardCells = new BoardCell[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < cols; column++)
            {
                GameObject cellObject = CreateCellObject(row, column);
                BoardCell cellUI = cellObject.AddComponent<BoardCell>();
                cellUI.boardIn(row, column, this, blackStonePrefab, whiteStonePrefab);

                boardCells[row, column] = cellUI;
            }
        }
    }

    private void ClearBoardPanel()
    {
        for (int i = boardPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(boardPanel.GetChild(i).gameObject);
        }
    }

    private void SetupGridLayout()
    {
        GridLayoutGroup grid = boardPanel.GetComponent<GridLayoutGroup>();

        if (grid == null)
        {
            grid = boardPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;
        grid.cellSize = cellSize;
        grid.spacing = cellSpacing;
        grid.childAlignment = TextAnchor.MiddleCenter;
    }

    private GameObject CreateCellObject(int row, int column)
    {
        GameObject cellObject = new GameObject($"Cell_{row}_{column}", typeof(RectTransform));
        cellObject.transform.SetParent(boardPanel, false);

        Image image = cellObject.AddComponent<Image>();
        image.color = cellBackgroundColor;

        Button button = cellObject.AddComponent<Button>();

        return cellObject;
    }

    public void PlaceStone(BoardCell cell)
    {
        if (cell.hasStone)
        {
            Debug.Log("이미 바둑돌이 놓인 위치입니다.");
            return;
        }

        GoStone selectedStone = battleManager.GetSelectedStone();

        if (selectedStone == null)
        {
            Debug.Log("먼저 손패에서 바둑돌을 선택하세요.");
            return;
        }

        cell.SetStone(new GoStone(selectedStone.GoType));
        battleManager.UseSelectedStone();

        Debug.Log($"바둑돌 배치 완료: {selectedStone.GoType}, 위치: ({cell.row}, {cell.col})");
    }

    public List<GoStone> GetPlacedStones()
    {
        List<GoStone> stones = new List<GoStone>();

        BoardCell[] cells = boardPanel.GetComponentsInChildren<BoardCell>(true);

        foreach (BoardCell cell in cells)
        {
            if (cell != null && cell.hasStone && cell.GoStone != null)
            {
                stones.Add(cell.GoStone);
            }
        }

        return stones;
    }

    private void SyncBoardCellsFromPanel()
    {
        if (boardCells == null || boardCells.GetLength(0) != rows || boardCells.GetLength(1) != cols)
        {
            boardCells = new BoardCell[rows, cols];
        }

        BoardCell[] cells = boardPanel.GetComponentsInChildren<BoardCell>(true);

        foreach (BoardCell cell in cells)
        {
            if (cell == null)
            {
                continue;
            }

            boardCells[cell.row, cell.col] = cell;
        }
    }

    public int GetMaxConnectedLine(GoType targetType)
    {
        SyncBoardCellsFromPanel();

        int maxLine = 0;

        int[,] directions =
        {
            { 0, 1 },   // 가로
            { 1, 0 },   // 세로
            { 1, 1 },   // 대각선 우하향
            { 1, -1 }   // 대각선 좌하향
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                BoardCell cell = boardCells[row, col];

                if (!IsSameStone(cell, targetType))
                {
                    continue;
                }

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int rowDir = directions[i, 0];
                    int colDir = directions[i, 1];

                    int lineLength = CountLineBothDirections(row, col, rowDir, colDir, targetType);

                    if (lineLength > maxLine)
                    {
                        maxLine = lineLength;
                    }
                }
            }
        }

        return maxLine;
    }

    private int CountLineBothDirections(
    int startRow,
    int startCol,
    int rowDir,
    int colDir,
    GoType targetType
)
    {
        int count = 1;

        count += CountOneDirection(startRow, startCol, rowDir, colDir, targetType);
        count += CountOneDirection(startRow, startCol, -rowDir, -colDir, targetType);

        return count;
    }

    private int CountOneDirection(
        int startRow,
        int startCol,
        int rowDir,
        int colDir,
        GoType targetType
    )
    {
        int count = 0;

        int currentRow = startRow + rowDir;
        int currentCol = startCol + colDir;

        while (IsInsideBoard(currentRow, currentCol))
        {
            BoardCell cell = boardCells[currentRow, currentCol];

            if (!IsSameStone(cell, targetType))
            {
                break;
            }

            count++;

            currentRow += rowDir;
            currentCol += colDir;
        }

        return count;
    }

    private bool IsSameStone(BoardCell cell, GoType targetType)
    {
        if (cell == null)
        {
            return false;
        }

        if (!cell.hasStone)
        {
            return false;
        }

        if (cell.GoStone == null)
        {
            return false;
        }

        return cell.GoStone.GoType == targetType;
    }

    private bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
    public void ClearBoard()
    {
        BoardCell[] cells = boardPanel.GetComponentsInChildren<BoardCell>(true);

        foreach (BoardCell cell in cells)
        {
            if (cell != null)
            {
                cell.ClearStone();
            }
        }

        Debug.Log($"[BoardManager] ClearBoard 완료: {cells.Length}개 셀 검사");
    }
}
