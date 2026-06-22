using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QAEventLogger
{
    public static void LogEvent(
        string eventType,
        string message = "")
    {
        Debug.Log(
            $"[QA_EVENT] {eventType} | {message}"
        );

        BugReport report = new BugReport
        {
            eventType = eventType,
            message = message,

            sceneName =
                SceneManager.GetActiveScene().name,

            currentHp =
                GameManager.PlayerCurrentHp,

            maxHp =
                GameManager.PlayerMaxHp,

            mapStep =
                GameManager.MapCurrentStep,

            selectedRoute =
                GameManager.SelectedRoute1
                    ? "Route1"
                    : "Route2",

            blackStone =
                GameManager.BlackStoneCount,

            whiteStone =
                GameManager.WhiteStoneCount,

            isBoss =
                GameManager.boss,

            timestamp =
                DateTime.Now.ToString()
        };

        BugReporter.Instance.SendReport(report);
    }
}