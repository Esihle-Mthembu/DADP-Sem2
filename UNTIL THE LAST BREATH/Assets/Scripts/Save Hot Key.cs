using UnityEngine;
using UnityEngine.InputSystem;

public class SaveHotkey : MonoBehaviour
{
    [SerializeField] Key saveKey = Key.K;

    void Start()
    {
        Debug.Log("SaveHotkey is running. Key = " + saveKey);
    }

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current[saveKey].wasPressedThisFrame)
        {
            if (SaveManager.Instance == null)
            {
                return;
            }

            SaveManager.Instance.Save();
        }
    }
}