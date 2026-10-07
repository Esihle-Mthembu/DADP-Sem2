using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlightObject;
    private bool isOn = false;

    void Start()
    {
        flashlightObject.SetActive(isOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlightObject.SetActive(isOn);
        }
    }
}



