using UnityEngine;

public class PoliceSoundTrigger : MonoBehaviour
{
    public AudioSource sirens;
    public AudioSource talking;
    public bool stopWhenLeaving = false;
    public bool onlyOnce = true;

    private bool hasPlayed;

    public void PlayPoliceSounds()
    {
        if (onlyOnce && hasPlayed)
        {
            return;
        }

        if (sirens != null)
        {
            sirens.Play();
        }

        if (talking != null)
        {
            talking.Play();
        }

        hasPlayed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!stopWhenLeaving || !other.CompareTag("Player"))
        {
            return;
        }

        if (sirens != null)
        {
            sirens.Stop();
        }

        if (talking != null)
        {
            talking.Stop();
        }
    }
}