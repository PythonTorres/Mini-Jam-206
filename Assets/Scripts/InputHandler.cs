using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour {

    public PlayerScript player;
    public EnemyScript enemy;
    public GameObject wantedList;

    public float zoomedFov = 30f;
    public float normalFov = 60f;
    public float zoomSpeed = 5f;

    private bool zoom = false;
    private PlayerInput playerInput;
    private Camera cam;

    void Awake() {
        playerInput = GetComponent<PlayerInput>();
        cam = GetComponent<Camera>();
    }

    void OnEnable() {
        playerInput.actions["Zoom"].performed += OnZoomPerformed;
        playerInput.actions["Zoom"].canceled  += OnZoomCanceled;
    }

    void OnDisable() {
        playerInput.actions["Zoom"].performed -= OnZoomPerformed;
        playerInput.actions["Zoom"].canceled  -= OnZoomCanceled;
    }

    private void OnZoomPerformed(InputAction.CallbackContext ctx) => zoom = true;
    private void OnZoomCanceled(InputAction.CallbackContext ctx)  => zoom = false;

    public void OnMainAction() {
        if (player.isAimed) {
            player.makeShot();
        }
        else {
            player.takeAim();
            if (!enemy.isAimed) enemy.takeAimReactively();
        }
    }

    void Update() {
        float targetFov = zoom ? zoomedFov : normalFov;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * zoomSpeed);
    }

    public void OnMenu() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
        #else
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("Main Menu");
        #endif
    }

    public void OnRemove() {
        var current = wantedList.GetComponent<RectTransform>().localPosition;
        current.y -= 300f;
        wantedList.GetComponent<RectTransform>().localPosition = current;
    }
}