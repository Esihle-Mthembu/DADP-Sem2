using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Serializable]
        public class ItemLimit
        {
            public ItemType type;
            public int maxCount;
        }

        [Tooltip("Set the maximum amount the player can carry of each item type.")]
        [SerializeField]
        private List<ItemLimit> itemLimits = new List<ItemLimit>
        {
            new ItemLimit { type = ItemType.File,         maxCount = 1 },
            new ItemLimit { type = ItemType.Key,          maxCount = 3 },
            new ItemLimit { type = ItemType.Torch,        maxCount = 1 },
            new ItemLimit { type = ItemType.TorchBattery, maxCount = 3 },
        };

        private readonly Dictionary<ItemType, int> _maxCounts = new Dictionary<ItemType, int>();
        private readonly Dictionary<ItemType, int> _currentCounts = new Dictionary<ItemType, int>();

        public event Action<ItemType, int, int> OnItemChanged;
        public event Action<ItemType, int> OnItemPickedUp;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var limit in itemLimits)
            {
                _maxCounts[limit.type] = limit.maxCount;
                _currentCounts[limit.type] = 0;
            }
        }

        public int TryAddItem(ItemType type, int amount = 1)
        {
            if (!_maxCounts.ContainsKey(type))
            {
                Debug.LogWarning($"[InventoryManager] No limit configured for {type}. Add it in the Inspector.");
                return 0;
            }

            int current = _currentCounts[type];
            int max = _maxCounts[type];
            int space = max - current;

            if (space <= 0)
                return 0;

            int amountAdded = Mathf.Min(space, amount);
            _currentCounts[type] = current + amountAdded;

            OnItemChanged?.Invoke(type, _currentCounts[type], max);
            if (amountAdded > 0)
            {
                OnItemPickedUp?.Invoke(type, amountAdded);
            }

            return amountAdded;
        }

        public int RemoveItem(ItemType type, int amount = 1)
        {
            if (!_currentCounts.ContainsKey(type))
            {
                return 0;
            }

            int current = _currentCounts[type];
            int amountRemoved = Mathf.Min(current, amount);
            _currentCounts[type] = current - amountRemoved;

            OnItemChanged?.Invoke(type, _currentCounts[type], _maxCounts[type]);
            return amountRemoved;
        }

        public int GetCount(ItemType type) => _currentCounts.TryGetValue(type, out var c) ? c : 0;
        public int GetMax(ItemType type) => _maxCounts.TryGetValue(type, out var m) ? m : 0;
        public bool HasItem(ItemType type, int amount = 1) => GetCount(type) >= amount;
        public bool IsFull(ItemType type) => GetCount(type) >= GetMax(type);

        public IEnumerable<ItemType> AllTypes => _maxCounts.Keys;
    }
}

