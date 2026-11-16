using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode; // Добавляем пространство имен Netcode

public class PlayerMovementTests
{
    private GameObject _player;
    private CarController _carController;
    private Rigidbody _rb;

    [SetUp]
    public void Setup()
    {
        // 1. Создаем тестовую машину
        _player = new GameObject("TestCar");

        // 2. Добавляем физику (обязательно для CarController)
        _rb = _player.AddComponent<Rigidbody>();

        // 3. Добавляем ваш скрипт управления
        _carController = _player.AddComponent<CarController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_player);
    }

    [UnityTest]
    public IEnumerator CarMovesForward_WhenThrottleApplied()
    {
        // ВАЖНО: В Unit-тесте нет NetworkManager, поэтому мы используем Reflection, 
        // чтобы обмануть скрипт и сказать ему: "Ты сервер, и ввод уже пришел".

        // 1. Устанавливаем свойство IsServer в true
        var isServerProperty = typeof(NetworkBehaviour).GetProperty("IsServer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        // В разных версиях Netcode свойство может быть защищенным, обходим это
        if (isServerProperty != null) isServerProperty.SetValue(_carController, true);

        // 2. Устанавливаем приватную переменную currentVerticalInput в 1 (нажатие вперед)
        var inputField = typeof(CarController).GetField("currentVerticalInput", BindingFlags.Instance | BindingFlags.NonPublic);
        if (inputField != null) inputField.SetValue(_carController, 1f);

        // Запоминаем начальную позицию
        Vector3 startPos = _player.transform.position;

        // 3. Ждем несколько кадров физики, чтобы FixedUpdate сработал и машина поехала
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // 4. Проверяем, сдвинулась ли машина
        Vector3 endPos = _player.transform.position;

        Assert.AreNotEqual(startPos, endPos, "Ошибка: Машина не сдвинулась с места при нажатии газа!");
        Assert.Greater(endPos.z, startPos.z, "Ошибка: Машина поехала не вперед!");
    }
}