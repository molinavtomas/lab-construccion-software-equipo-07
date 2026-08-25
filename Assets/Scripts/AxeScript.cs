using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Configuración del Hacha")]
    [Tooltip("Ángulo máximo de inclinación hacia cada lado")]
    [SerializeField] private float maxAngle = 75f;

    [Tooltip("Velocidad de oscilación")]
    [SerializeField] private float speed = 2.5f;

    [Tooltip("Eje sobre el cual oscilará (X o Z habitualmente)")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Magnitud del empuje")]
    [SerializeField] private float fuerzaEmpuje = 18f;

    [Tooltip("Fuerza vertical adicional para levantar al jugador")]
    [SerializeField] private float impulsoVertical = 4f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // Movimiento armónico de vaivén (Péndulo sinusoidal)
        float angle = Mathf.Sin(Time.time * speed) * maxAngle;
        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. Validar que el objeto impactado sea el jugador
        if (other.CompareTag("Player"))
        {
            // 2. Obtener el Rigidbody del jugador
            Rigidbody rbJugador = other.GetComponent<Rigidbody>();

            if (rbJugador != null)
            {
                // 3. Calcular la dirección desde el hacha hacia el jugador
                Vector3 direccionEmpuje = (other.transform.position - transform.position).normalized;

                // 4. Agregar elevación vertical para romper la fricción con el piso
                direccionEmpuje.y = 0f; // Anular inclinaciones raras
                direccionEmpuje = direccionEmpuje.normalized * fuerzaEmpuje + Vector3.up * impulsoVertical;

                // 5. Opcional: Reiniciar la velocidad previa para que el empuje sea consistente
                rbJugador.linearVelocity = Vector3.zero; // En versiones previas a Unity 6 usa: rbJugador.velocity = Vector3.zero;

                // 6. Aplicar la fuerza instantánea
                rbJugador.AddForce(direccionEmpuje, ForceMode.Impulse);

                Debug.Log("Jugador empujado con fuerza: " + direccionEmpuje);
            }
        }
    }
}
