using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleGridController : MonoBehaviour
{
    [Header("Grid Setup")]
    [SerializeField] private Texture2D sourceImage;
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;
    [SerializeField] private List<Image> tileImages = new List<Image>();

    [SerializeField] private bool shuffleOnEnable = true;
    [SerializeField] private int shuffleSwaps = 40;

    public System.Action OnSolved;

    private Sprite[] correctSprites;
    private int[] slotContents;
    private int firstSelectedSlot = -1;
    private bool solved = false;

    private void Awake()
    {
        SliceSourceImage();
    }

    private void OnEnable()
    {
        solved = false;
        firstSelectedSlot = -1;
        if (shuffleOnEnable)
            ShuffleAndDisplay();
        else
            DisplayGrid();
    }

    private void SliceSourceImage()
    {
        int tileCount = columns * rows;
        correctSprites = new Sprite[tileCount];

        float tileWidth = sourceImage.width / (float)columns;
        float tileHeight = sourceImage.height / (float)rows;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                float x = col * tileWidth;
                float y = sourceImage.height - (row + 1) * tileHeight;

                Sprite sprite = Sprite.Create(
                    sourceImage,
                    new Rect(x, y, tileWidth, tileHeight),
                    new Vector2(0.5f, 0.5f)
                );
                correctSprites[index] = sprite;
            }
        }
    }

    private void ShuffleAndDisplay()
    {
        int tileCount = correctSprites.Length;
        slotContents = new int[tileCount];
        for (int i = 0; i < tileCount; i++)
            slotContents[i] = i;

        for (int s = 0; s < shuffleSwaps; s++)
        {
            int a = Random.Range(0, tileCount);
            int b = Random.Range(0, tileCount);
            (slotContents[a], slotContents[b]) = (slotContents[b], slotContents[a]);
        }

        if (IsSolvedState())
        {
            (slotContents[0], slotContents[1]) = (slotContents[1], slotContents[0]);
        }

        DisplayGrid();
    }

    private void DisplayGrid()
    {
        for (int slot = 0; slot < tileImages.Count; slot++)
        {
            tileImages[slot].sprite = correctSprites[slotContents[slot]];
        }
    }

    public void OnTileClicked(int slotIndex)
    {
        if (solved) return;

        if (firstSelectedSlot == -1)
        {
            firstSelectedSlot = slotIndex;
            SetHighlight(slotIndex, true);
            return;
        }

        // click the same tile twice to deselect
        if (firstSelectedSlot == slotIndex) 
        {
            SetHighlight(slotIndex, false);
            firstSelectedSlot = -1;
            return;
        }

        // swap two slots
        (slotContents[firstSelectedSlot], slotContents[slotIndex]) = (slotContents[slotIndex], slotContents[firstSelectedSlot]);

        SetHighlight(firstSelectedSlot, false);
        firstSelectedSlot = -1;

        DisplayGrid();
        CheckSolved();
    }

    //Visual feedback
    private void SetHighlight(int slot, bool on)
    {
        tileImages[slot].color = on ? new Color(1f, 1f, 0.6f) : Color.white;
    }

    private bool IsSolvedState()
    {
        for (int i = 0; i < slotContents.Length; i++)
        {
            if (slotContents[i] != i) return false;
        }
        return true;
    }

    private void CheckSolved()
    {
        if (!IsSolvedState()) return;

        solved = true;
        OnSolved?.Invoke();
    }

    public bool IsSolved() => solved;
}

