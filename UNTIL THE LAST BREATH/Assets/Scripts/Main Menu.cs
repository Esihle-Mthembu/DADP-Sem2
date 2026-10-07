using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // New game button
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Intro Scene");
    }

    // Continue button
    public void ContinueGame()
    {
        
    }

    // Exit button
    public void QuitGame()
    {
        Application.Quit();
    }
}
