using UnityEngine;

public class GhostProximity : MonoBehaviour
{
    public static GhostProximity Instance { get; private set; }

    [SerializeField] Transform player;
    [SerializeField] float farDistance = 12f; //Start effect
    [SerializeField] float nearDistance = 5f; //Full effect level at this point
    [SerializeField] float smoothing = 3f;      

    public float Intensity { get; private set; }
    public Transform Player => player;

    void Awake() { Instance = this; }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float d = Vector3.Distance(transform.position, player.position);
        float target = Mathf.InverseLerp(farDistance, nearDistance, d);
        Intensity = Mathf.Lerp(Intensity, target, smoothing * Time.deltaTime);
    }
}