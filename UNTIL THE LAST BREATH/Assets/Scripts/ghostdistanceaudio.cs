using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GhostDistanceAudio : MonoBehaviour
{
    public Transform playerCamera;
    public float fullVolumeDistance = 20f;
    public float silentDistance = 80f;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        source.spatialBlend = 0f;
        source.dopplerLevel = 0f;
        source.pitch = 1f;
        source.loop = true;
        source.mute = false;
        source.volume = 0f;
        source.Play();
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            source.volume = 0f;
            return;
        }

        Vector3 offset = transform.position - playerCamera.position;
        offset.y = 0f;

        source.volume = Mathf.InverseLerp(
            silentDistance,
            fullVolumeDistance,
            offset.magnitude
        );
    }
}