using UnityEngine;

public class CursorSceneManager : MonoBehaviour
{
    static CursorSceneManager instance;

    void Awake()
    {
        if (instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
