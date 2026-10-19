using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject FlashlightLight;
    private bool FlashlightActive = false;
    void Start()
    {
        FlashlightLight.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (FlashlightActive == false)
            {
                FlashlightLight.SetActive(true);
                FlashlightActive = true;
            }
            else
            {
                FlashlightLight.SetActive(false);
                FlashlightActive = false;
            }
        }
    }
}