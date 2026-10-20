using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] float distance = 5f;

    ParticleSystem[] systems;

    void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        bool locked = Cursor.lockState == CursorLockMode.Locked;
        foreach (var ps in systems)
        {
            var em = ps.emission;
            em.enabled = !locked; //No trail during gameplay
        }
        if (locked)
        {
            return;
        }

        Camera cam = Camera.main;  //Always use the camera of the current scene
        if (cam == null)
        {
            return;
        }

        Vector3 p = Mouse.current.position.ReadValue();
        p.z = distance;
        transform.position = cam.ScreenToWorldPoint(p);
    }
}