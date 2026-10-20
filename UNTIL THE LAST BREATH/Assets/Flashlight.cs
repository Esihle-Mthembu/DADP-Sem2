using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Flashlight : MonoBehaviour
{
    [SerializeField] Light flashlightLight;
    [SerializeField] KeyCode toggleKey = KeyCode.L;

    void Awake()
    {
        // If the slot is empty, use the Light on this same object
        if (flashlightLight == null)
            flashlightLight = GetComponent<Light>();
    }

    void Start()
    {
        if (flashlightLight == null)
        {
            Debug.LogError("Flashlight: no Light assigned!", this);
            return;
        }
        flashlightLight.enabled = false;
    }

    void Update()
    {
        if (flashlightLight == null) return;

        if (TogglePressed())
        {
            Debug.Log("L pressed");
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }

    bool TogglePressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(toggleKey);
#endif
    }
}
