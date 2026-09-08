using Unity.Netcode;
using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    public Transform puntoDeRespawn; // Lo dejamos para que los tests de Unity no lloren

    private void OnTriggerEnter(Collider other) { TryRespawn(other); }
    private void OnTriggerStay(Collider other) { TryRespawn(other); }

    private void TryRespawn(Collider other)
    {
        PlayerNetworkSetup networkPlayer = other.GetComponentInParent<PlayerNetworkSetup>();
        if (networkPlayer != null)
        {
            if (networkPlayer.IsSpawned && !networkPlayer.IsServer) return;
            networkPlayer.RespawnAlUltimoCheckpoint();
            return;
        }

        // Por si se cae un objeto común con Rigidbody
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || !rb.CompareTag("Player")) return;

        if (puntoDeRespawn != null)
        {
            rb.position = puntoDeRespawn.position;
            rb.rotation = puntoDeRespawn.rotation;
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
