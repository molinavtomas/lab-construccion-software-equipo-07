using Unity.Netcode;
using UnityEngine;

public class PlayerRespawn : NetworkBehaviour
{
    private Vector3 ultimoCheckpoint;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();

        // Al aparecer por primera vez, el checkpoint inicial es donde spawnea
        if (IsOwner)
        {
            ultimoCheckpoint = transform.position;
        }
    }

    private void Update()
    {
        // Solo evaluamos esto para el jugador local
        if (!IsOwner) return;

        // Si el jugador se cae al vacío (por ejemplo, si su coordenada Y baja de -20)
        if (transform.position.y < -20f)
        {
            TeleportToLastCheckpoint();
        }
    }

    public void ActualizarCheckpoint(Vector3 nuevaPosicion)
    {
        ultimoCheckpoint = nuevaPosicion;
        Debug.Log("¡Checkpoint actualizado!");
    }

    public void TeleportToLastCheckpoint()
    {
        // Frenamos las físicas para evitar que el personaje salga disparado al reaparecer
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Lo movemos a la posición del último checkpoint (un poco más arriba para que no se trabe en el piso)
        transform.position = ultimoCheckpoint + Vector3.up * 2f;
    }
}