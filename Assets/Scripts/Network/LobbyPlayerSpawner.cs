using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(NetworkManager))]
public class LobbyPlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject networkPlayerPrefab;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string spawnPointName = "Respawn";
    [SerializeField] private float playerSpacing = 1.5f;

    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();

        networkManager.ConnectionApprovalCallback = ApproveConnection;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void ApproveConnection(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;

        // No crear al jugador mientras permanece en el lobby.
        response.CreatePlayerObject = false;

        response.Position = null;
        response.Rotation = null;
        response.Pending = false;
    }

    private void OnServerStarted()
    {
        networkManager.SceneManager.OnLoadEventCompleted
            += OnLoadEventCompleted;
    }

    private void OnLoadEventCompleted(
        string sceneName,
        LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        if (!networkManager.IsServer || sceneName != gameSceneName)
            return;

        foreach (ulong clientId in networkManager.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Permite generar un jugador si alguien entra cuando
        // GameScene ya se encuentra abierta.
        if (networkManager.IsServer &&
            SceneManager.GetActiveScene().name == gameSceneName)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!networkManager.ConnectedClients.TryGetValue(
                clientId, out NetworkClient client))
            return;

        if (client.PlayerObject != null)
            return;

        Transform spawnPoint = null;
        GameObject spawnObject = GameObject.Find(spawnPointName);

        if (spawnObject != null)
            spawnPoint = spawnObject.transform;

        IReadOnlyList<ulong> connectedClientIds =
            networkManager.ConnectedClientsIds;

        int playerIndex = 0;

        for (int i = 0; i < connectedClientIds.Count; i++)
        {
            if (connectedClientIds[i] == clientId)
            {
                playerIndex = i;
                break;
            }
        }

        int playerCount = connectedClientIds.Count;

        float offset =
            (playerIndex - (playerCount - 1) * 0.5f) * playerSpacing;

        Vector3 position = spawnPoint != null
            ? spawnPoint.position + spawnPoint.right * offset
            : Vector3.right * offset;

        Quaternion rotation = spawnPoint != null
            ? spawnPoint.rotation * networkPlayerPrefab.transform.rotation
            : networkPlayerPrefab.transform.rotation;

        GameObject playerInstance =
            Instantiate(networkPlayerPrefab, position, rotation);

        NetworkObject networkObject =
            playerInstance.GetComponent<NetworkObject>();

        networkObject.SpawnAsPlayerObject(clientId, true);
    }

    private void OnDestroy()
    {
        if (networkManager == null)
            return;

        networkManager.OnServerStarted -= OnServerStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;

        if (networkManager.SceneManager != null)
        {
            networkManager.SceneManager.OnLoadEventCompleted
                -= OnLoadEventCompleted;
        }
    }
}