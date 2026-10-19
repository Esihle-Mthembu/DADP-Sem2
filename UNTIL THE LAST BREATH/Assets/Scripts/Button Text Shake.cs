using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextShake : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float magnitude = 3f;
    [SerializeField] private float speed = 40f;

    private Vector2 origin;
    private bool shaking;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponentInChildren<TMP_Text>().rectTransform;
        }

        origin = target.anchoredPosition;
    }

    private void Update()
    {
        if (!shaking)
        {
            return;
        }

        float t = Time.unscaledTime * speed;
        float x = Mathf.PerlinNoise(t, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, t) - 0.5f;
        target.anchoredPosition = origin + new Vector2(x, y) * 2f * magnitude;
    }

    private void StartShake() => shaking = true;

    private void StopShake()
    {
        shaking = false;
        target.anchoredPosition = origin;
    }

    public void OnPointerEnter(PointerEventData e) => StartShake();
    public void OnPointerExit(PointerEventData e) => StopShake();
    public void OnSelect(BaseEventData e) => StartShake();
    public void OnDeselect(BaseEventData e) => StopShake();
    private void OnDisable() 
    {
        if (target != null)
        {
            StopShake();
        }
    }
}