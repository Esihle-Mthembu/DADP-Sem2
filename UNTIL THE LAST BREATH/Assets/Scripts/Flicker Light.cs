using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    [SerializeField] float radiusFromPlayer = 12f; //Flicker the lights close to the player
    [SerializeField] float flickerSpeed = 25f;

    Light lamp;
    float baseIntensity;
    float seed;

    void Awake()
    {
        lamp = GetComponent<Light>();
        baseIntensity = lamp.intensity;
        seed = Random.value * 100f;   //Lights shouldn't all flicker in sync
    }

    void Update()
    {
        GhostProximity prox = GhostProximity.Instance;
        if (prox == null || prox.Player == null)
        {
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, prox.Player.position);
        float nearPlayer = Mathf.InverseLerp(radiusFromPlayer, radiusFromPlayer * 0.5f, distToPlayer);
        float strength = prox.Intensity * nearPlayer;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, seed);
        float flicker = Mathf.Clamp01((noise - 0.25f) * 3f);

        lamp.intensity = baseIntensity * Mathf.Lerp(1f, flicker, strength);
    }
}