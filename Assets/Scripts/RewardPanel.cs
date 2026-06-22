using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    public Button blackStoneButton;
    public Button whiteStoneButton;
    public Button skipButton;

    private PlayerManager player;

    private void Awake()
    {
        BindRewardButton(blackStoneButton, SelectBlackStone);
        BindRewardButton(whiteStoneButton, SelectWhiteStone);
        BindRewardButton(skipButton, SkipButton);
    }

    private void BindRewardButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(action);
    }

    public void Open(PlayerManager player)
    {
        this.player = player;

        gameObject.SetActive(true);
    }

    private void SelectBlackStone()
    {
        if (player != null)
        {
            player.AddStone(GoType.Black);
        }
        SceneManager.LoadScene("Map");
    }

    private void SelectWhiteStone()
    {
        if (player != null)
        {
            player.AddStone(GoType.White);
        }
        SceneManager.LoadScene("Map");
    }

    private void SkipButton()
    {
        SceneManager.LoadScene("Map");
    }
}