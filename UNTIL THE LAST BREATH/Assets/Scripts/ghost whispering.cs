using System.Collections;
using UnityEngine;

public class GhostWhispers : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] whispers;          

    [Header("Timing")]
    public float minGap = 4f;            
    public float maxGap = 12f;
    public bool playOnStart = true;

    [Header("Variation")]
    public Vector2 volumeRange = new Vector2(0.3f, 0.8f);
    public Vector2 pitchRange = new Vector2(0.85f, 1.1f);

    [Header("Position (optional)")]
    public Transform player;             
    public float distanceFromPlayer = 4f;

    private Coroutine routine;

    private void Start()
    {
        if (playOnStart) StartWhispers();
    }

    public void StartWhispers()
    {
        if (routine == null) routine = StartCoroutine(WhisperLoop());
    }

    public void StopWhispers()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
        source.Stop();
    }

    private IEnumerator WhisperLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minGap, maxGap));

            if (player != null)
            {
                Vector2 circle = Random.insideUnitCircle.normalized * distanceFromPlayer;
                transform.position = player.position + new Vector3(circle.x, 0f, circle.y);
            }

            source.clip = whispers[Random.Range(0, whispers.Length)];
            source.volume = Random.Range(volumeRange.x, volumeRange.y);
            source.pitch = Random.Range(pitchRange.x, pitchRange.y);
            source.Play();

            yield return new WaitForSeconds(source.clip.length / source.pitch);
        }
    }
}