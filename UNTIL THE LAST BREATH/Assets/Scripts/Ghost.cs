using UnityEngine;

public class Ghost : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 220f;   // how far away it starts chasing

    [Header("Movement")]
    public float chaseSpeed = 2f;
    public float turnSpeed = 5f;
    public float stopDistance = 5f;

    [Header("Floating ghost")]
    public float ghostHeight = 0.2f;
    public float ghostSpeed = 1.5f;

    private float baseY;

    void Start()
    {
        baseY = transform.position.y;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Ghost: No player assigned and no GameObject tagged 'Player' found.");
            }
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Hover();
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        // Horizontal direction toward the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        if (distanceToPlayer > stopDistance)
        {
            Vector3 newPos = transform.position + direction * chaseSpeed * Time.deltaTime;
            newPos.y = baseY + Mathf.Sin(Time.time * ghostSpeed) * ghostHeight;
            transform.position = newPos;
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y = baseY + Mathf.Sin(Time.time * ghostSpeed) * ghostHeight;
            transform.position = pos;
        }

        // Face the player
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void Hover()
    {
        // Not chasing yet
        Vector3 pos = transform.position;
        pos.y = baseY + Mathf.Sin(Time.time * ghostSpeed) * ghostHeight;
        transform.position = pos;
    }
}