using NUnit.Framework;
using UnityEngine;

public static class GameManager
{
    public static int PlayerMaxHp = 100;
    public static int PlayerCurrentHp = 100;
    public static int BlackStoneCount = 5;
    public static int WhiteStoneCount = 5;

    public static int MapCurrentStep = 0;
    public static bool SelectedRoute1 = true;
    public static bool HasSelectedRoute = false;

    public static bool HasRunStarted = false;
    public static bool HasGeneratedMap = false;
    public static bool boss = false;

    public static MapType[] _1Types = new MapType[3];
    public static MapType[] _2Types = new MapType[3];
    public static Vector3 playerIcon;

    public static void StartNewRun()
    {
        PlayerMaxHp = 100;
        PlayerCurrentHp = 100;
        BlackStoneCount = 5;
        WhiteStoneCount = 5;

        MapCurrentStep = 0;
        SelectedRoute1 = true;
        HasSelectedRoute = false;

        HasGeneratedMap = false;
        HasRunStarted = true;
    }

    public static void SavePlayerHp(int currentHp, int maxHp)
    {
        PlayerMaxHp = maxHp;
        PlayerCurrentHp = currentHp;

        if (PlayerCurrentHp < 0)
        {
            PlayerCurrentHp = 0;
        }

        if (PlayerCurrentHp > PlayerMaxHp)
        {
            PlayerCurrentHp = PlayerMaxHp;
        }
    }
}