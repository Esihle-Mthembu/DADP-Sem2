using UnityEngine;
using TMPro;

public class GuidePopUpManager : MonoBehaviour
{
    public GameObject popup;

    public void ShowGuide()
    {
        popup.SetActive(true);
    }

    public void CloseGuide()
    {
        popup.SetActive(false);
    }
}
