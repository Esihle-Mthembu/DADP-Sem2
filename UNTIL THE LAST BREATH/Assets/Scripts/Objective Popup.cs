using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectivePopup : MonoBehaviour
{
    //Stops re-appearing if the scene reloads during the same session
    public static bool hasShown;
    public static void ResetShown() => hasShown = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => hasShown = false;

    [Header("UI")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text bodyText;

    [Header("Timing")]
    [SerializeField] private float startDelay = 1.5f;
    [SerializeField] private float fadeTime = 0.6f;
    [SerializeField] private float autoHideAfter = 20f; //0 = only hide on key press

    [Header("Message")]
    [TextArea(8, 14)]
    [SerializeField]
    private string message =
        "You made it to the East Wing.\n\n" +
        "Find and collect all 3 files, then go to Dr Gustavo to get your grandmother's medication. Check the inventory to keep track of your progress.\n\n" +
        "Be quick. The police will raid the hospital soon, and you can't be caught here.\n\n" +
        "Watch out for the ghost that haunts these halls and wards. Use your torch and magnifying glass to help you.\n\n" +
        "<size=70%>Press Enter or Space to close</size>";

    private bool dismissed;

    private void Awake()
    {
        panel.alpha = 0f;
        panel.gameObject.SetActive(false);
    }

    private void Start()
    {
        Debug.Log($"ObjectivePopup Start in scene '{gameObject.scene.name}', hasShown = {hasShown}, timeScale = {Time.timeScale}", this);

        if (hasShown)
        {
            return;
        }

        hasShown = true;
        StartCoroutine(Run());
    }

    private void Update()
    {
        if (!panel.gameObject.activeSelf || dismissed)
        {
            return;
        }

        var kb = Keyboard.current;
        if (kb != null && (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame))
            dismissed = true;
    }

    private IEnumerator Run()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        bodyText.text = message;
        panel.gameObject.SetActive(true);
        yield return Fade(0f, 1f);

        float timer = 0f;
        while (!dismissed && (autoHideAfter <= 0f || timer < autoHideAfter))
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return Fade(1f, 0f);
        panel.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            panel.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        panel.alpha = to;
    }
}