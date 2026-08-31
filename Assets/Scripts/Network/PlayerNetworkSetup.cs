using Unity.Netcode;
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
}