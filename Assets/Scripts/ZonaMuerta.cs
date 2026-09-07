using Unity.Netcode;
using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    // Restablecemos esta variable para que los tests de PlayMode no den error de compilación
    public Transform puntoDeRespawn;

    private void OnTriggerEnter(Collider other)
    {
        TryRespawn(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryRespawn(other);
    }

    private void TryRespawn(Collider other)
    {
        PlayerNetworkSetup networkPlayer = other.GetComponentInParent<PlayerNetworkSetup>();

        if (networkPlayer != null)
        {
            if (networkPlayer.IsSpawned && !networkPlayer.IsServer)
                return;

            Debug.Log("Situación inválida: el jugador de red cayó al vacío. Respawneando en su checkpoint.");

            // El jugador se encarga de ir a su último checkpoint
            networkPlayer.RespawnAlUltimoCheckpoint();
            return;
        }

        // Compatibilidad con jugadores de escenas y pruebas sin Netcode.
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null || !rb.CompareTag("Player"))
            return;

        Debug.Log("Situación inválida: el jugador cayó al vacío.");

        if (puntoDeRespawn != null)
        {
            rb.position = puntoDeRespawn.position;
            rb.rotation = puntoDeRespawn.rotation;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}