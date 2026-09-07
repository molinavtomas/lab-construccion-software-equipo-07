using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [Header("Checkpoint")]
    private Vector3 ultimoCheckpointPos;
    private Quaternion ultimoCheckpointRot;
    private bool tieneCheckpointCustom = false;

    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();

        // Si arranca el juego, el primer checkpoint por defecto es dónde apareció
        ultimoCheckpointPos = transform.position;
        ultimoCheckpointRot = transform.rotation;
    }

    public void GuardarCheckpoint(Vector3 nuevaPos, Quaternion nuevaRot)
    {
        ultimoCheckpointPos = nuevaPos;
        ultimoCheckpointRot = nuevaRot;
        tieneCheckpointCustom = true;
        Debug.Log("Checkpoint guardado correctamente para este jugador.");
    }

    public void RespawnAlUltimoCheckpoint()
    {
        Respawn(ultimoCheckpointPos, ultimoCheckpointRot);
    }

    public void Respawn(Vector3 posicion, Quaternion rotacion)
    {
        if (IsServer)
        {
            RespawnClientRpc(posicion, rotacion);
        }
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 posicion, Quaternion rotacion)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = posicion;
        transform.rotation = rotacion;
    }
}