using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float SprintSpeed = 2f;

    [Header("Salto")]
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Sistemas")]
    public Grappling grappling;
    public WallRunning wallRunning;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // SALTO NORMAL
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (IsGrounded() && !IsWallRunning())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        // Si está usando el grappling, no modificar el movimiento
        if (grappling != null && grappling.IsGrappling())
            return;

        // Si está haciendo wall run, el WallRunning controla el movimiento
        if (IsWallRunning())
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1;
        }

        // Evita velocidad extra en diagonal
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 movement =
            transform.right * input.x +
            transform.forward * input.y;

        movement *= speed;

        // SHIFT = correr
        if (Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed)
        {
            movement *= SprintSpeed;
        }

        // No tocar la velocidad vertical
        rb.linearVelocity = new Vector3(
            movement.x,
            rb.linearVelocity.y,
            movement.z
        );
    }

    bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            1.1f,
            groundMask
        );
    }

    bool IsWallRunning()
    {
        return wallRunning != null && wallRunning.wallrunning;
    }
}