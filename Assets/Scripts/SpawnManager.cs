using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Точки спавна")]
    public Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        int index = NetworkManager.Singleton.ConnectedClientsList.Count - 1;

        if (index >= spawnPoints.Length) index = index % spawnPoints.Length;

        Transform spawnPoint = spawnPoints[index];

        GameObject playerInstance = Instantiate(
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}