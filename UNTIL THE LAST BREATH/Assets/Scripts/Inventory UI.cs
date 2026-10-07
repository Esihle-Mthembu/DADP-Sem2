using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Button closeButton;
        [SerializeField] private InventorySlotUI[] slots;

        [SerializeField] private bool pauseGameWhileOpen = false;
        [SerializeField] private bool lockCursorWhileClosed = true;
        [SerializeField] private Key shortcutKey = Key.Tab;

        private bool _isOpen;

        private void OnEnable()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnItemChanged += HandleItemChanged;
            }
        }

        private void OnDisable()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnItemChanged -= HandleItemChanged;
            }
        }

        private void Start()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnItemChanged -= HandleItemChanged;
                InventoryManager.Instance.OnItemChanged += HandleItemChanged;

                RefreshAllSlots();
            }
        }

        private void Update()
        {
            if (Keyboard.current[shortcutKey].wasPressedThisFrame)
            {
                Toggle();
            }

            if (_isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Close();
            }
        }

        public void Toggle() => SetOpen(!_isOpen);
        public void Open() => SetOpen(true);
        public void Close() => SetOpen(false);

        private void SetOpen(bool open, bool instant = false)
        {
            _isOpen = open;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(open);
            }

            if (open)
            {
                RefreshAllSlots();
            }

            if (pauseGameWhileOpen)
            {
                Time.timeScale = open ? 0f : 1f;
            }

            if (lockCursorWhileClosed)
            {
                Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = open;
            }
        }

        private void HandleItemChanged(ItemType type, int current, int max)
        {
            foreach (var slot in slots)
            {
                if (slot != null && slot.ItemType == type)
                {
                    slot.UpdateDisplay(current, max);
                }
            }
        }

        private void RefreshAllSlots()
        {
            if (InventoryManager.Instance == null) return;

            foreach (var slot in slots)
            {
                if (slot == null)
                {
                    continue;
                }

                int current = InventoryManager.Instance.GetCount(slot.ItemType);
                int max = InventoryManager.Instance.GetMax(slot.ItemType);
                slot.UpdateDisplay(current, max);
            }
        }
    }
}
