using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Configuración del Péndulo")]
    [SerializeField] private float maxAngle = 75f;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Header("Configuración del Empuje Lateral")]
    [Tooltip("Fuerza horizontal hacia los lados")]
    [SerializeField] private float fuerzaEmpujeLateral = 15f;

    [Tooltip("Fuerza hacia arriba")]
    [SerializeField] private float fuerzaArriba = 2f;

    [SerializeField] private float cooldownGolpe = 0.5f;

    private Quaternion initialRotation;
    private float ultimoGolpe = -1f;
    private float anguloPrevio = 0f;
    private float sentidoMovimiento = 1f;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * speed) * maxAngle;

        // 1 = moviéndose a la derecha, -1 = moviéndose a la izquierda
        sentidoMovimiento = (angle >= anguloPrevio) ? 1f : -1f;
        anguloPrevio = angle;

        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time > ultimoGolpe + cooldownGolpe)
        {
            PlayerMovement jugador = other.GetComponent<PlayerMovement>();

            if (jugador != null)
            {
                ultimoGolpe = Time.time;

                // Empuje lateral exacto en el eje horizontal (hacia donde viaja el péndulo)
                Vector3 direccionLateral = transform.right * sentidoMovimiento;

                // Vector final: lateral puro + pequeña elevación
                Vector3 fuerzaFinal = (direccionLateral * fuerzaEmpujeLateral) + (Vector3.up * fuerzaArriba);

                jugador.RecibirEmpuje(fuerzaFinal);
            }
        }
    }
}