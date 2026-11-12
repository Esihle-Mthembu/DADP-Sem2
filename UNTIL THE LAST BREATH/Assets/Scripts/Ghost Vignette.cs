using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class GhostVignette : MonoBehaviour
{
    [Header("Post-processing vignette")]
    [SerializeField] float maxIntensity = 0.6f;
    [SerializeField] float pulseSpeed = 6f;
    [SerializeField] float pulseAmount = 0.08f;

    [Header("Cloud overlay")]
    [SerializeField] RectTransform cloudOverlay;
    [SerializeField] CanvasGroup cloudGroup;
    [SerializeField] float farScale = 1.6f;
    [SerializeField] float nearScale = 1.0f;

    Vignette vignette;

    void Awake()
    {
        Volume volume = GetComponent<Volume>();
        if (!volume.profile.TryGet(out vignette))
        {
            Debug.LogWarning("GhostVignette: add a vignette override to the Volume profile", this);
        }
    }

    void Update()
    {
        float i = GhostProximity.Instance != null ? GhostProximity.Instance.Intensity : 0f;

        if (vignette != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            vignette.intensity.Override(Mathf.Clamp01(i * maxIntensity * pulse));
        }

        if (cloudOverlay != null)
        {
            float s = Mathf.Lerp(farScale, nearScale, i);
            cloudOverlay.localScale = new Vector3(s, s, 1f);
        }

        if (cloudGroup != null)
        {
            cloudGroup.alpha = i;
        }
    }
}