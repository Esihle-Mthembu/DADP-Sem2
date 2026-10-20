using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] string fallbackSceneName = "Gameplay Scene";
    [SerializeField] string playerTag = "Player";

    public SaveData Current = new SaveData();
    SaveData pendingLoad;

    public event System.Action OnSaved;

    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public bool HasSave() => File.Exists(SavePath);

    public void ResetSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        Current = new SaveData();
        pendingLoad = null;
    }

    public void Continue()
    {
        if (!HasSave())
        {
            return;
        }

        Current = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        pendingLoad = Current;
        SceneManager.LoadScene(string.IsNullOrEmpty(Current.sceneName) ? fallbackSceneName : Current.sceneName);
    }

    //Saving
    public void Save()
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            return;
        }

        Current.sceneName = SceneManager.GetActiveScene().name;
        Current.playerPosition = player.transform.position;
        Current.playerYaw = player.transform.eulerAngles.y;

        var cam = Camera.main;

        if (cam != null)
        {
            Current.cameraPitch = cam.transform.localEulerAngles.x;
        }

        Current.doors.Clear();
        foreach (var door in FindObjectsByType<DoorInteractable>(FindObjectsSortMode.None))
        {
            Current.doors.Add(new DoorSaveData
            {
                id = door.doorID,
                isUnlocked = door.isUnlocked,
                isOpen = door.IsOpen
            });
        }

        string tmp = SavePath + ".tmp";
        File.WriteAllText(tmp, JsonUtility.ToJson(Current, true));

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.Move(tmp, SavePath);
        Debug.Log("Saved to " + SavePath);
        OnSaved?.Invoke();
    }

    //Loading scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingLoad == null || scene.name != pendingLoad.sceneName)
        {
            return;
        }

        StartCoroutine(ApplyNextFrame(pendingLoad));
        pendingLoad = null;
    }

    IEnumerator ApplyNextFrame(SaveData data)
    {
        yield return null;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            var cc = player.GetComponent<CharacterController>();
            if (cc)
            {
                cc.enabled = false;
            }

            player.transform.SetPositionAndRotation(
                data.playerPosition, Quaternion.Euler(0, data.playerYaw, 0));

            if (cc)
            {
                cc.enabled = true;
            }
        }

        foreach (var door in FindObjectsByType<DoorInteractable>(FindObjectsSortMode.None))
        {
            var saved = data.doors.Find(d => d.id == door.doorID);
            if (saved != null)
            {
                door.ApplySavedState(saved.isUnlocked, saved.isOpen);
            }
        } 
    }
}