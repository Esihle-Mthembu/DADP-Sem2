using UnityEngine;
using UnityEngine.InputSystem;

public class FPController : MonoBehaviour
{
    [Header("Movement settings")]
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Look settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 1.5f;
    public float mouseDeltaScale = 0.1f;
    public float verticalLookLimit = 100f;

    [Header("Sprint Settings")]
    public float sprintSpeed = 20f;
    private bool isSprinting;

    [Header("Crouch settings")]
    public float originalSpeed;
    public float crouchSpeed = 3f;
    public float originalHeight = 3f;
    public float crouchHeight = 1.5f;

    [Header("PickUp settings")]
    public float pickupRange = 20f;
    public Transform holdPos;
    public LayerMask pickupMask = ~0;
    private PickUp heldObject;

    [Header("Throw Settings")]
    public float throwForce = 15f;
    public float throwUpwardBoost = 4f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        originalSpeed = moveSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();

        if (heldObject != null)
        {
            heldObject.MoveToHoldPoint(holdPos.position);
        }
    }

    // Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (controller.height == crouchHeight)
        {
            currentSpeed = crouchSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Look
    public void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity * mouseDeltaScale;
        float mouseY = lookInput.y * lookSensitivity * mouseDeltaScale;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Sprint (Shift)
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) isSprinting = true;
        else if (context.canceled) isSprinting = false;
    }

    // Jump (Spacebar)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    // Crouch (C)
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            controller.height = crouchHeight;
            moveSpeed = crouchSpeed;
        }
        else if (context.canceled)
        {
            controller.height = originalHeight;
            moveSpeed = originalSpeed;
        }
    }

    // Pick Up (E)
    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (heldObject == null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
            {
                if (hit.collider.CompareTag("Pickup"))
                {
                    PickUp pickUp = hit.collider.GetComponent<PickUp>();
                    if (pickUp != null)
                    {
                        pickUp.PickUpObject(holdPos, controller);
                        heldObject = pickUp;
                    }
                    else
                    {
                        Debug.Log("Object has PickUp tag but no component");
                    }
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing");
            }
        }
        else
        {
            heldObject.Drop(controller);
            heldObject = null;
        }
    }

    // Throw (T)
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (heldObject == null) return;

        Vector3 dir = cameraTransform.forward;
        Vector3 impulse = dir * throwForce + Vector3.up * throwUpwardBoost;

        heldObject.Throw(impulse, controller);
        heldObject = null;
    }
}