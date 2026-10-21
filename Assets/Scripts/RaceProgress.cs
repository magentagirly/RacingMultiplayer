using Unity.Netcode;
using UnityEngine;

public class RaceProgress : NetworkBehaviour
{
    [Header("Race Settings")]
    public int totalCheckpoints = 3;
    public int totalLaps = 2;

    public NetworkVariable<int> currentCheckpoint = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentLap = new NetworkVariable<int>(0);
    public NetworkVariable<double> raceStartTime = new NetworkVariable<double>(0);
    public NetworkVariable<double> finishTime = new NetworkVariable<double>(-1);
    public NetworkVariable<bool> isFinished = new NetworkVariable<bool>(false);

    private void OnTriggerEnter(Collider other)
    {
        // Логику обрабатывает только сервер
        if (!IsServer) return;

        // Проверка чекпоинтов
        Checkpoint cp = other.GetComponent<Checkpoint>();
        if (cp != null)
        {
            Debug.Log($"[ГОНКА] Машина задела чекпоинт {cp.checkpointIndex}. Текущий нужный: {currentCheckpoint.Value}");
            if (cp.checkpointIndex == currentCheckpoint.Value)
            {
                currentCheckpoint.Value++;
                Debug.Log($"[ГОНКА] Чекпоинт засчитан! Следующий: {currentCheckpoint.Value}");
            }
        }

        // Проверка финиша
        if (other.CompareTag("Finish") && !isFinished.Value)
        {
            Debug.Log($"[ГОНКА] Пересечена финишная линия! Чекпоинтов пройдено: {currentCheckpoint.Value}");
            if (currentCheckpoint.Value >= totalCheckpoints)
            {
                currentLap.Value++;
                currentCheckpoint.Value = 0;

                if (currentLap.Value >= totalLaps)
                {
                    isFinished.Value = true;
                    finishTime.Value = NetworkManager.Singleton.ServerTime.Time;
                    Debug.Log($"[ГОНКА] ФИНИШ! Игрок закончил.");
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartRaceServerRpc()
    {
        Debug.Log("[ГОНКА] Сервер запустил таймер!");
        raceStartTime.Value = NetworkManager.Singleton.ServerTime.Time;
        currentLap.Value = 0;
        currentCheckpoint.Value = 0;
        isFinished.Value = false;
        finishTime.Value = -1;
    }
}