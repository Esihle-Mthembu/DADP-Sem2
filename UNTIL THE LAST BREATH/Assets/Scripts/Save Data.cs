using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DoorSaveData
{
    public string id;
    public bool isUnlocked;
    public bool isOpen;
}

[Serializable]
public class SaveData
{
    public string sceneName;
    public Vector3 playerPosition;
    public float playerYaw; 
    public float cameraPitch; 
    public List<DoorSaveData> doors = new List<DoorSaveData>();
    public List<string> collectedItems = new List<string>();
    public List<InventorySaveEntry> inventory = new List<InventorySaveEntry>();
    public List<PickUpSaveData> pickUps = new List<PickUpSaveData>();
}

[Serializable]
public class InventorySaveEntry
{
    public string itemType;
    public int count;
}

[Serializable]
public class PickUpSaveData
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public bool wasHeld;
}