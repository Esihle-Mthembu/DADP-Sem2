using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using InventorySystem;

public class WinCondition : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private FPController player;

    [Header("Instruction popup")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField, TextArea]
    private string instructionMessage = "Good job!You have all the files.\nYour final goal: find the medication in the last locked room.";
    [SerializeField] private float instructionDuration = 7f;

    [Header("Win screen")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button restartButton;

    private bool _instructionShown;
    private bool _won;

    public void Start()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(Restart);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemChanged += HandleItemChanged;
            InventoryManager.Instance.OnItemPickedUp += HandleItemPickedUp;
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemChanged -= HandleItemChanged;
            InventoryManager.Instance.OnItemPickedUp -= HandleItemPickedUp;
        }
    }

    private void HandleItemChanged(ItemType type, int current, int max)
    {
        if (_instructionShown || _won)
        {
            return;
        }

        var inv = InventoryManager.Instance;
        bool hasAllFiles = inv.HasItem(ItemType.DiagnosisFile) && inv.HasItem(ItemType.TreatmentFile) && inv.HasItem(ItemType.TestResultsFile);

        if (hasAllFiles)
        {
            _instructionShown = true;
            StartCoroutine(ShowInstruction());
        }
    }

    private void HandleItemPickedUp(ItemType type, int amount)
    {
        if (type == ItemType.Medication && !_won)
        {
            Win();
        }
    }

    private IEnumerator ShowInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text = instructionMessage;
        }

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(instructionDuration);

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    private void Win()
    {
        _won = true;

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (player != null)
        {
            player.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void Restart()
    {
        Time.timeScale = 1f;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetAll();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
