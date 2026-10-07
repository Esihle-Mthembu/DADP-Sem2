using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Header("Setup")]
    public Shader fadeShader;
    public float fadeOutDuration = 1f;
    public float fadeInDuration = 1f;
    public Color fadeColor = Color.black;

    private Canvas canvas;
    private Image fadeImage;
    private Material fadeMaterial;

    private static readonly int AlphaProp = Shader.PropertyToID("_Alpha");
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildFadeCanvas();
    }

    private void BuildFadeCanvas()
    {
        GameObject canvasGO = new GameObject("ScreenFaderCanvas");
        canvasGO.transform.SetParent(transform);
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // always on top

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Fullscreen image
        GameObject imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imgGO.AddComponent<Image>();
        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        if (fadeShader == null)
            fadeShader = Shader.Find("Custom/ScreenFade");

        fadeMaterial = new Material(fadeShader);
        fadeMaterial.SetColor(ColorProp, fadeColor);
        fadeMaterial.SetFloat(AlphaProp, 0f);

        fadeImage.material = fadeMaterial;
        fadeImage.color = Color.white;
        fadeImage.raycastTarget = true;

        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        fadeMaterial.SetFloat(AlphaProp, a);
        fadeImage.raycastTarget = a > 0.001f;
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutLoadFadeInRoutine(sceneName));
    }

    private IEnumerator FadeOutLoadFadeInRoutine(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
            yield return null;

        yield return null; 

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        SetAlpha(from);

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(to);
    }
}