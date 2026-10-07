using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private bool showCountEvenIfMaxIsOne = false;

        public ItemType ItemType => itemType;

        public void UpdateDisplay(int current, int max)
        {
            bool owned = current > 0;

            if (iconImage != null)
            {
                iconImage.color = owned ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            }

            if (countText != null)
            {
                bool shouldShowCount = max > 1 || showCountEvenIfMaxIsOne;
                countText.gameObject.SetActive(shouldShowCount);
                countText.text = $"{current}/{max}";
            }
        }
    }
}
