using System.Collections.Generic;
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
    private float speedMultiplier = 1f;

    private readonly List<Vector3> wallContactNormals = new List<Vector3>();

    private const float MaxWallNormalUpDot = 0.5f;
    private const float SameWallNormalDot = 0.99f;

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

        // Los callbacks de colision vuelven a llenar la lista despues del
        // siguiente paso de fisica. De esta forma nunca usamos contactos viejos.
        wallContactNormals.Clear();
    }

    private void MovePlayer()
    {
        Vector3 moveDirection =
            orientation.right * input.x +
            orientation.forward * input.y;

        moveDirection.y = 0f;

        // Velocidad normal o de correr
        float currentSpeed = (running ? runSpeed : speed) * speedMultiplier;

        Vector3 targetVelocity = ProjectVelocityAlongWalls(
            moveDirection * currentSpeed,
            wallContactNormals
        );

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

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.1f, 5f);
    }

    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }

    public static Vector3 ProjectVelocityAlongWalls(
        Vector3 velocity,
        IList<Vector3> wallNormals
    )
    {
        Vector3 projectedVelocity = velocity;

        // Repetir la proyeccion permite resolver correctamente el contacto con
        // mas de una pared (por ejemplo, una esquina) sin atravesar ninguna.
        for (int pass = 0; pass < wallNormals.Count; pass++)
        {
            bool velocityChanged = false;

            for (int i = 0; i < wallNormals.Count; i++)
            {
                Vector3 horizontalNormal = Vector3.ProjectOnPlane(
                    wallNormals[i],
                    Vector3.up
                );

                if (horizontalNormal.sqrMagnitude <= Mathf.Epsilon)
                    continue;

                horizontalNormal.Normalize();

                float speedIntoWall = Vector3.Dot(
                    projectedVelocity,
                    horizontalNormal
                );

                if (speedIntoWall >= 0f)
                    continue;

                projectedVelocity -= horizontalNormal * speedIntoWall;
                velocityChanged = true;
            }

            if (!velocityChanged)
                break;
        }

        return projectedVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        RegisterWallContacts(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        RegisterWallContacts(collision);
    }

    private void RegisterWallContacts(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal.normalized;

            // Ignorar suelo, pendientes transitables y techos. Solo las
            // superficies suficientemente verticales deben modificar el avance.
            if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > MaxWallNormalUpDot)
                continue;

            Vector3 horizontalNormal = Vector3.ProjectOnPlane(
                normal,
                Vector3.up
            ).normalized;

            bool alreadyRegistered = false;

            for (int j = 0; j < wallContactNormals.Count; j++)
            {
                if (Vector3.Dot(wallContactNormals[j], horizontalNormal) >
                    SameWallNormalDot)
                {
                    alreadyRegistered = true;
                    break;
                }
            }

            if (!alreadyRegistered)
                wallContactNormals.Add(horizontalNormal);
        }
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
