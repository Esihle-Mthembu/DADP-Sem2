using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button continueButton;

    void Start()
    {
        continueButton.interactable = SaveManager.Instance.HasSave();
    }

    // New game button
    public void NewGame()
    {
        ObjectivePopup.ResetShown();

        PlayerPrefs.DeleteAll();
        SaveManager.Instance.ResetSave();
        SceneManager.LoadScene("Intro Scene");
    }

    // Continue button
    public void ContinueGame()
    {
        ObjectivePopup.hasShown = true;
        SaveManager.Instance.Continue();
    }

    // Exit button
    public void QuitGame()
    {
        Application.Quit();
    }
}
