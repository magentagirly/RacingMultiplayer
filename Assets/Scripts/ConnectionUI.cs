using Unity.Netcode;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Перетащите сюда Canvas с кнопками")]
    public GameObject uiPanel;

    [Header("Spawn Settings")]
    [Tooltip("Перетащите сюда SpawnPoint_1 и SpawnPoint_2")]
    public Transform[] spawnPoints;

    private void Start()
    {
        // Подписываемся на проверку ДО того, как запустим сеть
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }
        else
        {
            Debug.LogError("NetworkManager не найден на сцене!");
        }
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Запуск Хоста...");
            HideUI();
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Попытка подключения к Хосту...");
            HideUI();
        }
    }

    private void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    // Этот метод сервер вызывает перед тем, как заспавнить игрока
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Разрешаем подключение и создание объекта
        response.Approved = true;
        response.CreatePlayerObject = true;

        // Проверка на пустой массив точек (чтобы сервер не выдал ошибку)
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("ВНИМАНИЕ: Массив Spawn Points пуст! Спавн в центре (0,1,0)");
            response.Position = new Vector3(0, 1, 0);
            response.Rotation = Quaternion.identity;
            return;
        }

        // Вычисляем индекс точки (Хост = 0, Клиент = 1 и т.д.)
        int index = (int)(request.ClientNetworkId % (ulong)spawnPoints.Length);
        Transform targetSpawn = spawnPoints[index];

        // Выводим в консоль для проверки
        Debug.Log($"[СЕТЬ] Клиент {request.ClientNetworkId} подключается. Спавн на точке {index}. Координаты: {targetSpawn.position}");

        // Задаем позицию и поворот
        response.Position = targetSpawn.position;
        response.Rotation = targetSpawn.rotation;
    }
}