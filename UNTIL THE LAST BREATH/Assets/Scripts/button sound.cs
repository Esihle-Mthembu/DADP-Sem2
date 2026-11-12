using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip clickSound;
    public AudioClip hoverSound;     
    [Range(0f, 1f)] public float volume = 1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Play(clickSound));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable) Play(hoverSound);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
}