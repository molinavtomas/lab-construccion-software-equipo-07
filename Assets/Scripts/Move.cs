using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Move : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float runSpeed = 10f;
    public float acceleration = 20f;

    [Header("Salto")]
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.25f;
    public LayerMask groundMask;

    [Header("Referencias")]
    public Transform orientation;

    [HideInInspector]
    public bool wallrunning;

    private Rigidbody rb;
    private Vector2 input;
    private bool jumpPressed;
    private bool running;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Movimiento
        input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        input = Vector2.ClampMagnitude(input, 1f);

        // Correr con Shift SOLO en el suelo
        running = Keyboard.current.leftShiftKey.isPressed && IsGrounded();

        // Salto
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (!wallrunning)
        {
            MovePlayer();
        }

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection =
            orientation.right * input.x +
            orientation.forward * input.y;

        moveDirection.y = 0f;

        // Velocidad normal o de correr
        float currentSpeed = running ? runSpeed : speed;

        Vector3 targetVelocity = moveDirection * currentSpeed;

        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 velocityChange =
            targetVelocity - horizontalVelocity;

        velocityChange = Vector3.ClampMagnitude(
            velocityChange,
            acceleration * Time.fixedDeltaTime
        );

        rb.AddForce(
            velocityChange,
            ForceMode.VelocityChange
        );
    }

    private void Jump()
    {
        if (!IsGrounded())
        {
            Debug.Log("NO ESTOY EN EL SUELO");
            return;
        }

        Debug.Log("SALTANDO");

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }

    public bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("Falta asignar Ground Check en Move");
            return false;
        }

        return Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }
}