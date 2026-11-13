using UnityEngine;

public class CollectibleGlow : MonoBehaviour
{
    public Color glowColor = new Color(1f, 0.9f, 0.4f);
    public float minIntensity = 0.5f;
    public float maxIntensity = 3f;
    public float pulseSpeed = 2f;

    Renderer rend;
    Material mat;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material; //To avoid getting other objects affected
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        mat.SetColor("_EmissionColor", glowColor * intensity);
    }
}