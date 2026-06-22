using UnityEngine;
using UnityEngine.SceneManagement;

public class TItle : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Map");
    }
}
