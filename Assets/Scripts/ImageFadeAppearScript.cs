using UnityEngine;
using UnityEngine.UI;

public class ImageFadeAppearScript : MonoBehaviour
{
    public float duration = 1f; 
    private bool on = false;
    private Color cg;
    private float t = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        on = false;
        cg = GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (on) {
            if (cg.a >= 1f) return;

            t += Time.deltaTime;
            cg.a = Mathf.Clamp01(t / duration);
        } else {
            if (cg.a <= 0f) return;

            t -= Time.deltaTime;
            cg.a = Mathf.Clamp01(t / duration);
        }
    }

    public void turnOn(float duration) {
        on = true;
        this.duration = duration;
        t = 0f;
    }

    public void turnOff(float duration) {
        on = false;
        this.duration = duration;
        t = duration;
    }
    public void turnOn() {
        on = true;
        t = 0f;
    }

    public void turnOff() {
        on = false;
        t = duration;
    }
}
