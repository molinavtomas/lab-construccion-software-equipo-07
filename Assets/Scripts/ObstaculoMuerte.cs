using UnityEngine;

public class ObstaculoMuerte : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadRotacion = 100f; // Ajustá este número para que gire más rápido o más lento

    [Header("Respawn")]
    public Transform puntoDeRespawn; // Acá vamos a conectar tu objeto "Respawn"

    void Update()
    {
        // Esto hace que la hélice gire constantemente. 
        // Si gira para un lado que no querés, cambiale el eje (ej: transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0); )
        transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si lo que chocó tiene la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Teletransportar al jugador
            collision.gameObject.transform.position = puntoDeRespawn.position;

            // 2. Frenar las fuerzas físicas (para que no salga volando al reaparecer)
            Rigidbody rbJugador = collision.gameObject.GetComponent<Rigidbody>();
            if (rbJugador != null)
            {
                rbJugador.linearVelocity = Vector3.zero;
                rbJugador.angularVelocity = Vector3.zero;
            }
        }
    }
}