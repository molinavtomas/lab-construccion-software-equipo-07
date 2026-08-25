using UnityEngine;

public class MovimientoEsferaX : MonoBehaviour
{
    [Header("Configuración del Movimiento")]
    [Tooltip("Distancia máxima que recorrerá hacia cada lado desde el punto inicial")]
    [SerializeField] private float distancia = 2.5f;

    [Tooltip("Velocidad de desplazamiento")]
    [SerializeField] private float velocidad = 2f;

    [Header("Configuración del Empuje")]
    [Tooltip("Fuerza horizontal del impacto")]
    [SerializeField] private float fuerzaEmpuje = 22f;

    [Tooltip("Fuerza hacia arriba para levantarlo")]
    [SerializeField] private float fuerzaArriba = 4f;

    [SerializeField] private float cooldownGolpe = 0.5f;

    private Vector3 posicionInicial;
    private float ultimoGolpe = -1f;
    private float posXPrevia;
    private float sentidoMovimiento = 1f;

    private void Start()
    {
        posicionInicial = transform.position;
        posXPrevia = transform.position.x;
    }

    private void Update()
    {
        // 1. Movimiento de ida y vuelta
        float desplazamiento = Mathf.PingPong(Time.time * velocidad, distancia * 2f) - distancia;
        float nuevaPosX = posicionInicial.x + desplazamiento;

        // 2. Determinar si viaja hacia la derecha (+1) o izquierda (-1)
        sentidoMovimiento = (nuevaPosX >= posXPrevia) ? 1f : -1f;
        posXPrevia = nuevaPosX;

        transform.position = new Vector3(nuevaPosX, posicionInicial.y, posicionInicial.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 3. Validar si impacta al jugador respetando el cooldown
        if (other.CompareTag("Player") && Time.time > ultimoGolpe + cooldownGolpe)
        {
            PlayerMovement jugador = other.GetComponent<PlayerMovement>();

            if (jugador != null)
            {
                ultimoGolpe = Time.time;

                // 4. Dirección del empuje sobre el eje X del mundo hacia donde se mueve la esfera
                Vector3 direccionEmpuje = Vector3.right * sentidoMovimiento;

                // 5. Componer el vector de fuerza final
                Vector3 fuerzaFinal = (direccionEmpuje * fuerzaEmpuje) + (Vector3.up * fuerzaArriba);

                // 6. Aplicar empuje y pausar el control por 0.35 segundos
                jugador.RecibirEmpuje(fuerzaFinal, 0.35f);

                Debug.Log($"Esfera impactó al jugador con fuerza: {fuerzaFinal}");
            }
        }
    }
}