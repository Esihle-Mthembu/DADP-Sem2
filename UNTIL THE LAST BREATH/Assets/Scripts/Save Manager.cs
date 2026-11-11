using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using InventorySystem;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public static bool IsLoadingSave => Instance != null && Instance.isApplyingLoad;

    [SerializeField] string fallbackSceneName = "Gameplay Scene";
    [SerializeField] string playerTag = "Player";
    [SerializeField] float holdPositionSeconds = 0.4f;

    public SaveData Current = new SaveData();
    SaveData pendingLoad;
    string pendingSceneName;
    bool isApplyingLoad;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            new GameObject("SaveManager").AddComponent<SaveManager>();
        }
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

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetAll();
        }

        Current = new SaveData();
        pendingLoad = null;
        pendingSceneName = null;
        isApplyingLoad = false;
    }

    public void Continue()
    {
        if (!HasSave())
        {
            return;
        }

        Current = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        pendingLoad = Current;
        pendingSceneName = string.IsNullOrEmpty(Current.sceneName) ? fallbackSceneName : Current.sceneName;
        SceneManager.LoadScene(pendingSceneName);
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

        var fp = player.GetComponent<FPController>();

        if (fp != null)
        {
            Current.cameraPitch = fp.Pitch;
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

        Current.inventory.Clear();
        if (InventoryManager.Instance != null)
        {
            foreach (var type in InventoryManager.Instance.AllTypes)
            {
                int count = InventoryManager.Instance.GetCount(type);

                if (count > 0)
                {
                    Current.inventory.Add(new InventorySaveEntry { itemType = type.ToString(), count = count });
                }
            }
        }

        Current.pickUps.Clear();
        foreach (var p in FindObjectsByType<PickUp>(FindObjectsSortMode.None))
        {
            if (string.IsNullOrEmpty(p.pickupID))
            {
                continue;
            }

            Current.pickUps.Add(new PickUpSaveData
            {
                id = p.pickupID,
                position = p.transform.position,
                rotation = p.transform.rotation,
                wasHeld = p.IsHeld
            });
        }

        string tmp = SavePath + ".tmp";
        File.WriteAllText(tmp, JsonUtility.ToJson(Current, true));

        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        File.Move(tmp, SavePath);
        Debug.Log("Saved to " + SavePath + " | player pos: " + Current.playerPosition);
        OnSaved?.Invoke();
    }

    //Loading scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingLoad == null || scene.name != pendingSceneName)
        {
            return;
        }

        isApplyingLoad = true;

        StartCoroutine(ApplyNextFrame(pendingLoad));
        pendingLoad = null;
        pendingSceneName = null;
    }

    void PlacePlayer(SaveData data)
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc)
        {
            cc.enabled = false;
        }

        player.transform.SetPositionAndRotation( data.playerPosition, Quaternion.Euler(0, data.playerYaw, 0));

        if (cc)
        {
            cc.enabled = true;
        }

        var fp = player.GetComponent<FPController>();
        if (fp != null)
        {
            fp.SetPitch(data.cameraPitch);
        }
    }

    IEnumerator ApplyNextFrame(SaveData data)
    {
        yield return null;

        PlacePlayer(data);

        var player = GameObject.FindGameObjectWithTag(playerTag);

        foreach (var door in FindObjectsByType<DoorInteractable>(FindObjectsSortMode.None))
        {
            var saved = data.doors.Find(d => d.id == door.doorID);
            if (saved != null)
            {
                door.ApplySavedState(saved.isUnlocked, saved.isOpen);
            }
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetAll();
            foreach (var entry in data.inventory)
            {
                if (System.Enum.TryParse(entry.itemType, out ItemType type))
                {
                    InventoryManager.Instance.SetCount(type, entry.count);
                }
            }
        }

        var playerFP = player != null ? player.GetComponent<FPController>() : null;

        foreach (var p in FindObjectsByType<PickUp>(FindObjectsSortMode.None))
        {
            var saved = data.pickUps.Find(s => s.id == p.pickupID);

            if (saved == null)
            {
                continue;
            }

            p.transform.SetPositionAndRotation(saved.position, saved.rotation);

            if (saved.wasHeld && playerFP != null)
            {
                playerFP.RestoreHeld(p);
            }
        }

        float t = 0f;
        while (t < holdPositionSeconds)
        {
            yield return null;
            PlacePlayer(data);
            t += Time.unscaledDeltaTime;
        }

        isApplyingLoad = false;
        Debug.Log("Load applied, player placed at " + data.playerPosition);
    }
}