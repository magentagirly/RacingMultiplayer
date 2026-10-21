using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 1500f; // Сила разгона (подобрана под массу)
    public float maxSpeed = 25f;       // Максимальная скорость
    public float turnSpeed = 120f;     // Скорость поворота
    public float brakeForce = 5f;      // Сила торможения

    private Rigidbody rb;

    // Переменные для хранения ввода (обновляются через RPC)
    private float currentVerticalInput;
    private float currentHorizontalInput;
    private bool currentBrakeInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Читаем ввод ТОЛЬКО на владельце (клиенте) каждый кадр
        if (!IsOwner) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float h = 0f;
        float v = 0f;

        if (OwnerClientId == 0) // Хост (WASD)
        {
            if (keyboard.aKey.isPressed) h -= 1f;
            if (keyboard.dKey.isPressed) h += 1f;
            if (keyboard.wKey.isPressed) v += 1f;
            if (keyboard.sKey.isPressed) v -= 1f;
        }
        else // Клиент (Стрелочки)
        {
            if (keyboard.leftArrowKey.isPressed) h -= 1f;
            if (keyboard.rightArrowKey.isPressed) h += 1f;
            if (keyboard.upArrowKey.isPressed) v += 1f;
            if (keyboard.downArrowKey.isPressed) v -= 1f;
        }

        bool brake = keyboard.spaceKey.isPressed;

        // Отправляем ввод на сервер
        SendInputServerRpc(h, v, brake);
    }

    [ServerRpc]
    private void SendInputServerRpc(float h, float v, bool brake)
    {
        // Сервер просто запоминает, какие кнопки зажал игрок
        currentHorizontalInput = h;
        currentVerticalInput = v;
        currentBrakeInput = brake;
    }

    private void FixedUpdate()
    {
        if (!IsServer || rb == null) return;

        // 1. Движение (Задаем скорость напрямую, игнорируя массу)
        // Умножаем на 15, чтобы задать максимальную скорость
        Vector3 targetVelocity = transform.forward * currentVerticalInput * 15f;

        // Сохраняем текущую скорость падения (гравитацию), чтобы машина не парила в воздухе
        targetVelocity.y = rb.linearVelocity.y;

        // Плавно задаем новую скорость
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);

        // 2. Поворот
        if (rb.linearVelocity.magnitude > 0.5f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, currentHorizontalInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}