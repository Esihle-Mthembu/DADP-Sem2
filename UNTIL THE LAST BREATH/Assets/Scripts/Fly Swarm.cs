using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FlySwarm : MonoBehaviour
{
    [SerializeField] private float driftRadius = 0.15f; //How far the swarm center can wander from its start point
    [SerializeField] private float driftSpeed = 0.3f; //How fast the swarm center wanders
    [SerializeField] private bool randomizePhaseOnStart = true; //Randomized movement so multiple swarms don't move in sync

    private Vector3 _origin;
    private float _seedX, _seedY, _seedZ;

    private void Start()
    {
        _origin = transform.localPosition;

        if (randomizePhaseOnStart)
        {
            _seedX = Random.Range(0f, 100f);
            _seedY = Random.Range(0f, 100f);
            _seedZ = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float t = Time.time * driftSpeed;

        float x = (Mathf.PerlinNoise(t, _seedX) - 0.5f) * 2f * driftRadius;
        float y = (Mathf.PerlinNoise(t, _seedY) - 0.5f) * 2f * driftRadius * 0.5f; // less vertical drift
        float z = (Mathf.PerlinNoise(t, _seedZ) - 0.5f) * 2f * driftRadius;

        transform.localPosition = _origin + new Vector3(x, y, z);
    }
}
