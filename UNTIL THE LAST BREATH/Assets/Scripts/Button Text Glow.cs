using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextGlow : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color glowColor = new Color(0.3f, 0.8f, 1f, 1f);
    [SerializeField, Range(0f, 1f)] private float glowPower = 0.6f;
    [SerializeField, Range(0f, 1f)] private float glowOuter = 0.5f;
    [SerializeField] private float fadeSpeed = 10f;

    private Material mat;
    private float current;
    private float goal;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        mat = text.fontMaterial; 
        mat.EnableKeyword(ShaderUtilities.Keyword_Glow);
        mat.SetColor(ShaderUtilities.ID_GlowColor, glowColor);
        mat.SetFloat(ShaderUtilities.ID_GlowOuter, glowOuter);
        mat.SetFloat(ShaderUtilities.ID_GlowPower, 0f);
    }

    private void Update()
    {
        if (Mathf.Approximately(current, goal))
        {
            return;
        }

        current = Mathf.Lerp(current, goal, fadeSpeed * Time.unscaledDeltaTime);
        if (Mathf.Abs(current - goal) < 0.005f)
        {
            current = goal;
        }

        mat.SetFloat(ShaderUtilities.ID_GlowPower, current);
    }

    public void OnPointerEnter(PointerEventData e) => goal = glowPower;
    public void OnPointerExit(PointerEventData e) => goal = 0f;
    public void OnSelect(BaseEventData e) => goal = glowPower;
    public void OnDeselect(BaseEventData e) => goal = 0f;

    private void OnDisable()
    {
        goal = current = 0f;
        if (mat != null)
        {
            mat.SetFloat(ShaderUtilities.ID_GlowPower, 0f);
        }
    }

    private void OnDestroy() { if (mat != null) Destroy(mat); }
}