using UnityEngine;

public class GhostWarning : MonoBehaviour
{
    [SerializeField] private Transform ghost;
    [SerializeField] private Transform player;
    [SerializeField] private CanvasGroup warningGroup;

    [SerializeField] private float showDistance = 15f;
    [SerializeField] private float hideDistance = 20f;
    [SerializeField] private float fadeSpeed = 3f;

    private bool ghostNear;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        warningGroup.alpha = 0f;
    }

    void Update()
    {
        if (ghost == null || player == null)
        {
            return;
        }

        float dist = Vector3.Distance(ghost.position, player.position);

        if (!ghostNear && dist < showDistance)
        {
            ghostNear = true;
        }
        else if (ghostNear && dist > hideDistance)
        {
            ghostNear = false;
        }

        float target = ghostNear ? 1f : 0f;
        warningGroup.alpha = Mathf.MoveTowards(warningGroup.alpha, target, fadeSpeed * Time.deltaTime);
    }
}