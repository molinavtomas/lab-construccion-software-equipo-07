using System.Collections.Generic;
using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    public Transform puntoDeRespawn;

    private readonly HashSet<int> jugadoresProcesados = new HashSet<int>();

    private void OnTriggerEnter(Collider other)
    {
        TryRespawn(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Respaldo para jugadores de red que ya estaban dentro del volumen
        // cuando el host recibio su posicion sincronizada.
        TryRespawn(other);
    }

    private void OnTriggerExit(Collider other)
    {
        jugadoresProcesados.Remove(GetPlayerId(other));
    }

    private void TryRespawn(Collider other)
    {
        PlayerNetworkSetup networkPlayer =
            other.GetComponentInParent<PlayerNetworkSetup>();

        if (networkPlayer != null)
        {
            // En multijugador el host es quien valida la muerte. El componente
            // del jugador se encarga de avisar al cliente propietario.
            if (networkPlayer.IsSpawned && !networkPlayer.IsServer)
                return;

            if (!jugadoresProcesados.Add(networkPlayer.gameObject.GetInstanceID()))
                return;

            Debug.Log("Situación inválida: el jugador de red cayó al vacío.");

            networkPlayer.Respawn(
                puntoDeRespawn.position,
                puntoDeRespawn.rotation
            );

            return;
        }

        // Compatibilidad con jugadores de escenas y pruebas sin Netcode.
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null || !rb.CompareTag("Player"))
            return;

        if (!jugadoresProcesados.Add(rb.gameObject.GetInstanceID()))
            return;

        Debug.Log("Situación inválida: el jugador cayó al vacío.");

        rb.position = puntoDeRespawn.position;
        rb.rotation = puntoDeRespawn.rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private static int GetPlayerId(Collider other)
    {
        PlayerNetworkSetup networkPlayer =
            other.GetComponentInParent<PlayerNetworkSetup>();

        if (networkPlayer != null)
            return networkPlayer.gameObject.GetInstanceID();

        Rigidbody rb = other.attachedRigidbody;
        return rb != null ? rb.gameObject.GetInstanceID() : 0;
    }
}
