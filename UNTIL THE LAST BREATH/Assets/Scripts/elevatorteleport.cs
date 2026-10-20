using System.Collections;
using UnityEngine;

public class ElevatorTeleport : MonoBehaviour
{
    public FPController playerController;
    public Transform destination;
    public float travelTime = 3f;

    private bool busy;

    public void GoToDestination()
   
    {
        busy = true;

        CharacterController cc = playerController.GetComponent<CharacterController>();
        cc.enabled = false;
        playerController.transform.SetPositionAndRotation(destination.position, destination.rotation);
        cc.enabled = true;

        playerController.enabled = true;
        busy = false;
    }
}