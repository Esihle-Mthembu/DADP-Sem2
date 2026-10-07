using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Points")]
    [Tooltip("World position the elevator moves to at the bottom.")]
    public Transform bottomPoint;

    [Tooltip("World position the elevator moves to at the top.")]
    public Transform topPoint;

    [Header("Movement Settings")]
    [Tooltip("Speed the elevator moves, in units per second.")]
    public float moveSpeed = 2f;

    [Tooltip("Seconds the elevator waits at top/bottom before it can move again.")]
    public float waitTime = 1f;

    [Header("Activation")]
    [Tooltip("If true, the elevator moves automatically when the player enters its trigger. If false, call CallElevator() from another script (e.g. a button).")]
    public bool activateOnPlayerTouch = true;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool waiting = false;
    private float waitTimer = 0f;
    private bool atTop = false;

    private void Start()
    {
        if (bottomPoint == null || topPoint == null)
        {
            Debug.LogWarning("Elevator: bottomPoint and topPoint must both be assigned.");
            enabled = false;
            return;
        }

        
        transform.position = bottomPoint.position;
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
            }
            return;
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                waiting = true;
                waitTimer = waitTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activateOnPlayerTouch) return;
        if (!other.CompareTag("Player")) return;

        CallElevator();
    }

    public void CallElevator()
    {
        if (isMoving || waiting) return;

        atTop = !atTop;
        targetPosition = atTop ? topPoint.position : bottomPoint.position;
        isMoving = true;
    }

    public void GoToTop()
    {
        if (isMoving) return;
        atTop = true;
        targetPosition = topPoint.position;
        isMoving = true;
        waiting = false;
    }

    public void GoToBottom()
    {
        if (isMoving) return;
        atTop = false;
        targetPosition = bottomPoint.position;
        isMoving = true;
        waiting = false;
    }
}