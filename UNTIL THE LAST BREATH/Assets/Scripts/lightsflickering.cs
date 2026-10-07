using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Base Flicker")]
    [Tooltip("Normal brightness when the light is 'on'.")]
    [SerializeField] private float maxIntensity = 1.2f;
    [Tooltip("Lowest brightness during a normal flicker (not full blackout).")]
    [SerializeField] private float minIntensity = 0.2f;
    [Tooltip("How fast the light flickers, in seconds between changes.")]
    [SerializeField] private float flickerSpeedMin = 0.02f;
    [SerializeField] private float flickerSpeedMax = 0.15f;
    [Tooltip("Chance per flicker step that the light stays steady instead of jumping (keeps it from looking too random/uniform).")]
    [Range(0f, 1f)]
    [SerializeField] private float steadyChance = 0.3f;

    [Header("Dramatic Dead-Outs")]
    [Tooltip("Occasionally the light fully cuts out for a moment, like a failing bulb.")]
    [SerializeField] private bool enableDeadOuts = true;
    [SerializeField] private float deadOutCheckInterval = 4f;
    [Range(0f, 1f)]
    [SerializeField] private float deadOutChance = 0.15f;
    [SerializeField] private float deadOutDurationMin = 0.3f;
    [SerializeField] private float deadOutDurationMax = 1.2f;

    [Header("Optional")]
    [Tooltip("Also flicker an attached MeshRenderer's emission (for a bulb/fixture mesh). Leave empty to skip.")]
    [SerializeField] private Renderer bulbRenderer;
    [SerializeField] private string emissionColorProperty = "_EmissionColor";
    [SerializeField] private Color emissionBaseColor = Color.white;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] flickerBuzzSounds;
    [Range(0f, 1f)]
    [SerializeField] private float buzzSoundChance = 0.1f;

    private Light lightSource;
    private float lastIntensity;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        lightSource.intensity = maxIntensity;
        lastIntensity = maxIntensity;

        if (bulbRenderer != null)
            propBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerRoutine());

        if (enableDeadOuts)
            StartCoroutine(DeadOutRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(flickerSpeedMin, flickerSpeedMax);

            if (Random.value > steadyChance)
            {
                float newIntensity = Random.Range(minIntensity, maxIntensity);
                SetIntensity(newIntensity);

                if (audioSource != null && flickerBuzzSounds.Length > 0 && Random.value < buzzSoundChance)
                {
                    AudioClip clip = flickerBuzzSounds[Random.Range(0, flickerBuzzSounds.Length)];
                    audioSource.PlayOneShot(clip, 0.5f);
                }
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator DeadOutRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(deadOutCheckInterval);

            if (Random.value < deadOutChance)
            {
                float duration = Random.Range(deadOutDurationMin, deadOutDurationMax);
                SetIntensity(0f);
                yield return new WaitForSeconds(duration);
                SetIntensity(maxIntensity);
            }
        }
    }

    private void SetIntensity(float value)
    {
        lastIntensity = value;
        lightSource.intensity = value;

        if (bulbRenderer != null && propBlock != null)
        {
            bulbRenderer.GetPropertyBlock(propBlock);
            float t = maxIntensity > 0f ? Mathf.Clamp01(value / maxIntensity) : 0f;
            propBlock.SetColor(emissionColorProperty, emissionBaseColor * t);
            bulbRenderer.SetPropertyBlock(propBlock);
        }
    }
}