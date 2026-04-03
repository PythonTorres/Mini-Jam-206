using UnityEngine;

public class EnemyScript : MonoBehaviour {
    public Animator animator;
    public float shotDelay;
    public GameObject ragdoll;
    public GameObject kinematicModel;
    public float shotForce;
    public ParticleSystem hitEffect;
    public ParticleSystem shotEffect;

    public bool isAimed { get; private set; }
    private float timeSinceAimed = 0f;
    private bool isDead = false;
    private bool hasFired = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        animator.enabled = false;
        ragdoll.SetActive(false);
        shotEffect.gameObject.SetActive(false);
        hasFired = false;
    }

    // Update is called once per frame
    void Update() {
        if (isDead) return;
        if (timeSinceAimed > shotDelay) {
            timeSinceAimed = 0f;
            isAimed = false;
            makeShot();
        }
        if (isAimed) {
            timeSinceAimed += Time.deltaTime;
        }
    }

    public void takeAim() {
        if (isAimed) return;
        animator.enabled = true;
        isAimed = true;
        GameManager.Instance.registerEnemyAim(Time.time);
    }

    public void takeAimReactively() {
        if (isAimed) return;
        animator.enabled = true;
        isAimed = true;
        GameManager.Instance.registerEnemyAim(Time.time + 0.1f);
    }

    public void makeShot() {
        if (hasFired) return;
        Debug.Log("enemy shot");
        SoundManager.Instance.Play("shot");
        shotEffect.gameObject.SetActive(true);
        GameManager.Instance.registerEnemyShot(Time.time);
        hasFired = true;
    }

    public void die(Vector3 hitPos) {
        ragdoll.SetActive(true);
        kinematicModel.SetActive(false);
        
        Rigidbody closestBone = GetClosestRigidbody(hitPos);
        closestBone.AddForceAtPosition(shotForce * -transform.forward, hitPos);
        
        ParticleSystem hitEffectObj = Instantiate(hitEffect);
        var main = hitEffectObj.main;
        main.loop = true;
        hitEffectObj.transform.SetParent(closestBone.transform);
        hitEffectObj.transform.position = hitPos;
        
        isDead = true;
    }

    Vector3 ApplyForceAtPoint(Vector3 point, Vector3 force) {
        Rigidbody closest = null;
        float minDist = Mathf.Infinity;
        Vector3 contactPoint = Vector3.zero;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
            float dist = Vector3.Distance(rb.position, point);
            if (dist < minDist) {
                minDist = dist;
                closest = rb;
                contactPoint = closest.position;
            }
        }

        if (closest != null)
            closest.AddForceAtPosition(force, point);
        
        return contactPoint;
    }

    Rigidbody GetClosestRigidbody(Vector3 point) {
        Rigidbody closest = null;
        float minDist = Mathf.Infinity;

        foreach (Rigidbody rb in ragdoll.GetComponentsInChildren<Rigidbody>()) {
            float dist = Vector3.Distance(rb.position, point);
            if (dist < minDist) {
                minDist = dist;
                closest = rb;
            }
        }
        return closest;
    }
}
