using UnityEngine;

public class Llegada : MonoBehaviour
{
    [Header("Conexión con el sistema")]
    public GameManager gameManager; // Arrastrar acá el objeto que tenga el GameManager

    private void OnTriggerEnter(Collider other)
    {
        // Si el jugador cruza la meta y el juego no había terminado...
        if (other.CompareTag("Player") && !gameManager.juegoTerminado)
        {
            gameManager.GanarJuego();
        }
    }
}