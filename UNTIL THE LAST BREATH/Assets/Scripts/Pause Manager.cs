using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]   //Runs before PuzzleLockUI so it can see the puzzle is still open
public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Puzzle")]
    public PuzzleLockUI puzzleLockUI;

    [Header("Pin Pad")]
    public PinPadUI pinPadUI;

    private bool isPaused = false;

    void Start()
    {
        UIState.Reset(); // Clears any leftover "blocked" state
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Esc closes the puzzle or pinpad first, not the pause menu
            if (puzzleLockUI != null && puzzleLockUI.IsOpen)
            {
                return;
            }

            if (pinPadUI != null && pinPadUI.IsOpen)
            {
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        pauseMenu.SetActive(true);
        pauseMenu.transform.SetAsLastSibling(); //Goes on top of the pinpad and other panels
        Time.timeScale = 0f;
        UIState.Push(); //Frees the cursor and blocks player input
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        UIState.Pop(); //Locks cursor again only if nothing else is open
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        UIState.Reset();
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        SceneManager.LoadScene("Main Menu Scene");
    }
}