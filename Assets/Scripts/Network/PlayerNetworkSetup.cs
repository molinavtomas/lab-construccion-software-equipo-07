using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [Header("Componentes locales")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private Move move;
    [SerializeField] private WallRun wallRun;
    [SerializeField] private Grappling grappling;

    private Rigidbody playerRigidbody;
    private NetworkTransform networkTransform;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        networkTransform = GetComponent<NetworkTransform>();
    }

    public override void OnNetworkSpawn()
    {
        bool esPropietario = IsOwner;

        if (playerCamera != null)
            playerCamera.enabled = esPropietario;

        if (audioListener != null)
            audioListener.enabled = esPropietario;

        if (cameraMovement != null)
            cameraMovement.enabled = esPropietario;

        if (move != null)
            move.enabled = esPropietario;

        if (wallRun != null)
            wallRun.enabled = esPropietario;

        if (grappling != null)
            grappling.enabled = esPropietario;
    }

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        // El NetworkTransform del jugador usa autoridad del propietario.
        // Las copias remotas no deben intentar corregir su posicion.
        if (IsSpawned && !IsOwner)
            return;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.position = position;
            playerRigidbody.rotation = rotation;
        }
        else
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        // Teleport evita que la interpolacion recorra visualmente toda la
        // distancia desde la zona de muerte hasta el punto de respawn.
        if (IsSpawned && networkTransform != null)
        {
            networkTransform.Teleport(
                position,
                rotation,
                transform.localScale
            );
        }
    }
}
