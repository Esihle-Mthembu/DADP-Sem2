using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorOpen : MonoBehaviour
{
    public float openForce = 100f;
    public float range = 10f;

    public GameObject door;
    public bool isOpening = false;

    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        // New Input System equivalent of Input.GetKeyDown("f")
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Trigger trigger = hit.transform.GetComponent<Trigger>();
            if (trigger != null && !isOpening)
            {
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        door.GetComponent<Animator>().Play("DoorOpen");
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForSeconds(5.0f);
        door.GetComponent<Animator>().Play("New State");
        isOpening = false;
    }
}
