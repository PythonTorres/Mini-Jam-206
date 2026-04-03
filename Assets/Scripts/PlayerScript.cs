using UnityEngine;

public class PlayerScript : MonoBehaviour {
    public GameObject crosshair;
    public GameObject ragdoll;
    public Transform headShotPos;
    public float headShotForce;
    public GameObject kinematicModel;
    public GameObject revolver;
    public ParticleSystem shotEffect;
    public bool isAimed { get; private set; }

    private bool hasFired = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        crosshair.SetActive(false);
        ragdoll.SetActive(false);
        revolver.SetActive(false);
        shotEffect.gameObject.SetActive(false);
        SoundManager.Instance.Play("wind");
    }

    // Update is called once per frame
    void Update() {

    }

    public void takeAim() {
        isAimed = true;
        crosshair.SetActive(true);
        revolver.SetActive(true);
        SoundManager.Instance.Play("aim");
        GameManager.Instance.registerPlayerAim(Time.time);
    }

    public void makeShot() {
        if (hasFired) {
            Debug.Log("player already fired");
            return;
        }
        Debug.Log("player shot");
        SoundManager.Instance.Play("shot");
        hasFired = true;
        shotEffect.gameObject.SetActive(true);
        revolver.GetComponent<PositionAnimator>().PlayQueue("Fire", "Cooldown");
        GameManager.Instance.registerPlayerShot(Time.time);
    }

    public void die() {
        ragdoll.SetActive(true);
        kinematicModel.SetActive(false);
        revolver.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        headShotPos.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(headShotForce * transform.forward, headShotPos.position);
    }
}
