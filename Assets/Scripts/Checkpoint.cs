using Unity.Netcode;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerNetworkSetup networkPlayer = other.GetComponentInParent<PlayerNetworkSetup>();

        if (networkPlayer != null)
        {
            // Guardamos la posición y rotación exacta del checkpoint
            networkPlayer.GuardarCheckpoint(transform.position, transform.rotation);
        }
    }
}