using Unity.Netcode;
using UnityEngine;

public class Llegada : MonoBehaviour
{
    [Header("Conexión con el sistema")]
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Clave de seguridad: Solo dejamos que el Servidor procese la meta
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.CompareTag("Player"))
        {
            // 2. Buscamos la identidad de red del jugador que acaba de chocar
            NetworkObject playerNetworkObject = other.GetComponent<NetworkObject>();

            if (playerNetworkObject != null)
            {
                // 3. Le pasamos el ID exacto (OwnerClientId) de ese jugador al Manager
                gameManager.RegistrarLlegada(playerNetworkObject.OwnerClientId);
            }
        }
    }
}