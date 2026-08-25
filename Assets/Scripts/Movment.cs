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
    public Grappling grappling;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

      void Update()
{
    if (Keyboard.current.spaceKey.wasPressedThisFrame)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}

    void FixedUpdate()
    {
        if (grappling != null && grappling.IsGrappling())
            return;
        Vector2 input = Vector2.zero;

        // WASD
        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        // Evita que diagonal sea más rápido
        input = Vector2.ClampMagnitude(input, 1f);

        // Movimiento según hacia dónde mira el jugador
        Vector3 movement =
            transform.right * input.x +
            transform.forward * input.y;

        // Velocidad normal
        movement *= speed;

        // SHIFT = CORRER
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            movement *= SprintSpeed;
        }

        // Aplicar movimiento sin modificar la velocidad vertical
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
  
}