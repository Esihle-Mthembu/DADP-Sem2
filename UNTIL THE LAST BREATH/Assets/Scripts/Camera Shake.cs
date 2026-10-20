using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float maxAmount = 0.04f;
    [SerializeField] float speed = 25f;

    Vector3 lastOffset;

    void LateUpdate()
    {
        float i = GhostProximity.Instance != null ? GhostProximity.Instance.Intensity : 0f;

        float t = Time.time * speed;
        Vector3 offset = new Vector3(
            Mathf.PerlinNoise(t, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, t) - 0.5f,
            0f) * 2f * maxAmount * i;

        transform.localPosition += offset - lastOffset;
        lastOffset = offset;
    }
}