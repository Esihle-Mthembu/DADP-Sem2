using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class PuzzleTileClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PuzzleGridController grid;
    [SerializeField] private int slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        grid.OnTileClicked(slotIndex);
    }
}