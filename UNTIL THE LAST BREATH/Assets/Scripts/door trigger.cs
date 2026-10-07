using UnityEngine;
[RequireComponent(typeof(Animator))]
public class DoorProximity : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find the object tagged 'Player'.")]
    [SerializeField] private Transform player;
    private Animator animator;

    [Header("Detection")]
    [Tooltip("If true, uses a 3D trigger Collider (OnTriggerEnter/Exit) instead of distance checks.")]
    [SerializeField] private bool useTriggerCollider = false;
    [Tooltip("Only used if useTriggerCollider is false.")]
    [SerializeField] private float detectionRadius = 3f;

    [Header("Animator")]
    [Tooltip("Name of the bool parameter in your Animator Controller that drives open/close.")]
    [SerializeField] private string isOpenParam = "IsOpen";

    [Header("Optional")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool isOpen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (useTriggerCollider || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius && !isOpen)
        {
            OpenDoor();
        }
        else if (distance > detectionRadius && isOpen)
        {
            CloseDoor();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!useTriggerCollider) return;
        if (other.CompareTag("Player") && !isOpen)
            OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!useTriggerCollider) return;
        if (other.CompareTag("Player") && isOpen)
            CloseDoor();
    }
    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool(isOpenParam, true);
        PlaySound(openSound);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool(isOpenParam, false);
        PlaySound(closeSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
    private void OnDrawGizmosSelected()
    {
        if (!useTriggerCollider)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
