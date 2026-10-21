using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public Light flashlightLight;   // the Light component, set in the Inspector

    void Start()
    {
        flashlightLight.enabled = false;   // flashlight starts off
    }

    void Update()
    {
        // every frame, check if the L key was pressed this frame
        if (Keyboard.current.lKey.wasPressedThisFrame)
        { 
            Debug.Log("L pressed");
            flashlightLight.enabled = !flashlightLight.enabled;   // flip on/off
        }
    }
}