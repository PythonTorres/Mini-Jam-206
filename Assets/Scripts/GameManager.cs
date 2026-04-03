using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IShotHandler {
    public static GameManager Instance;
    public PlayerScript player;
    public EnemyScript enemy;
    public FirstPersonCamera cameraScript;
    public Collider targetCollider;

    public CanvasGroup deadScreen;
    public CanvasGroup winScreen;
    public CanvasGroup jailScreen;

    private float enemyShotTime = float.PositiveInfinity;
    private float playerShotTime = float.PositiveInfinity;
    private float enemyAimTime = float.PositiveInfinity;
    private float playerAimTime = float.PositiveInfinity;
    private bool shotDone = false;
    private bool playerMissed = false;

    void Awake() {
        Instance = this;
        deadScreen.alpha = 0f;
        jailScreen.alpha = 0f;
        winScreen.alpha = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (shotDone) {
            if (enemyShotTime < playerShotTime || playerMissed) {
                Debug.Log("Defeat by shot");
                cameraScript.Die();
                player.die();
                deadScreen.GetComponent<FadeAppearScript>().turnOn();
                shotDone = false;
                return;
            }
            Vector3 hitPos = shotCalculation();
            if (hitPos.Equals(Vector3.zero)) {
                Debug.Log("Player missed");
                playerMissed = true;
                shotDone = false;
                return;
            }
            if (enemyAimTime > playerAimTime) {
                Debug.Log("Defeat by aim time");
                enemy.die(hitPos);
                jailScreen.GetComponent<FadeAppearScript>().turnOn();
                shotDone = false;
                return;
            }
            Debug.Log("Win");
            enemy.die(hitPos);
            winScreen.GetComponent<FadeAppearScript>().turnOn();
            shotDone = false;
        }
    }

    private Vector3 shotCalculation() {
        Vector3 rayStart = cameraScript.gameObject.transform.position;
        Vector3 rayDir = cameraScript.gameObject.transform.forward;
        bool didHit = Physics.Raycast(rayStart, rayDir, out RaycastHit hit);
        if (didHit && hit.collider.Equals(targetCollider)) {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void registerPlayerShot(float time) {
        playerShotTime = time;
        shotDone = true;
    }

    public void registerEnemyShot(float time) {
        enemyShotTime = time;
        shotDone = true;
    }

    public void registerPlayerAim(float time) {
        playerAimTime = time;
    }

    public void registerEnemyAim(float time) {
        enemyAimTime = time;
    }

    public void OnShot() {
        enemy.takeAim();
    }

    public void Continue() {
        var current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current + 1);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
