using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(Collider))]
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private int amount = 1;
        [SerializeField] private string playerTag = "Player";

        private bool _collected;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_collected)
            {
                return;
            }

            if (!other.CompareTag(playerTag))
            {
                return;
            }

            TryCollect();
        }

        private void TryCollect()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("[Colectible] No InventoryManager found in scene");
                return;
            }

            int added = InventoryManager.Instance.TryAddItem(itemType, amount);

            if (added <= 0)
            {
                return;
            }

            _collected = true;

            Destroy(gameObject);
        }
    }
}
