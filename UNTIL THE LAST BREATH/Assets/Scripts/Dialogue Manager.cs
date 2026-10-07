using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public DialogueData introDialogue;

    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Image backgroundImage;
    public Image backgroundImageBack;

    public float backgroundFadeDuration = 0.3f;

    public InputAction advanceDialogue;
    public InputAction previousDialogue;

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool dialogueActive = false;
    private bool isTyping = false;

    private Coroutine typingCoroutine;
    private Coroutine backgroundCoroutine;

    [Header("Scene Transition")]
    public string nextSceneName = "Gameplay Scene";

    private void OnEnable()
    {
        advanceDialogue.Enable();
        advanceDialogue.performed += OnNextDialogue;

        previousDialogue.Enable();
        previousDialogue.performed += OnPreviousDialogue;
    }

    private void OnDisable()
    {
        advanceDialogue.performed -= OnNextDialogue;
        advanceDialogue.Disable();

        previousDialogue.performed -= OnPreviousDialogue;
        previousDialogue.Disable();
    }

    private void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        currentLine = 0;
        dialogueActive = true;

        dialoguePanel.SetActive(true);

        ShowLine();
    }

    private void ShowLine()
    {
        DialogueData.DialogueLine line =
            introDialogue.dialogueLines[currentLine];

        speakerNameText.text = line.speakerName;
        SetBackground(line.backgroundImage);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line.dialogueText));
    }

    private void SetBackground(Sprite sprite)
    {
        if (backgroundImage == null || sprite == null)
        {
            return;
        }

        if (backgroundImage.sprite == sprite)
        {
            return;
        }

        if (backgroundCoroutine != null)
        {
            StopCoroutine(backgroundCoroutine);
        }

        if (backgroundFadeDuration <= 0f)
        {
            backgroundImage.sprite = sprite;
        }
        else
        {
            backgroundCoroutine = StartCoroutine(CrossfadeBackground(sprite));
        }
    }

    private IEnumerator CrossfadeBackground(Sprite newSprite)
    {
        backgroundImageBack.sprite = newSprite;
        Color backColor = backgroundImageBack.color;
        backColor.a = 1f;
        backgroundImageBack.color = backColor;

        float t = 0f;
        Color frontColor = backgroundImage.color;

        while (t < backgroundFadeDuration)
        {
            t += Time.deltaTime;
            frontColor.a = Mathf.Lerp(1f, 0f, t / backgroundFadeDuration);
            backgroundImage.color = frontColor;
            yield return null;
        }

        backgroundImage.sprite = newSprite;
        frontColor.a = 1f;
        backgroundImage.color = frontColor;
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void OnNextDialogue(InputAction.CallbackContext context)
    {
        if (!dialogueActive)
            return;

        Debug.Log("Dialogue input detected!");

        if (isTyping)
        {
            FinishCurrentLine();
        }
        else
        {
            NextLine();
        }
    }

    private void OnPreviousDialogue(InputAction.CallbackContext context)
    {
        if (!dialogueActive)
            return;

        Debug.Log("Previous dialogue input detected");

        PreviousLine();
    }

    private void PreviousLine()
    {
        if (currentLine <= 0)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentLine--;
        ShowLine();
    }

    private void FinishCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text =
            introDialogue.dialogueLines[currentLine].dialogueText;

        isTyping = false;
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine < introDialogue.dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueActive = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialoguePanel.SetActive(false);

        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeAndLoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("ScreenFader not found in scene, loading directly.");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}