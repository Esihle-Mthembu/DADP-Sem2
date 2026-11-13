using UnityEngine;
using TMPro;

public class GuidePopUpManager : MonoBehaviour
{
    public GameObject popup;
    public GameObject pausePanel;

    public void ShowGuide()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        popup.SetActive(true);
    }

    public void CloseGuide()
    {
        popup.SetActive(false);
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
}