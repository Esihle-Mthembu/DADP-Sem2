using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public ElevatorTeleport elevator;
    public AudioSource buttonSound;        

    public void Press()
    {
        if (buttonSound != null) buttonSound.Play();
        elevator.GoToDestination();
    }
}