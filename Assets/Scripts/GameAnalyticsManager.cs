using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsManager : MonoBehaviour
{
    private void Awake()
    {
        GameAnalytics.Initialize();
        Debug.Log("GameAnalytics инициализирован.");
    }


    public void LogRaceStart()
    {
        GameAnalytics.NewDesignEvent("Race:Start");
        Debug.Log("Событие: Race:Start отправлено");
    }

    public void LogRaceFinish(float timeInSeconds)
    {
        GameAnalytics.NewDesignEvent("Race:Finish", timeInSeconds);
        Debug.Log($"Событие: Race:Finish отправлено. Время: {timeInSeconds}");
    }

    public void LogShopPurchase(string itemId, int price)
    {
        GameAnalytics.NewDesignEvent($"Shop:Purchase:{itemId}", price);
        Debug.Log($"Событие: Shop:Purchase:{itemId} отправлено");
    }

    public void LogNitroUsed()
    {
        GameAnalytics.NewDesignEvent("Gameplay:NitroUsed");
        Debug.Log("Событие: Gameplay:NitroUsed отправлено");
    }

    private void OnApplicationQuit()
    {
        GameAnalytics.NewDesignEvent("App:Quit");
        Debug.Log("Событие: App:Quit отправлено (игра закрывается)");
    }
}