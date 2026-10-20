using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SaveNotification : MonoBehaviour
{
    [SerializeField] float fadeInTime = 0.2f;
    [SerializeField] float holdTime = 1.5f;
    [SerializeField] float fadeOutTime = 0.5f;

    CanvasGroup group;
    Coroutine routine;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnSaved += Show;
        }
    }

    void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnSaved -= Show;
        }
    }

    void Show()
    {
        if (routine != null) StopCoroutine(routine);
        {
            routine = StartCoroutine(ShowRoutine());
        }
    }

    IEnumerator ShowRoutine()
    {
        yield return Fade(group.alpha, 1f, fadeInTime);
        yield return new WaitForSecondsRealtime(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);
        routine = null;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        group.alpha = to;
    }
}