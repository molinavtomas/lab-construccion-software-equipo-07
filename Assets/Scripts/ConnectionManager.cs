using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField joinCodeInput;

    [Header("UI Display")]
    public Text codeDisplay; // Para mostrar el código en pantalla

    async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Autenticado anónimamente con ID: " + AuthenticationService.Instance.PlayerId);
        }
    }

    public async void StartHostWithRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            Debug.Log($"¡Partida creada con éxito! Código de unión: {joinCode}");

            // Mostrar el código en la UI si está asignado
            if (codeDisplay != null)
            {
                codeDisplay.text = "Código: " + joinCode;
            }

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                transport.SetRelayServerData(relayServerData);
            }

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void StartClientWithRelay()
    {
        string joinCode = joinCodeInput != null ? joinCodeInput.text : "";

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("¡Debes ingresar un código de Relay válido para unirte!");
            return;
        }

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                transport.SetRelayServerData(relayServerData);
            }

            NetworkManager.Singleton.StartClient();
            Debug.Log("¡Conectado a la partida mediante Relay!");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}