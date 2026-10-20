using UnityEngine;

public class SavedPickup : MonoBehaviour
{
    public string pickupID; //Unique per pickup object

    void Start()
    {
        var sm = SaveManager.Instance;
        if (sm != null && sm.Current.collectedItems.Contains(pickupID))
        {
            Destroy(gameObject);
        }
    }

    public void MarkCollected()
    {
        var sm = SaveManager.Instance;
        if (sm != null && !sm.Current.collectedItems.Contains(pickupID))
        {
            sm.Current.collectedItems.Add(pickupID);
        }
    }
}
