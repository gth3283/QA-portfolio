<h1>AI 리포트 자동화 시스템</h1>
&nbsp;해당 프로젝트는 QA의 프로세스를 자동화하고 Unity와 n8n을 Webhook으로 연결하여 실시간 정보수집 및 발생 시각 파악에 용이하도록 제작한 프로젝트입니다. 
<h3>구성</h3>
<ul>
  <li>
    Unity: 게임 및 로그 제공
  </li>
  <li>
    n8n: 자동화 도구
  </li>
  <li>
    Ollama(Qwen3): AI
  </li>
  <li>
    Jira: 버그 리포트 표시
  </li>
  <li>
    (임시) sql로 로그 DB 관리
  </li>
</ul>
<hr>
<br>
<h2>1주차 작업</h2>
<h3>Unity 코드</h3>
&nbsp;임시로 로그를 참조할 게임은 미리 만들어둔 Unity 기반 로그라이크 게임으로 진행하였습니다. 다음은 리포트를 작성하기 위한 스크립트입니다. 
<br><br>

&nbsp;먼저 기존 게임에서 받아올 정보들과 리포트의 타입, 추가 메세지를 생성하기 위해 클래스의 형태로 정의하여 주었습니다.
```cs
[System.Serializable]
public class BugReport
{
    public string eventType;

    public string message;

    public string sceneName;

    public int currentHp;

    public int maxHp;

    public int mapStep;

    public string selectedRoute;

    public int blackStone;

    public int whiteStone;

    public bool isBoss;

    public string timestamp;
}
```
<br>

&nbsp;다음으로 Webhook의 URL로 BugReport를 Json형태로 전달하는 스크립트를 작성하였습니다. 코루틴을 사용하면서 다른 씬을 이동하는 등의 상황에도 지속해서 동작하도록 MonoBehaviour로 생성하였습니다.
```cs
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class BugReporter : MonoBehaviour
{
    public static BugReporter Instance;

    private const string WEBHOOK_URL = "<WEBHOOK_URL>";//해당 부분에 URL 기입

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
```
<br>

&nbsp;마지막으로 각 상황 발생시에 호출하여 로그에 정보를 기입하고 생성할 스크립트를 작성하였습니다. 이제 다른 스크립트에서 플레이어 사망, 적 처치, 스테이지 이동 등이 처리될때 해당 클래스를 호출하여 최종적으로 BugReporter를 통해 WebHook으로 전송할 것입니다.
```cs
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QAEventLogger
{
    public static void LogEvent(
        string eventType,
        string message = "")
    {
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
```
<hr>
<h3>n8n 작동 확인</h3>
&nbsp;n8n은 Docker를 이용해 셀프 호스팅하였고, Webhook을 위하여 로컬환경이 아닌 cloudflare를 이용한 호스팅을 해주었습니다. 우선 테스트를 위하여 Webhook을 받고 아무 요청이나 한번이라도 보내진다면 저에게 메일을 보내도록 구현하였습니다.
<img width="792" height="472" alt="image" src="https://github.com/user-attachments/assets/d82234d7-8d82-4777-9000-eeffdb600907" />
<br><br>

&nbsp;AI 에이전트에게 테스트용으로 준 프롬포트는 다음과 같습니다.

```
너는 QA테스트를 도와줄 AI 비서야.

일단 지금은 테스트단계이고 Webhook을 통해 로그를 확인하고 조회가 된다면 나에게 보내줘.
```
<br>
&nbsp;해당 프롬포트 입력 후 Unity 스크립트에서 적 조우 이벤트 발생 시 적의 종류와 현재 체력 등을 전송하도록 코드를 추가하였습니다

```
QAEventLogger.LogEvent("적 조우 이벤트", $"{index}번 적 조우");
```

&nbsp;이후 게임 플레이 중 적 이벤트 조우가 발생하였을 때 n8n의 로그에 정상적으로 AI 에이전트에게 전달이 되는 것과 메일이 전송되는 것을 확인할 수 있었습니다.
<img width="1857" height="553" alt="image" src="https://github.com/user-attachments/assets/ec58256e-7c18-41f8-8bbd-07393648d053" />
<img width="273" height="143" alt="image" src="https://github.com/user-attachments/assets/378a65f5-a2d5-4574-aeae-866356163031" />
<br>
<hr>
<h3>Jira 연결</h3>

&nbsp;이제 정상적으로 Webhook 전송 및 받아오기가 되는 것을 확인하였으니 Unity에서 상황에 따라 다른 메세지를 생성하도록 추가하였습니다. 그리고 기존 메일 전송으로 테스트하던 노드를 Jira에 업무 항목을 추가하는 방식으로 수정하여 문제 발생시 담당자가 확인 가능하도록 변경하였습니다.

```cs
QAEventLogger.LogEvent("Enemy_Dead", $"Kill {enemy.CurrentEnemyData.Name} ID:{enemy.CurrentEnemyData.Id}"); //적 처치 로그
QAEventLogger.LogEvent("Player_Dead", $"Killed By {enemy.CurrentEnemyData.Name} ID:{enemy.CurrentEnemyData.Id}"); //플레이어 사망 로그
QAEventLogger.LogEvent("보상 선택", $"{stone} 추가"); //보상 선택 로그
```
<br>
&nbsp;이제 Webhook을 테스트링크가 아닌 실시간으로 받아오도록 워크플로우를 Publish해준 뒤 게임을 실행하여 정상적으로 이슈 발생시 Jira에 추가되는지 확인한 결과 플레이어가 사망할 경우 n8n에서 정상적으로 플레이어 사망처리 로그가 확인되는 것을 볼 수 있고, Jira에도 업무가 생성되었습니다.
<img width="757" height="424" alt="image" src="https://github.com/user-attachments/assets/e25204ae-b5a0-4b18-af35-055253b69fe6" />
<img width="1554" height="308" alt="image" src="https://github.com/user-attachments/assets/7d6d9e52-b604-48e5-b00a-cff412f427bc" />
<img width="1031" height="567" alt="image" src="https://github.com/user-attachments/assets/98718481-005f-479a-97ab-c0cad6efb1b0" />

