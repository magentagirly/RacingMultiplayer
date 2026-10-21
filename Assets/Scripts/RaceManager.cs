using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class RaceManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Если в игре 2 игрока, запускаем гонку
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            StartCoroutine(StartRaceDelayed());
        }
    }

    private IEnumerator StartRaceDelayed()
    {
        // Ждем 1 секунду, чтобы машины точно появились на сцене
        yield return new WaitForSeconds(1f);

        Debug.Log("[ГОНКА] Начинаем заезд для всех!");
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                RaceProgress progress = client.PlayerObject.GetComponent<RaceProgress>();
                if (progress != null)
                {
                    progress.StartRaceServerRpc();
                }
            }
        }
    }
}