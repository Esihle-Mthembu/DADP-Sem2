using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneController : MonoBehaviour
{
    [Header("Opening Scene")]
    public string nextSceneName = "Main Menu Scene";

    // Begin button
    public void BeginGame()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // Exit button
    public void ExitGame()
    {
        Debug.Log("Game is exiting");
        Application.Quit();
    }
}
