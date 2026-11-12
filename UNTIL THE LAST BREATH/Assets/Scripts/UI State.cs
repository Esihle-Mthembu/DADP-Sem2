using UnityEngine;

public static class UIState
{
    static int blockers;
    public static bool IsBlocked => blockers > 0;

    public static void Push()
    {
        blockers++;
        Apply();
    }

    public static void Pop()
    {
        blockers = Mathf.Max(0, blockers - 1);
        Apply();
    }

    public static void Reset()
    {
        blockers = 0;
        Apply();
    }

    static void Apply()
    {
        Cursor.lockState = blockers > 0 ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blockers > 0;
    }
}