using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    [Header("Configuración del Nivel")]
    public float tiempoMaximo = 120f;

    [Header("Estado del Juego (Sincronizado)")]
    public readonly NetworkVariable<float> tiempoActual = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public readonly NetworkVariable<bool> carreraActiva = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private bool juegoTerminado;

    private void Update()
    {
        if (!IsSpawned || !IsServer || juegoTerminado || !carreraActiva.Value)
            return;

        tiempoActual.Value += Time.deltaTime;

        if (tiempoActual.Value >= tiempoMaximo)
            PerderJuegoPorTiempo();
    }

    public bool IniciarCarrera()
    {
        if (!IsServer || juegoTerminado || carreraActiva.Value)
            return false;

        tiempoActual.Value = 0f;
        carreraActiva.Value = true;
        return true;
    }

    public void RegistrarLlegada(ulong idGanador)
    {
        if (!IsServer || juegoTerminado || !carreraActiva.Value)
            return;

        juegoTerminado = true;
        carreraActiva.Value = false;

        Debug.Log(
            $"¡El jugador {idGanador} cruzó la meta en " +
            $"{tiempoActual.Value:F2} segundos!"
        );
    }

    public void PerderJuegoPorTiempo()
    {
        if (!IsServer || juegoTerminado || !carreraActiva.Value)
            return;

        juegoTerminado = true;
        carreraActiva.Value = false;

        Debug.Log("¡Derrota global! Se agotó el tiempo límite para ambos.");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        tiempoActual.Value = 0f;
        carreraActiva.Value = false;
        juegoTerminado = false;
    }
}
