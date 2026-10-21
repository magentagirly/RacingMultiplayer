using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnPosition : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Выполняем только на сервере (Хосте)
        if (!IsServer) return;

        // Ищем наш ConnectionUI на сцене, чтобы взять из него точки
        ConnectionUI ui = FindAnyObjectByType<ConnectionUI>();

        if (ui != null && ui.spawnPoints != null && ui.spawnPoints.Length > 0)
        {
            // Вычисляем, на какую точку ставить (0 для хоста, 1 для клиента)
            int index = (int)(OwnerClientId % (ulong)ui.spawnPoints.Length);
            Transform targetSpawn = ui.spawnPoints[index];

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Принудительно двигаем Rigidbody
                rb.position = targetSpawn.position;
                rb.rotation = targetSpawn.rotation;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                transform.position = targetSpawn.position;
                transform.rotation = targetSpawn.rotation;
            }

            Debug.Log($"[СПАВН] Машина клиента {OwnerClientId} принудительно перемещена на SpawnPoint {index}");
        }
    }
}