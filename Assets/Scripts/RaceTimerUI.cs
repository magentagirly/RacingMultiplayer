using Unity.Netcode;
using UnityEngine;
using TMPro;

public class RaceTimerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI checkpointText; // Новое поле для чекпоинтов

    private RaceProgress progress;

    private void Update()
    {
        // 1. Ищем нашу машину
        if (progress == null)
        {
            FindMyCar();
            return;
        }

        // 2. Отображение времени и финиша
        if (progress.isFinished.Value)
        {
            double finalTime = progress.finishTime.Value - progress.raceStartTime.Value;
            timerText.text = $"FINISHED!\nTime: {finalTime:F2}s";
            if (checkpointText != null) checkpointText.text = ""; // Очищаем чекпоинты при финише
            return;
        }

        if (progress.raceStartTime.Value > 0)
        {
            double currentTime = NetworkManager.Singleton.ServerTime.Time - progress.raceStartTime.Value;
            timerText.text = $"Time: {currentTime:F2}s\nLap: {progress.currentLap.Value + 1}/{progress.totalLaps}";

            // 3. Отображение пройденных чекпоинтов
            if (checkpointText != null)
            {
                int passed = progress.currentCheckpoint.Value;
                int total = progress.totalCheckpoints;

                if (passed >= total)
                {
                    // Если все чекпоинты пройдены, говорим ехать на финиш
                    checkpointText.text = "GO TO FINISH!";
                    checkpointText.color = Color.yellow; // Подсвечиваем желтым
                }
                else
                {
                    // Показываем, сколько пройдено
                    checkpointText.text = $"Checkpoint: {passed} / {total}";
                    checkpointText.color = Color.white;
                }
            }
        }
        else
        {
            timerText.text = "Waiting for start...";
            if (checkpointText != null) checkpointText.text = "";
        }
    }

    private void FindMyCar()
    {
        RaceProgress[] allCars = FindObjectsByType<RaceProgress>(FindObjectsSortMode.None);

        foreach (var car in allCars)
        {
            if (car.IsOwner)
            {
                progress = car;
                break;
            }
        }
    }
}