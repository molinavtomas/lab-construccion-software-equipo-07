using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public float tiempoMaximo = 60f; // Tiempo límite en segundos para perder

    [Header("Estado del Juego")]
    public bool juegoTerminado = false;
    private float tiempoActual = 0f;

    void Update()
    {
        // Si el juego ya terminó, no seguimos contando el tiempo
        if (juegoTerminado) return;

        tiempoActual += Time.deltaTime;

        // Condición de derrota: se acabó el tiempo
        if (tiempoActual >= tiempoMaximo)
        {
            PerderJuego();
        }
    }

    public void GanarJuego()
    {
        juegoTerminado = true;
        Debug.Log($"¡Llegaste a la meta! Tiempo total: {tiempoActual:F2} segundos.");
        // Más adelante acá podés agregar la interfaz de victoria
    }

    public void PerderJuego()
    {
        juegoTerminado = true;
        Debug.Log("¡Derrota! Se agotó el tiempo límite.");
        // Más adelante acá podés reiniciar la escena
    }
}