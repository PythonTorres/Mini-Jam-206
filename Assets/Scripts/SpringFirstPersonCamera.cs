using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour {
    [Header("Mount")]
    [Tooltip("Пустой объект — точка крепления камеры")]
    public Transform mountPoint;

    [Header("Clamp Angles")]
    [Range(-90f, 0f)]
    public float minPitch = -80f;
    [Range(0f, 90f)]
    public float maxPitch = 80f;
    [Tooltip("Ограничение влево/вправо. 0 = свободный поворот")]
    [Range(0f, 180f)]
    public float maxYaw = 0f;

    [Header("Look Settings")]
    public float sensitivity = 0.15f;

    [Header("Smoothing")]
    [Tooltip("Время сглаживания поворота. 0 = мгновенно")]
    [Range(0f, 0.2f)]
    public float rotationSmoothTime = 0.05f;

    [Header("Tilt (наклон при повороте)")]
    [Tooltip("Максимальный угол наклона по Z")]
    [Range(0f, 10f)]
    public float maxTiltAngle = 3f;
    [Tooltip("Скорость наклона")]
    [Range(0f, 0.2f)]
    public float tiltSmoothTime = 0.1f;

    private float _yaw;
    private float _pitch;
    private float _baseYaw;
    private Vector2 _lookInput;

    // SmoothDamp state
    private float _currentYaw;
    private float _currentPitch;
    private float _yawVelocity;
    private float _pitchVelocity;

    // Tilt state
    private float _currentTilt;
    private float _tiltVelocity;
    private bool isDead;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mountPoint != null)
            _baseYaw = mountPoint.eulerAngles.y;

        _yaw = _baseYaw;
        _currentYaw = _yaw;
        _currentPitch = 0f;
    }

    void OnLook(InputValue value) {
        _lookInput = value.Get<Vector2>();
    }

    void LateUpdate() {
        if (isDead) {
            transform.position = mountPoint.transform.position;
            transform.rotation = mountPoint.transform.rotation;
            return;
        }

        if (mountPoint != null)
            transform.position = mountPoint.position;

        // Накапливаем целевые углы
        _yaw += _lookInput.x * sensitivity;
        _pitch -= _lookInput.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        if (maxYaw > 0f) {
            float delta = Mathf.DeltaAngle(_baseYaw, _yaw);
            delta = Mathf.Clamp(delta, -maxYaw, maxYaw);
            _yaw = _baseYaw + delta;
        }

        // Сглаживаем вращение
        _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _yaw, ref _yawVelocity, rotationSmoothTime);
        _currentPitch = Mathf.SmoothDampAngle(_currentPitch, _pitch, ref _pitchVelocity, rotationSmoothTime);

        // Tilt — наклон по Z зависит от скорости горизонтального поворота
        float targetTilt = -_lookInput.x * maxTiltAngle * sensitivity * 10f;
        targetTilt = Mathf.Clamp(targetTilt, -maxTiltAngle, maxTiltAngle);
        _currentTilt = Mathf.SmoothDamp(_currentTilt, targetTilt, ref _tiltVelocity, tiltSmoothTime);

        transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, _currentTilt);
    }

    public void SyncBaseYaw() {
        if (mountPoint != null)
            _baseYaw = mountPoint.eulerAngles.y;
    }

    public void Die() {
        isDead = true;
        // 3. lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. disable player input & controller
        GetComponent<PlayerInput>().enabled = false;
    }
}