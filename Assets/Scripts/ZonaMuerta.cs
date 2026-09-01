using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    public Transform puntoDeRespawn;

    private void OnTriggerEnter(Collider other)
    {
        PlayerNetworkSetup networkPlayer =
            other.GetComponentInParent<PlayerNetworkSetup>();

        if (networkPlayer != null)
        {
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

        Debug.Log("Situación inválida: el jugador cayó al vacío.");

        rb.position = puntoDeRespawn.position;
        rb.rotation = puntoDeRespawn.rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
