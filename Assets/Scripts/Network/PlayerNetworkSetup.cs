using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [Header("Componentes locales")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Grappling grappling;

    public override void OnNetworkSpawn()
    {
        bool esPropietario = IsOwner;

        playerCamera.enabled = esPropietario;
        audioListener.enabled = esPropietario;
        cameraMovement.enabled = esPropietario;
        playerMovement.enabled = esPropietario;
        grappling.enabled = esPropietario;
    }
}