using Unity.Netcode;
using UnityEngine;

// 1. Cambiamos MonoBehaviour por NetworkBehaviour
public class GameManager : NetworkBehaviour
{
    [Header("Configuración del Nivel")]
    public float tiempoMaximo = 120f;

    [Header("Estado del Juego (Sincronizado)")]
    // 2. Usamos NetworkVariable para que el tiempo sea igual en todas las pantallas
    public NetworkVariable<float> tiempoActual = new NetworkVariable<float>(0f);
    public NetworkVariable<bool> carreraActiva = new NetworkVariable<bool>(false);

    // Flag interno del servidor (para cumplir el criterio de no repetir eventos)
    private bool juegoTerminado = false;
    void Update()
    {
        // Si estamos en red y NO somos servidor, salimos
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient && !IsServer) return;

        // Si ya terminó o no arrancó, salimos
        if (juegoTerminado || !carreraActiva.Value) return;

        // Si llega acá, o es SinglePlayer puro (no hay NetworkManager) o es el Host
        // (Usamos un float normal o la network variable según prefieras)
        tiempoActual.Value += Time.deltaTime;

        if (tiempoActual.Value >= tiempoMaximo)
        {
            PerderJuegoPorTiempo();
        }
    }

    // Método para arrancar el cronómetro cuando den la señal de largada
    public void IniciarCarrera()
    {
        if (!IsServer) return;
        carreraActiva.Value = true;
        juegoTerminado = false;
    }

    
    public void RegistrarLlegada(ulong idGanador)
    {
        if (!IsServer || juegoTerminado || !carreraActiva.Value) return;

        juegoTerminado = true;
        carreraActiva.Value = false; // Frena el cronómetro para todos

        Debug.Log($"¡El jugador {idGanador} cruzó la meta en {tiempoActual.Value:F2} segundos!");

        
    }

    // Tu método PerderJuego
    public void PerderJuegoPorTiempo()
    {
        juegoTerminado = true;
        carreraActiva.Value = false;

        Debug.Log("¡Derrota global! Se agotó el tiempo límite para ambos.");

        
    }
    public override void OnNetworkSpawn()
    {
        // Apenas el Servidor carga el nivel, forzamos el inicio de la carrera
        if (IsServer)
        {
            IniciarCarrera();
        }
    }
}