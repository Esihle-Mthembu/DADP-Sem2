using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private Transform originalParent;
    private int originalLayer;
    private bool isHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalLayer = gameObject.layer;
    }

    public void PickUpObject(Transform holdPos, Collider playerCollider)
    {
        isHeld = true;
        originalParent = transform.parent;

        rb.isKinematic = true;
        transform.parent = holdPos;

        int holdLayer = LayerMask.NameToLayer("Hold");
        if (holdLayer != -1) gameObject.layer = holdLayer;

        if (playerCollider != null && col != null)
            Physics.IgnoreCollision(col, playerCollider, true);
    }

    public void MoveToHoldPoint(Vector3 targetPos)
    {
        if (!isHeld) return;
        transform.position = targetPos;
    }

    public void Drop(Collider playerCollider)
    {
        Release(playerCollider);
    }

    public void Throw(Vector3 impulse, Collider playerCollider)
    {
        Release(playerCollider);
        rb.AddForce(impulse, ForceMode.VelocityChange);
    }

    private void Release(Collider playerCollider)
    {
        isHeld = false;
        transform.parent = originalParent;
        gameObject.layer = originalLayer;
        rb.isKinematic = false;

        if (playerCollider != null && col != null)
            Physics.IgnoreCollision(col, playerCollider, false);
    }
}