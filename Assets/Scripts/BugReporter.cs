using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class BugReporter : MonoBehaviour
{
    public static BugReporter Instance;

    private const string WEBHOOK_URL = "https://sports-sheriff-groundwater-tract.trycloudflare.com/webhook-test/unity-bug-report";

    private void Awake()
    {
        Instance = this;
    }

    public void SendReport(
        BugReport report)
    {
        StartCoroutine(
            SendCoroutine(report)
        );
    }

    private IEnumerator SendCoroutine(
        BugReport report)
    {
        string json =
            JsonUtility.ToJson(report);

        UnityWebRequest request =
            new UnityWebRequest(
                WEBHOOK_URL,
                "POST"
            );

        byte[] body =
            Encoding.UTF8.GetBytes(json);

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        Debug.Log(
            request.result
        );
    }
}