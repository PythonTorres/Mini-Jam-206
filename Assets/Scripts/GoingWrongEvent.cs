using UnityEngine;

public class GoingWrongEvent : MonoBehaviour
{
    public GameObject eventMainObject;
    public AudioClip eventMainSound;

    private bool OnAnimationFinishedExecuted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        eventMainObject.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OnAnimationFinished() {
        if (OnAnimationFinishedExecuted) return;
        eventMainObject.SetActive(true);
        gameObject.SetActive(false);
        Play();
        OnAnimationFinishedExecuted = true;
    }

    private void Play()
    {
        AudioSource.PlayClipAtPoint(eventMainSound, Camera.main.transform.position, AudioListener.volume / 4);
    }
}
