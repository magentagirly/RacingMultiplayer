using Unity.Netcode;
using UnityEngine;

public class PlayerCameraSetup : MonoBehaviour // Теперь это обычный MonoBehaviour!
{
    private void Start()
    {
        // Ищем сетевой компонент на нашей машине (родителе)
        NetworkObject carNetworkObject = GetComponentInParent<NetworkObject>();

        if (carNetworkObject != null)
        {
            // Если машина принадлежит другому игроку, выключаем эту камеру
            if (!carNetworkObject.IsOwner)
            {
                gameObject.SetActive(false);
            }
        }
    }
}